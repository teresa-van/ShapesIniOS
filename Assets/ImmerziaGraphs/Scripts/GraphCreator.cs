using GraphLibrary;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GraphCreator : MonoBehaviour
{
    public string DatasetName;
    public Graph plot;
    [HideInInspector]
    public Transform axisParent, tickParent;
    public List<GameObject> points = new List<GameObject>();
    public List<DataPoint> datapoints = new List<DataPoint>();

    public GameObject dataPointPrefab;
    public List<int> selectedIndicies;
}
