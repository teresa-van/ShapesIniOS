using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GraphLibrary;
using System.IO;

public class PanelDemoSetupScript : MonoBehaviour
{

    // Use this for initialization
    void Start()
    {
        GameObject scatterPlot = GraphManager.CreateScatterPlot(Directory.GetCurrentDirectory() + "\\Assets\\Resources\\DataFiles\\download_prepared.csv");
        
        scatterPlot.transform.position = Camera.main.transform.position + Camera.main.transform.forward * 25;
        scatterPlot.transform.localScale = new Vector3(5, 5, 5);

        ScatterPlotCreator spc = scatterPlot.GetComponent<ScatterPlotCreator>();

        spc.ChangeAttribute(AttributeType.x, "CSD", false);
        spc.ChangeAttribute(AttributeType.y, "Period", false);
        spc.ChangeAttribute(AttributeType.z, "OriginalValue", true);
    }

    // Update is called once per frame
    void Update()
    {

    }
}
