using GraphLibrary;
using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Contains functions and manages graphs that exist within the scene.
/// </summary>
public class GraphManager : Singleton<GraphManager>
{
    public static string workspaceName, workspaceId = null;

    public static Dictionary<string, Dataset> datasets = new Dictionary<string, Dataset>();
    public static Dictionary<string, List<GameObject>> graphs = new Dictionary<string, List<GameObject>>();

    public static List<int> selectedIndicies = new List<int>();

    /// <summary>
    /// Creates a dataset object for a specific dataset.
    /// </summary>
    /// <param name="pathName">The entire path name starting from the Resources folder.</param>
    /// <returns>Dataset object.</returns>
    public static Dataset CreateDataset(string datasetName, string allText)
    {
        datasets[datasetName] = new Dataset(datasetName, allText);
        return datasets[datasetName];
    }

    /// <summary>
    /// If dataset does not exist, creates it and adds it to dictionary datasets.    
    /// /// If list for scatterPlots does not exist for specific file name, creates it and adds it to dictionary datasets.
    /// Creates scatterplot object and adds to list.
    /// </summary>
    /// <param name="filename">Filename of the dataset.</param>
    /// <returns>ScatterPlot GameObject</returns>
    public static GameObject CreateScatterPlot(string datasetName, string allText = null)
    {
        if (!datasets.ContainsKey(datasetName))
        {
            if (allText == null)
                throw new Exception("Dataset does not exist and no dataset available.");
            
            else
                CreateDataset(datasetName, allText);
        }
        
        ScatterPlot s = new ScatterPlot(datasets[datasetName]);

        if (!graphs.ContainsKey(datasetName))
        {
            graphs[datasetName] = new List<GameObject>();
        }

        string title = datasetName + "_" + graphs[datasetName].Count;
        GameObject plot = new GameObject(title);
        ScatterPlotCreator creator = plot.AddComponent<ScatterPlotCreator>();
        creator.OnCreate(s, title);

        graphs[datasetName].Add(plot);

        return plot;
    }

    public static void DestroyScatterPlot(string filename, int index)
    {
        Destroy(graphs[filename][index]);
    }

    public static void UpdateGraph()
    {
        foreach (int i in selectedIndicies)
        {
            graphs["Data"][0].GetComponent<ScatterPlotCreator>().points[i].SetActive(true);
        }
    }

    public static void WriteToJSON()
    {
        Dictionary<string, List<GraphObject>> graphObjects = new Dictionary<string, List<GraphObject>>();
        foreach (string s in graphs.Keys)
        {
            graphObjects[s] = new List<GraphObject>();

            for (int i = 0; i < graphs[s].Count; i++)
            {
                GraphObject graphObject = new GraphObject(graphs[s][i].GetComponent<GraphCreator>().plot, graphs[s][i].transform);
                graphObjects[s].Add(graphObject);
            }
        }

        string l = JsonConvert.SerializeObject(graphObjects);
        CommunicationSOD.Instance.connection["GoogleDriveHub"].Call("UploadWorkspace", workspaceName + ".json", l, workspaceId);
    }

    public static void ReadFromJSON(string _workspaceName, string allText)
    {
        Dictionary<string, List<GraphObject>> graphObjects = JsonConvert.DeserializeObject<Dictionary<string, List<GraphObject>>>(allText);

        foreach(KeyValuePair<string, List<GraphObject>> entry in graphObjects)
        {
            foreach (GraphObject value in entry.Value)
            {
                ScatterPlotCreator spc = CreateScatterPlot(value.datasetName).GetComponent<ScatterPlotCreator>();

                foreach (KeyValuePair<AttributeType, AttributeMappingObject> kvp in value.attributeMapping)
                {
                    if (kvp.Key == AttributeType.color && value.attributeOptions[kvp.Key].ContainsKey(AttributeOption.colorRange))
                    {
                        Gradient g = JsonUtility.FromJson<Gradient>((string)value.attributeOptions[kvp.Key][AttributeOption.colorRange]);
                        value.attributeOptions[kvp.Key][AttributeOption.colorRange] = g;
                    }

                    spc.ChangeAttribute(kvp.Key, kvp.Value.columnName, kvp.Value.isMeasure, value.attributeOptions[kvp.Key]);
                }

                spc.transform.position = JsonUtility.FromJson<Vector3>(value.position);
                spc.transform.rotation = JsonUtility.FromJson<Quaternion>(value.rotation);
                spc.transform.localScale = JsonUtility.FromJson<Vector3>(value.scale);
            }
        }
    }
}
