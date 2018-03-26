using GraphLibrary;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class ScatterPlotCreator : GraphCreator
{
    public GameObject xPlane { get; private set; }
    public GameObject yPlane { get; private set; }
    public GameObject zPlane { get; private set; }

    public GraphAxis xAxis { get; private set; }
    public GraphAxis yAxis { get; private set; }
    public GraphAxis zAxis { get; private set; }


    float lerpSpeed = 0.25f;

    void Awake()
    {
        dataPointPrefab = Resources.Load<GameObject>("Prefabs/DataPointPrefab");
    }

    public void OnCreate(ScatterPlot s, string t)
    {
        tickParent = new GameObject("tickParent").transform;
        tickParent.SetParent(transform, false);
        CreateAxisPlanes(transform);
        GameObject pointParent = new GameObject("pointParent");
        pointParent.transform.SetParent(transform, false);

        // Generate title for scatterplot
        GameObject titleLabel = new GameObject("Title Label");
        TextMesh tm = titleLabel.AddComponent<TextMesh>();
        tm.text = t;
        tm.fontSize = 180;

        // Finally figured out how to programmatically change a textmesh font.
        tm.font = (Font)Resources.Load("Fonts/segoeuisl");
        titleLabel.GetComponent<MeshRenderer>().material = tm.font.material;

        titleLabel.transform.localPosition = new Vector3(0f, 1.5f, 0f);
        titleLabel.transform.localScale = new Vector3(0.008f, 0.008f, 1f);
        titleLabel.transform.parent = this.transform;

        plot = s;
        s.points.ForEach(p => PopulatePointGameObject(pointParent.transform, p, s.datasetObject));
        //SpriteManager.Instance.InitList(points);
    }

    public void ChangeAttribute(AttributeType t, string columnName, bool isMeasure, Dictionary<AttributeOption, object> limits = null)
    {
        //MapCreator m = GetComponent<MapCreator>();
        //if ((t == AttributeType.x || t == AttributeType.z) && m != null)
        //{
        //    m.mapPanel.ToggleMaps();
        //    m = null;
        //}

        if (columnName == "" || columnName == null)
            RemoveAttribute(t);
        else
            AddAttribute(t, columnName, isMeasure, limits);

        //if (m != null)
            //m.mapPanel.Rebuild();

        GraphManager.WriteToJSON();
    }

    public void RemoveAttribute(AttributeType t)
    {
        plot.RemoveAttributeFromGraph(t);
        ResetPlaneAndPoints(t);
    }

    #region Add Attribute To Graph
    public void AddAttribute(AttributeType t, string columnName, bool isMeasure, Dictionary<AttributeOption, object> limits = null)
    {
        plot.AddAttributeToPoints(t, columnName, isMeasure, limits);
        switch (t)
        {
            case AttributeType.x:
                ShowTicks(xPlane, false);
                break;
            case AttributeType.y:
                ShowTicks(yPlane, false);
                break;
            case AttributeType.z:
                ShowTicks(zPlane, false);
                break;
        }
        ChangePlanePosition(transform, t, isMeasure);
        plot.points.ForEach(i => ChangePointAttributeValue(t, i));
    }

    public void PopulatePointGameObject(Transform parent, int index, string nameOfPoint = null)
    {
        GameObject g = Instantiate(dataPointPrefab);

        if (nameOfPoint != null)
            g.name = nameOfPoint;
        else
            g.name = "point_" + index;

        g.transform.localScale = new Vector3(0.03f, 0.03f, 0.03f);
        g.transform.SetParent(parent, false);
        points.Add(g);

        DataPoint d = g.GetComponent<DataPoint>();
        datapoints.Add(d);
    }

    public void PopulatePointGameObject(Transform parent, Point point, Dataset dataset)
    {
        GameObject g = Instantiate(dataPointPrefab);

        g.name = "point_" + point.pointIndex;

        g.transform.localScale = Vector3.one * 0.03f;
        g.transform.SetParent(parent, false);
        points.Add(g);

        Dictionary<string, string> data = new Dictionary<string, string>();

        foreach (string column in dataset.columnNames)
            data.Add(column, dataset.ReturnPointValueByColumn(column, point.pointIndex));

        DataPoint d = g.GetComponent<DataPoint>();
        d.SetValues(point.pointAttributes).SetData(data);
        datapoints.Add(d);
    }

    // TODO: REFACTOR THIS
    public void ChangePointAttributeValue(AttributeType t, Point p)
    {
        points[p.pointIndex].SetActive(true);

        foreach (AttributeType obj in p.pointAttributes.Keys)
        {
            float pos;
            switch (t)
            {
                case AttributeType.x:
                    decimal val = (decimal)p.pointAttributes[t];

                    if (val == -1)
                    {
                        points[p.pointIndex].SetActive(false);
                        return;
                    }

                    pos = (float)val * axisParent.GetChild(0).localScale.x;
                    LeanTween.moveLocalX(points[p.pointIndex], pos, lerpSpeed).setEase(LeanTweenType.easeInOutSine);
                    break;

                case AttributeType.y:
                    val = (decimal)p.pointAttributes[t];

                    if (val == -1)
                    {
                        points[p.pointIndex].SetActive(false);
                        return;
                    }

                    pos = (float)val * axisParent.GetChild(1).localScale.y;

                    LeanTween.moveLocalY(points[p.pointIndex], pos, lerpSpeed).setEase(LeanTweenType.easeInOutSine);
                    break;

                case AttributeType.z:
                    val = (decimal)p.pointAttributes[t];

                    if (val == -1)
                    {
                        points[p.pointIndex].SetActive(false);
                        return;
                    }

                    pos = -(float)val * axisParent.GetChild(2).localScale.z;
                    LeanTween.moveLocalZ(points[p.pointIndex], pos, lerpSpeed).setEase(LeanTweenType.easeInOutSine);
                    break;

                case AttributeType.color:
                    if (!p.pointAttributes.ContainsKey(t))
                        break;

                    Color c = (Color)p.pointAttributes[t];
                    points[p.pointIndex].GetComponent<Renderer>().material.color = c;
                    break;

                case AttributeType.size:
                    val = (decimal)p.pointAttributes[t];
                    float factor = (float)val * (0.15f - 0.03f) + 0.03f;
                    points[p.pointIndex].transform.localScale = new Vector3(factor, factor, factor);
                    break;
            }
        }
    }

    #endregion

    #region Axis Plane Creation

    public void CreateAxisPlanes(Transform parent)
    {
        axisParent = new GameObject("axisParent").transform;

        xPlane = CreateXPlane();
        yPlane = CreateYPlane();
        zPlane = CreateZPlane();

        xPlane.transform.SetParent(axisParent, false);
        yPlane.transform.SetParent(axisParent, false);
        zPlane.transform.SetParent(axisParent, false);

        //xPlane.AddComponent<HandDraggable>().HostTransform = parent;
        //yPlane.AddComponent<HandDraggable>().HostTransform = parent;
        //zPlane.AddComponent<HandDraggable>().HostTransform = parent;

        axisParent.transform.SetParent(parent, false);

        xAxis = xPlane.AddComponent<GraphAxis>();
        yAxis = yPlane.AddComponent<GraphAxis>();
        zAxis = zPlane.AddComponent<GraphAxis>();

        xAxis.SetAttributeType(AttributeType.x, zPlane.transform);
        yAxis.SetAttributeType(AttributeType.y, xPlane.transform);
        zAxis.SetAttributeType(AttributeType.z, yPlane.transform);

        // Default to a 2D graph setup
        yPlane.GetComponent<MeshRenderer>().enabled = false;
        yPlane.GetComponent<BoxCollider>().enabled = false;

        zPlane.GetComponent<MeshRenderer>().enabled = false;
        zPlane.GetComponent<BoxCollider>().enabled = false;
    }

    private GameObject CreateXPlane()
    {
        List<Vector3> vertices = new List<Vector3>();
        vertices.Add(new Vector3(0, 0, 0));
        vertices.Add(new Vector3(0, 1, 0));
        vertices.Add(new Vector3(1, 0, 0));
        vertices.Add(new Vector3(1, 1, 0));

        return CreatePlane("xPlane", vertices, AttributeType.x);
    }

    private GameObject CreateYPlane()
    {
        List<Vector3> vertices = new List<Vector3>();
        vertices.Add(new Vector3(0, 0, 0));
        vertices.Add(new Vector3(0, 0, -1));
        vertices.Add(new Vector3(0, 1, 0));
        vertices.Add(new Vector3(0, 1, -1));

        return CreatePlane("yPlane", vertices, AttributeType.y);
    }

    private GameObject CreateZPlane()
    {
        List<Vector3> vertices = new List<Vector3>();
        vertices.Add(new Vector3(0, 0, 0));
        vertices.Add(new Vector3(1, 0, 0));
        vertices.Add(new Vector3(0, 0, -1));
        vertices.Add(new Vector3(1, 0, -1));

        return CreatePlane("zPlane", vertices, AttributeType.z);
    }

    private void ResetPlaneAndPoints(AttributeType t)
    {
        switch (t)
        {
            case AttributeType.x:
                ChangePlanePosition(transform, t, false);
                points.ForEach(p => LeanTween.moveLocalX(p, 0, lerpSpeed).setEase(LeanTweenType.easeInOutSine));
                xAxis.ResetAxis();
                break;

            case AttributeType.y:
                ChangePlanePosition(transform, t, false);
                points.ForEach(p => LeanTween.moveLocalY(p, 0, lerpSpeed).setEase(LeanTweenType.easeInOutSine));
                yAxis.ResetAxis();
                break;

            case AttributeType.z:
                ChangePlanePosition(transform, t, false);
                points.ForEach(p => LeanTween.moveLocalZ(p, 0, lerpSpeed).setEase(LeanTweenType.easeInOutSine));
                zAxis.ResetAxis();
                break;

            case AttributeType.color:
                points.ForEach(p => p.GetComponent<Renderer>().material.color = Color.white);
                break;
            case AttributeType.size:
                points.ForEach(p => p.transform.localScale = new Vector3(0.03f, 0.03f, 0.03f));
                break;
        }
    }

    private GameObject CreatePlane(string nameOfPlane, List<Vector3> verticesArg, AttributeType T)
    {
        GameObject g = new GameObject(nameOfPlane);

        g.AddComponent<MeshRenderer>();
        MeshFilter mf = g.AddComponent<MeshFilter>();

        Mesh m = new Mesh();

        m.vertices = verticesArg.ToArray();

        int[] triangles = new int[] { 0, 1, 2, 1, 3, 2, 2, 1, 0, 2, 3, 1 };
        m.triangles = triangles;

        mf.mesh = m;

        g.GetComponent<Renderer>().material = Resources.Load("Materials/PlaneMaterial") as Material;

        g.AddComponent<BoxCollider>();

        return g;
    }

    #endregion

    #region Change Attribute

    public void ChangePlanePosition(Transform plotParent, AttributeType t, bool isMeasure)
    {
        if (t == AttributeType.color || t == AttributeType.shape || t == AttributeType.size)
            return;

        decimal val = -1;

        if (isMeasure)
            val = plot.GetPercentageOfZero(plot.attributeMapping[t].columnName, t);

        int numberOfTicks = 0;
        if (plot.attributeMapping.ContainsKey(t))
        {
            numberOfTicks = (int)plot.attributeOptions[t][AttributeOption.numberOfTicks];
        }

        //float scaleValue = (numberOfTicks > 10) ? numberOfTicks - 1 : 1;
        float scaleValue = 1f;
        //if (numberOfTicks > 10)
        //	scaleValue = (1f / 10f) * (numberOfTicks - 1);


        switch (t)
        {
            case AttributeType.x:
                xPlane.transform.localScale = new Vector3(scaleValue, xPlane.transform.localScale.y, xPlane.transform.localScale.z);
                zPlane.transform.localScale = new Vector3(scaleValue, zPlane.transform.localScale.y, zPlane.transform.localScale.z);

                xAxis.SetTickCount(numberOfTicks);

                LeanTween.moveLocalX(yPlane, (val == -1) ? 0 : (float)val, lerpSpeed).setEase(LeanTweenType.easeInOutSine);
                break;

            case AttributeType.y:
                yPlane.transform.localScale = new Vector3(yPlane.transform.localScale.x, scaleValue, yPlane.transform.localScale.z);
                xPlane.transform.localScale = new Vector3(xPlane.transform.localScale.x, scaleValue, xPlane.transform.localScale.z);

                yAxis.SetTickCount(numberOfTicks);

                LeanTween.moveLocalY(zPlane, (val == -1) ? 0 : (float)val, lerpSpeed).setEase(LeanTweenType.easeInOutSine);
                break;

            case AttributeType.z:
                zPlane.transform.localScale = new Vector3(zPlane.transform.localScale.x, zPlane.transform.localScale.y, scaleValue);
                yPlane.transform.localScale = new Vector3(yPlane.transform.localScale.x, yPlane.transform.localScale.y, scaleValue);

                zAxis.SetTickCount(numberOfTicks);

                LeanTween.moveLocalZ(xPlane, (val == -1) ? 0 : -(float)val, lerpSpeed).setEase(LeanTweenType.easeInOutSine);
                break;
        }
        CheckPlanes();
    }

    #endregion

    /// <summary>
    /// Shows/hides the mesh renderers of children in the given object.
    /// Used with planes to show/hide their axis ticks.
    /// </summary>
    void ShowTicks(GameObject plane, bool state)
    {
        foreach (MeshRenderer r in plane.transform.GetComponentsInChildren<MeshRenderer>())
        {
            if (r.transform.parent == plane.transform && !r.gameObject.name.Contains("Axis Label"))
            {
                r.enabled = state;
            }
        }
    }

    /// <summary>
    /// Handles visibility of graph planes and adjusts tick labels.
    /// </summary>
    void CheckPlanes()
    {
        ShowTicks(xPlane, false);
        ShowTicks(yPlane, false);
        ShowTicks(zPlane, false);

        bool x = plot.attributeMapping.ContainsKey(AttributeType.x);
        bool y = plot.attributeMapping.ContainsKey(AttributeType.y);
        bool z = plot.attributeMapping.ContainsKey(AttributeType.z);

        // Show the xPlane in these cases
        if ((x && y && z) || (x && !y && !z) || (!x && !y && !z) || (x && y && !z) || (!x && y && !z))
        {
            xPlane.GetComponent<MeshRenderer>().enabled = true;
            xPlane.GetComponent<BoxCollider>().enabled = true;
            ShowTicks(xPlane, true);
            LeanTween.scaleY(xPlane, 1f, lerpSpeed).setEase(LeanTweenType.easeInOutSine);
        }
        else if (xPlane.GetComponent<MeshRenderer>().enabled)
        {
            LeanTween.scaleY(xPlane, 0f, lerpSpeed).setEase(LeanTweenType.easeInOutSine).setOnComplete(() =>
            {
                xPlane.GetComponent<MeshRenderer>().enabled = false;
                xPlane.GetComponent<BoxCollider>().enabled = false;
                ShowTicks(xPlane, false);
                xPlane.transform.localScale = new Vector3(xPlane.transform.localScale.x, 1f, xPlane.transform.localScale.z);
            });
        }

        // Show the yPlane in these cases
        if ((x && y && z) || (!x && y && z))
        {
            yPlane.GetComponent<MeshRenderer>().enabled = true;
            yPlane.GetComponent<BoxCollider>().enabled = true;
            ShowTicks(yPlane, true);
            LeanTween.scaleY(yPlane, 1f, lerpSpeed).setEase(LeanTweenType.easeInOutSine);
        }
        else if (yPlane.GetComponent<MeshRenderer>().enabled)
        {
            LeanTween.scaleY(yPlane, 0f, lerpSpeed).setEase(LeanTweenType.easeInOutSine).setOnComplete(() =>
            {
                yPlane.GetComponent<MeshRenderer>().enabled = false;
                yPlane.GetComponent<BoxCollider>().enabled = false;
                ShowTicks(yPlane, false);
                yPlane.transform.localScale = new Vector3(yPlane.transform.localScale.x, 1f, yPlane.transform.localScale.z);
            });
        }

        // Show the zPlane in these cases
        if ((x && y && z) || (!x && !y && z) || (x && !y && z))
        {
            zPlane.GetComponent<MeshRenderer>().enabled = true;
            zPlane.GetComponent<BoxCollider>().enabled = true;
            ShowTicks(zPlane, true);
            LeanTween.scaleZ(zPlane, 1f, lerpSpeed).setEase(LeanTweenType.easeInOutSine);
        }
        else if (zPlane.GetComponent<MeshRenderer>().enabled)
        {
            LeanTween.scaleZ(zPlane, 0f, lerpSpeed).setEase(LeanTweenType.easeInOutSine).setOnComplete(() =>
            {
                zPlane.GetComponent<MeshRenderer>().enabled = false;
                zPlane.GetComponent<BoxCollider>().enabled = false;
                ShowTicks(zPlane, false);
                zPlane.transform.localScale = new Vector3(zPlane.transform.localScale.x, zPlane.transform.localScale.y, 1f);
            });
        }

        if (x) xAxis.CheckTickLabels(x, y, z);
        if (y) yAxis.CheckTickLabels(x, y, z);
        if (z) zAxis.CheckTickLabels(x, y, z);
    }
}
