using UnityEngine;
using BestHTTP.SignalR;
using BestHTTP.SignalR.Hubs;
using BestHTTP.SignalR.Messages;
using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using System.Runtime;
using GraphLibrary;
using UnityEngine.XR.iOS;
using Lean.Touch;

public class CommunicationSOD : Singleton<CommunicationSOD>
{
    public Connection connection;
    public GameObject marker;

    [Serializable]
    public struct DriveFile
    {
        public string name;
        public string id;

        public DriveFile(string _name, string _id)
        {
            name = _name;
            id = _id;
        }
    }

    public Dictionary<string, string> fileNames;

    void Awake()
    {
        GCSettings.LatencyMode = GCLatencyMode.LowLatency;
        StartConnection();
    }

    void OnDestroy()
    {
        StopConnection();
        connection.Close();
    }

    public void StartConnection()
    {
        Uri URI = new Uri("http://162.246.156.146:1337/signalR");
        connection = new Connection(URI, "GoogleDriveHub");
        connection.OnConnected += Connection_OnConnected;       // this was there already
        connection.Open();


    }

    void Connection_OnConnected(Connection connection)
    {
        connection["GoogleDriveHub"].Call("ClientConnected", "iPad Pro", 1);
        connection["GoogleDriveHub"].On("OnViewGoogleFiles", GoogleDriveHub_ViewGoogleFiles);
        connection["GoogleDriveHub"].On("OnLoadNewDataFile", GoogleDriveHub_LoadNewDataFile);
        connection["GoogleDriveHub"].On("OnLoadWorkspace", GoogleDriveHub_LoadWorkspace);
        connection["GoogleDriveHub"].On("OnSetWorkspaceId", GoogleDriveHub_SetWorkspaceId);

        Debug.Log("Connected successfully");
        connection["GoogleDriveHub"].Call("ViewGoogleDriveFiles");
    }

    public void StopConnection()
    {
        print("Connection stopped.");
        connection["GoogleDriveHub"].Off("OnViewGoogleFiles");
        connection["GoogleDriveHub"].Off("OnLoadNewDataFile");
        connection["GoogleDriveHub"].Off("OnLoadWorkspace");
        connection["GoogleDriveHub"].Off("OnSetWorkspaceId");

        //connection["KinectHub"].Off("OnGesture");
        //connection["KinectHub"].Off("OnPersonEnter");
        //connection["KinectHub"].Off("OnPersonExit");
        //connection["KinectHub"].Off("OnUpdatePeople");
    }

	public void Update()
    {
        //if (Input.GetKeyDown(KeyCode.Space))
        //{
        //    connection["GoogleDriveHub"].Call("ViewGoogleDriveFiles");
        //}

        //if (Input.GetKeyDown(KeyCode.L))
        //{
        //    connection["GoogleDriveHub"].Call("GetFile", "DeerfootData.csv", "1XLLxFMD0yJTEoM336wj_Alkl0jD-t2wF");
        //}
    }

    void GoogleDriveHub_ViewGoogleFiles(Hub hub, MethodCallMessage msg)
    {
        //Dictionary<string, string> fileNames = JsonConvert.DeserializeObject<Dictionary<string, string>>((string)msg.Arguments[0]);
        //Dictionary<string, string> mimeTypes = JsonConvert.DeserializeObject<Dictionary<string, string>>((string)msg.Arguments[1]);

        //object[] parameters = { fileNames, mimeTypes };

        //GameObject.Find("SpawnMenu").BroadcastMessage("PopulateDatasetList", parameters);

        print("Creating graph.");
        connection["GoogleDriveHub"].Call("GetFile", "DeerfootData.csv", "1XLLxFMD0yJTEoM336wj_Alkl0jD-t2wF");
        print("Graph Created.");
    }

    void GoogleDriveHub_LoadNewDataFile(Hub hub, MethodCallMessage msg)
    {
        Transform cam = Camera.main.transform;
        //Vector3 pos = ((cam.position + cam.forward * 2f) + Vector3.down * 0.5f + cam.right);

        //HGGraphManager.Instance.CreateScatterplot((string)msg.Arguments[0], (string)msg.Arguments[1], pos);
        //GraphManager.workspaceName = DateTime.Now.ToString().Replace(" ", "_");
        //Debug.Log(GraphManager.workspaceName);

        GameObject scatterPlot = GraphManager.CreateScatterPlot((string)msg.Arguments[0], (string)msg.Arguments[1]);
        scatterPlot.AddComponent<UnityARHitTestExample>();
        scatterPlot.GetComponent<UnityARHitTestExample>().m_HitTransform = marker.transform;

        scatterPlot.AddComponent<LeanScale>();
        scatterPlot.AddComponent<LeanRotate>();
        scatterPlot.AddComponent<LeanTranslateSmooth>();
        scatterPlot.GetComponent<LeanRotate>().RotateAxis.y = 0.5f;
        scatterPlot.GetComponent<LeanRotate>().RotateAxis.z = 0;

        ScatterPlotCreator spc = scatterPlot.GetComponent<ScatterPlotCreator>();

        spc.ChangeAttribute(AttributeType.x, "Min Temp", true);
        spc.ChangeAttribute(AttributeType.y, "Max Temp", true);
        spc.ChangeAttribute(AttributeType.z, "Mean Temp", true);

        scatterPlot.transform.SetParent(marker.transform);
        scatterPlot.transform.localScale = new Vector3(5, 5, 5);
        scatterPlot.transform.localPosition = Vector3.zero;
    }

    void GoogleDriveHub_LoadWorkspace(Hub hub, MethodCallMessage msg)
    {
        Dictionary<string, string> datasets = JsonConvert.DeserializeObject<Dictionary<string, string>>((string)msg.Arguments[1]);

        foreach(KeyValuePair<string, string> kvp in datasets)
        {
            GraphManager.CreateDataset(kvp.Key, kvp.Value);
        }

        GraphManager.ReadFromJSON(null, (string)msg.Arguments[0]);
    }

    void GoogleDriveHub_SetWorkspaceId(Hub hub, MethodCallMessage msg)
    {
        GraphManager.workspaceId = (string)msg.Arguments[0];
    }
}