using GraphLibrary;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

/// <summary>

/// </summary>
public class MapCreator : MonoBehaviour
{
    #region Properties

    public Map MapPlot;
    GameObject map;
    public List<OnlineMapsMarker3D> MapMarkers;
    public bool hasY;

    // Map translation properites
    private Vector3 beginTranslateHitPos;
    private float mapTranslateSpeed = 0.1f;

    //public MapPanel mapPanel { get; private set; }

    #endregion

    #region Methods

    void Start()
    {
        //Create the map
        GameObject mapPrefab = (GameObject)Resources.Load("Maps/MapPrefab", typeof(GameObject));
        map = Instantiate(mapPrefab, transform);
        map.name = map.name.Replace("(Clone)", "");
        Dataset d = GetComponent<ScatterPlotCreator>().plot.datasetObject;
        ScatterPlot p = (ScatterPlot)GetComponent<ScatterPlotCreator>().plot;

        //p.attributeOptions.Add(AttributeType.x, new Dictionary<AttributeOption, object>());
        //p.attributeOptions.Add(AttributeType.z, new Dictionary<AttributeOption, object>());

        //if(!p.attributeOptions[AttributeType.x].ContainsKey(AttributeOption.min))
        //    p.attributeOptions[AttributeType.x].Add(AttributeOption.min,-90f);

        //if (!p.attributeOptions[AttributeType.x].ContainsKey(AttributeOption.max))
        //    p.attributeOptions[AttributeType.x].Add(AttributeOption.max,90f);

        //if (!p.attributeOptions[AttributeType.z].ContainsKey(AttributeOption.min))
        //    p.attributeOptions[AttributeType.z].Add(AttributeOption.min,-180f);

        //if (!p.attributeOptions[AttributeType.z].ContainsKey(AttributeOption.max))
        //    p.attributeOptions[AttributeType.z].Add(AttributeOption.max,180f);

        MapPlot = new Map(d, p);

        //Change the position to match that of the x plane
        GameObject xPlane = transform.Find("axisParent").Find("xPlane").gameObject;
        map.transform.localPosition = xPlane.transform.localPosition;

        if (MapPlot.attributeMapping.ContainsKey(AttributeType.y))
        {
            hasY = true;
            map.GetComponent<OnlineMapsTileSetControl>().useElevation = true;
        }
        else
        {
            hasY = false;
            map.GetComponent<OnlineMapsTileSetControl>().useElevation = false;
        }
        map.GetComponent<OnlineMapsTileSetControl>().useElevation = hasY;

        //Change the position of the map the be under the scatter plot
        Vector3 temp;
        if (CheckMapDrop())
            temp = new Vector3(-1, 0.5f, 0f);
        // Otherwise place on bottom plane
        else
        {
            temp = new Vector3(-1, 0f, 0f);
            ShowZPlaneLines(false);
        }
        map.transform.localPosition -= temp;

        //Change the position to center around the inital data
        //TODO
        //decimal startingLat = ((decimal)MapPlot.attributeOptions[AttributeType.latitude][AttributeOption.max] - (decimal)MapPlot.attributeOptions[AttributeType.latitude][AttributeOption.min]) /2;
        //float startingLong = ((float)MapPlot.GetMaximumValueOfAxis(AttributeType.longitude) - (float)MapPlot.GetMinimumValueOfAxis(AttributeType.longitude))/2;
        map.GetComponent<OnlineMaps>().SetPositionAndZoom(-111.357939121313f, 49.0372461727351f, 9);

        DisplayMarkers();
    }

    /// <summary>
    /// Displays markers on map
    /// </summary>
    private void DisplayMarkers()
    {
        // Hide scatterplot points
        transform.Find("pointParent").gameObject.SetActive(false);

        //initiate prefab and list
        GameObject mapMarkerPrefab = (GameObject)Resources.Load("Maps/MapMarker", typeof(GameObject));
        MapMarkers = new List<OnlineMapsMarker3D>();

        //initate markers onto the map
        foreach (Point p in MapPlot.points)
        {
            //Create the marker
            //TODO check if lat and long is valid
            decimal latitude = Convert.ToDecimal(p.pointAttributes[AttributeType.latitude]);
            decimal longitude = Convert.ToDecimal(p.pointAttributes[AttributeType.longitude]);
            OnlineMapsMarker3D marker;

            // TODO: REFACTOR THIS 
            // If there's a Y attribute on the scatterplot, use the drop lines for the map marker (showing how high up the point is on the map)
            if (hasY)
            {
                //Set the original scatter plot point to the marker prefab
                GameObject mapMarkerWithDropline = Instantiate(mapMarkerPrefab);
                GameObject mapPoint = Instantiate(GetComponent<ScatterPlotCreator>().points[p.pointIndex]);     // COPY THE POINT, NOT FUCKING OVERWRITE IT HOLY FUCK
                mapPoint.name = GetComponent<ScatterPlotCreator>().points[p.pointIndex].name;
                mapPoint.transform.SetParent(mapMarkerWithDropline.transform);
                mapPoint.transform.position = Vector3.zero;
                marker = map.GetComponent<OnlineMapsControlBase3D>().AddMarker3D(new Vector2((float)longitude, (float)latitude), mapMarkerWithDropline);

                //Add point index as a property
                marker.instance.GetComponent<MarkerProperties>().PointIndex = p.pointIndex;

                Destroy(marker.instance.GetComponent<BoxCollider>());
                Destroy(mapMarkerWithDropline);
            }
            // If there is no Y attribute, just put the points on the surface of the map without droplines
            else
            {
                // Set the original scatter plot point to the marker prefab
                GameObject mapMarker = new GameObject();
                GameObject mapPoint = Instantiate(GetComponent<ScatterPlotCreator>().points[p.pointIndex]);
                mapPoint.name = GetComponent<ScatterPlotCreator>().points[p.pointIndex].name;
                mapPoint.transform.SetParent(mapMarker.transform);
                mapPoint.transform.position = Vector3.zero;
                marker = map.GetComponent<OnlineMapsControlBase3D>().AddMarker3D(new Vector2((float)longitude, (float)latitude), mapMarker);

                //Add point index as a property
                MarkerProperties pointProperties = marker.instance.AddComponent<MarkerProperties>();
                pointProperties.PointIndex = p.pointIndex;

                Destroy(marker.instance.GetComponent<BoxCollider>());
                Destroy(mapMarker);
            }

            marker.transform.gameObject.name = marker.transform.gameObject.name.Replace("(Clone)", "");

            //Add reference to marker
            MapMarkers.Add(marker);
        }

        // Initial update for markers
        if (hasY)
        {
            foreach (OnlineMapsMarker3D marker in MapMarkers)
            {
                if (!marker.transform.gameObject.activeSelf)
                    continue;

                //Update the line renderer
                UpdateMarkerLineRenderer(marker);
            }
        }

        //StartCoroutine(UpdateMarkers(1));
        ////Set update events
        //Map.GetComponent<OnlineMaps>().OnChangePosition += () =>
        //{
        //    StartCoroutine(UpdateMarkers());
        //};

        //Map.GetComponent<OnlineMaps>().OnChangeZoom += () =>
        //{
        //    StartCoroutine(UpdateMarkers());
        //};
    }


    /// <summary>
    /// Updates the line render of the marker
    /// </summary>
    private void UpdateMarkerLineRenderer(OnlineMapsMarker3D marker)
    {
        //Find the x plane lvl position
        //float distance = Mathf.Abs(marker.transform.Find("Cube").position.y - this.gameObject.transform.Find("axisParent").Find("xPlane").gameObject.transform.position.y) * Mathf.Pow(2, 20 - Map.GetComponent<OnlineMaps>().zoom);
        float distance = Mathf.Abs(marker.transform.Find("DropLine").position.y - this.gameObject.transform.Find("axisParent").Find("xPlane").gameObject.transform.position.y);

        Vector3 xPlaneLevelPosition = new Vector3(0, distance, 0);

        //Find max y value
        decimal maxYValue = (decimal)MapPlot.attributeOptions[AttributeType.y][AttributeOption.max];

        //Find this marker's y value
        float markerY = float.Parse(MapPlot.datasetObject.ReturnPointValueByColumn(MapPlot.attributeMapping[AttributeType.y].columnName, marker.transform.GetComponent<MarkerProperties>().PointIndex));

        //Find the y value position 
        float heightYPlane = this.transform.Find("axisParent").Find("yPlane").gameObject.transform.localScale.y;
        Vector3 yLevelPosition = xPlaneLevelPosition + new Vector3(0, heightYPlane * (float)markerY / (float)maxYValue, 0);

        //Updating the line rederer
        marker.transform.gameObject.GetComponentInChildren<LineRenderer>().SetPositions(new Vector3[] { Vector3.zero, xPlaneLevelPosition, yLevelPosition });

        //Update location of the sphere
        //marker.transform.Find("Sphere").localPosition = yLevelPosition + Vector3.up * (float)(marker.transform.Find("Sphere").localScale.y / 2);
        marker.transform.Find("point_" + marker.transform.GetComponent<MarkerProperties>().PointIndex).localPosition = yLevelPosition + Vector3.up * (float)(marker.transform.Find("Point").localScale.y / 2);
    }

    /// <summary>
    /// Check if map should be rendered underneath the current graph
    /// </summary>
    bool CheckMapDrop()
    {
        //Checks if map elvation is on and y is mapped
        if (map.GetComponent<OnlineMapsTileSetControl>().useElevation && hasY)
        {
            return true;
        }
        else
            return false;
    }


    /// <summary>
    /// Shows or hides z plane and lines
    /// </summary>
    void ShowZPlaneLines(bool visible)
    {
        Transform mapParent = map.transform.parent;

        // Hide actual Z plane
        mapParent.transform.Find("axisParent").Find("zPlane").gameObject.SetActive(visible);

        //GameObject xTickParent = mapParent.transform.Find("tickParent").Find("xTickParent").gameObject;
        //GameObject zTickParent = mapParent.transform.Find("tickParent").Find("zTickParent").gameObject;

        //// Iterate through children of xTickParent and zTickParent
        //int index = 0;
        //foreach (Transform t in xTickParent.GetComponentInChildren<Transform>())
        //{
        //    // Find correct tick lines on bottom plane (z)
        //    if (t.name.Contains("x_zPlane"))
        //    {
        //        // Don't hide inside border lines 
        //        if (t.name.Contains("0"))
        //        {
        //            index++;
        //            continue;
        //        }

        //        t.Find("line_" + index).gameObject.SetActive(visible);
        //        index++;
        //    }
        //}

        //index = 0;
        //foreach (Transform t in zTickParent.GetComponentInChildren<Transform>())
        //{
        //    if (t.name.Contains("z_zPlane"))
        //    {
        //        if (t.name.Contains("0"))
        //        {
        //            index++;
        //            continue;
        //        }

        //        t.Find("line_" + index).gameObject.SetActive(visible);
        //        index++;
        //    }
        //}
    }

    // Simply gives this MapCreator a reference to the MapPanel that owns it.
    //internal void SetMapPanel(MapPanel mapPanel)
    //{
    //    this.mapPanel = mapPanel;
    //}

    //// Makes Z plane lines visible and destroys the map.
    //internal void RemoveMap()
    //{
    //    ShowZPlaneLines(true);
    //    transform.Find("pointParent").gameObject.SetActive(true);
    //    foreach (OnlineMapsMarker3D marker in MapMarkers)
    //    {
    //        Destroy(marker.transform.gameObject);
    //    }
    //    Destroy(map);
    //    Destroy(this);
    //}

    #endregion
}
