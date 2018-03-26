using GraphLibrary;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GraphAxis : MonoBehaviour
{
    public AttributeType T { get; private set; }
    public List<GameObject> TickList1 = new List<GameObject>();
    List<GameObject> TickList2 = new List<GameObject>();
    public int Ticks { get; private set; }

    GameObject axisLabel;

    Vector3 scale1;
    Vector3 scale2;

    Vector3 pos1;
    Vector3 pos2;

    Vector3 free1;
    Vector3 free2;

    Vector3 offset;

    GameObject tickPrefab;

    int labelSize = 50;
    float thickness = 0.0005f;
    float lerpSpeed = 0.25f;

    Transform Follow;
    ScatterPlotCreator Plot;

    void Awake()
    {
        tickPrefab = Resources.Load<GameObject>("Prefabs/AxisTickPrefab");
        axisLabel = new GameObject();

        TextMesh tmLabel = axisLabel.AddComponent<TextMesh>();
        tmLabel.fontSize = 160;

        tmLabel.font = (Font)Resources.Load("Fonts/segoeuisl");
        axisLabel.GetComponent<MeshRenderer>().material = tmLabel.font.material;

        axisLabel.transform.localScale = new Vector3(0.008f, 0.008f, 1f);
        axisLabel.transform.parent = this.transform;
    }

    public void SetAttributeType(AttributeType t, Transform follow)
    {
        Plot = transform.root.GetComponent<ScatterPlotCreator>();
        T = t;
        Follow = follow;

        axisLabel.name = t.ToString() + " Axis Label";

        Vector3 scale = transform.localScale;

        switch (T)
        {
            case AttributeType.x:
                scale1 = new Vector3(thickness, scale.y, thickness);
                scale2 = new Vector3(thickness, thickness, scale.z);
                pos1 = new Vector3(0f, scale.y / 2f, 0f);
                pos2 = new Vector3(0f, 0f, -scale.z / 2f);
                free1 = Vector3.up;
                free2 = Vector3.forward;
                offset = Vector3.right;
                axisLabel.transform.localPosition = new Vector3(0f, 0.5f, 0f);
                break;

            case AttributeType.y:
                scale1 = new Vector3(thickness, thickness, scale.z);
                scale2 = new Vector3(scale.x, thickness, thickness);
                pos1 = new Vector3(0f, 0f, -scale.z / 2f);
                pos2 = new Vector3(scale.x / 2f, 0f, 0f);
                free1 = Vector3.forward;
                free2 = Vector3.right;
                offset = Vector3.up;
                axisLabel.transform.localPosition = new Vector3(0.5f, 0f, 0f);
                break;

            case AttributeType.z:
                scale1 = new Vector3(scale.x, thickness, thickness);
                scale2 = new Vector3(thickness, scale.y, thickness);
                pos1 = new Vector3(scale.x / 2f, 0f, 0f);
                pos2 = new Vector3(0f, scale.y / 2f, 0f);
                free1 = Vector3.right;
                free2 = Vector3.up;
                offset = -Vector3.forward;
                axisLabel.transform.localPosition = new Vector3(-0.5f, 0f, 0f);
                break;
        }
    }

    public void SetTickCount(int count)
    {
        if (count < 2)
            return;

        if (Ticks == 0)
        {
            AddMultiple(count);
            return;
        }

        while (Ticks < count)
            AddTick();

        while (Ticks > count)
            RemoveTick();
    }

    // REFACTOR THIS LOL (can condense the hell out of this)
    public void CheckTickLabels(bool x, bool y, bool z)
    {
        switch (T)
        {
            case AttributeType.x:
                // Check for x axis labels special alignment cases
                if ((x && !y && z))
                {
                    foreach (TextMesh tm in Plot.xPlane.GetComponentsInChildren<TextMesh>())
                    {
                        if (!tm.text.Contains(axisLabel.name.ToString()))
                        {
                            tm.transform.localPosition = new Vector3(pos1.x, -pos1.y, pos1.z) + offset;
                            tm.anchor = TextAnchor.LowerRight;
                            tm.fontSize = labelSize;
                        }
                    }
                }
                else if((x && !y && !z) || (x && y && !z) || (!x && y && !z))
                {
                    // place x labels on bottom of x axis case (clean 2d xy plot)
                    foreach (TextMesh tm in Plot.xPlane.GetComponentsInChildren<TextMesh>())
                    {
                        if (!tm.text.Contains(axisLabel.name.ToString()))
                        {
                            tm.transform.localPosition = new Vector3(pos1.x, -pos1.y, pos1.z) + offset;
                            tm.anchor = TextAnchor.UpperLeft;
                            tm.fontSize = labelSize;
                            axisLabel.transform.localPosition = new Vector3(0.5f, -0.1f, 0f);
                        }
                    }
                }
                else // just do normal x axis label alignment
                {
                    foreach (TextMesh tm in Plot.xPlane.GetComponentsInChildren<TextMesh>())
                    {
                        if (!tm.text.Contains(axisLabel.name.ToString()))
                        {
                            tm.transform.localPosition = pos1 + offset;
                            tm.anchor = TextAnchor.LowerLeft;
                            tm.fontSize = labelSize;
                            axisLabel.transform.localPosition = new Vector3(0.5f, 1.1f, 0f);
                        }
                    }
                }
                break;

            case AttributeType.y:
                // Check for y axis labels special alignment cases
                if ((x && y && !z) || (!x && y && !z))
                {
                    foreach (TextMesh tm in Plot.yPlane.GetComponentsInChildren<TextMesh>())
                    {
                        if (!tm.text.Contains(axisLabel.name.ToString()))
                        {
                            tm.transform.localPosition = new Vector3(pos1.x, pos1.y, -pos1.z) + offset;
                            tm.anchor = TextAnchor.LowerRight;
                            tm.fontSize = labelSize;
                            axisLabel.transform.localPosition = new Vector3(-0.2f, 0.5f, 0f);
                        }
                    }
                }
                else // just do normal y axis label alignment
                {
                    foreach (TextMesh tm in Plot.yPlane.GetComponentsInChildren<TextMesh>())
                    {
                        if (!tm.text.Contains(axisLabel.name.ToString()))
                        {
                            tm.transform.localPosition = pos1 + offset;
                            tm.anchor = TextAnchor.LowerLeft;
                            tm.fontSize = labelSize;
                            axisLabel.transform.localPosition = new Vector3(-0.2f, 0.5f, 1f);
                        }
                    }
                }
                break;

            case AttributeType.z:
                // Check for z axis labels special alignment cases
                if ((!x && y && z))
                {
                    foreach (TextMesh tm in Plot.zPlane.GetComponentsInChildren<TextMesh>())
                    {
                        if (!tm.text.Contains(axisLabel.name.ToString()))
                        {
                            tm.transform.localPosition = new Vector3(-pos1.x, pos1.y, pos1.z) + offset;
                            tm.anchor = TextAnchor.LowerRight;
                            tm.fontSize = labelSize; 
                        }
                    }
                }
                else // just do normal z axis label alignment
                {
                    foreach (TextMesh tm in Plot.zPlane.GetComponentsInChildren<TextMesh>())
                    {
                        if (!tm.text.Contains(axisLabel.name.ToString()))
                        {
                            tm.transform.localPosition = pos1 + offset;
                            tm.anchor = TextAnchor.LowerLeft;
                            tm.fontSize = labelSize;
                            //axisLabel.transform.localPosition = new Vector3(-0.3f, 0.5f, 1f);

                        }
                    }
                }
                break;
        }
    }

    void AddTick()
    {
        Ticks++;

        GameObject newTick1 = Instantiate(tickPrefab);
        GameObject newTick2 = Instantiate(tickPrefab);

        ClampScale s1 = newTick1.AddComponent<ClampScale>();
        ClampScale s2 = newTick2.AddComponent<ClampScale>();
        newTick1.transform.localScale = scale1;
        newTick2.transform.localScale = scale2;

        s1.Init(free1, thickness);
        s2.Init(free2, thickness);

        newTick1.transform.SetParent(transform, false);
        newTick2.transform.SetParent(Follow, false);

        TextMesh tm = newTick1.GetComponentInChildren<TextMesh>();
        //ClampScale t1 = tm.gameObject.AddComponent<ClampScale>();
        //t1.Init(Vector3.zero, 0.002f * 0.25f);
        tm.transform.localPosition = pos1 + offset;
        tm.text = "0.1234";
        tm.anchor = TextAnchor.LowerLeft;
        tm.fontSize = labelSize;

        TickList1.Insert(0, (newTick1));
        TickList2.Insert(0, (newTick2));

        float distance = (Ticks > 1) ? Ticks - 1f : Ticks;

        for (int i = 0; i < TickList1.Count; i++)
        {
            SetLabelText(TickList1[i], i, i / distance);
            if (i == 0)
            {
                TickList1[i].transform.localPosition = pos1;
                TickList2[i].transform.localPosition = pos2;
            }
            LeanTween.moveLocal(TickList1[i], pos1 + (offset * (i / distance)), lerpSpeed).setEase(LeanTweenType.easeInOutSine);
            LeanTween.moveLocal(TickList2[i], pos2 + (offset * (i / distance)), lerpSpeed).setEase(LeanTweenType.easeInOutSine);
        }
    }

    void AddMultiple(int num)
    {
        bool x = Plot.plot.attributeMapping.ContainsKey(AttributeType.x);
        bool y = Plot.plot.attributeMapping.ContainsKey(AttributeType.y);
        bool z = Plot.plot.attributeMapping.ContainsKey(AttributeType.z);

        for (int i = 0; i < num; i++)
        {
            Ticks++;

            GameObject newTick1 = Instantiate(tickPrefab);
            GameObject newTick2 = Instantiate(tickPrefab);

            ClampScale s1 = newTick1.AddComponent<ClampScale>();
            ClampScale s2 = newTick2.AddComponent<ClampScale>();
            newTick1.transform.localScale = scale1;
            newTick2.transform.localScale = scale2;

            s1.Init(free1, thickness);
            s2.Init(free2, thickness);

            newTick1.transform.SetParent(transform, false);
            newTick2.transform.SetParent(Follow, false);

            TextMesh tm = newTick1.GetComponentInChildren<TextMesh>();
            ClampScale t1 = tm.gameObject.AddComponent<ClampScale>();
            t1.Init(Vector3.zero, 0.002f);

            tm.transform.localPosition = pos1 + offset;
            tm.anchor = TextAnchor.LowerLeft;
            tm.fontSize = labelSize;

            TickList1.Add(newTick1);
            TickList2.Add(newTick2);
        }

        float distance = (Ticks > 1) ? Ticks - 1f : Ticks;

        for (int i = 0; i < TickList1.Count; i++)
        {
            SetLabelText(TickList1[i], i, i / distance);
            TickList1[i].transform.localPosition = pos1;
            TickList2[i].transform.localPosition = pos2;
            LeanTween.moveLocal(TickList1[i], pos1 + (offset * (i / distance)), lerpSpeed).setEase(LeanTweenType.easeInOutSine);
            LeanTween.moveLocal(TickList2[i], pos2 + (offset * (i / distance)), lerpSpeed).setEase(LeanTweenType.easeInOutSine);
        }

        axisLabel.GetComponent<TextMesh>().text = Plot.plot.attributeMapping[T].columnName;
    }

    void RemoveTick()
    {
        Ticks--;

        GameObject delete1 = TickList1[TickList1.Count - 2];
        GameObject delete2 = TickList2[TickList2.Count - 2];

        TickList1.Remove(delete1);
        TickList2.Remove(delete2);

        Destroy(delete1);
        Destroy(delete2);

        float distance = (Ticks > 1) ? Ticks - 1f : Ticks;

        for (int i = 0; i < TickList1.Count - 1; i++)
        {
            SetLabelText(TickList1[i], i, i / distance);
            LeanTween.moveLocal(TickList1[i], pos1 + (offset * (i / distance)), lerpSpeed).setEase(LeanTweenType.easeInOutSine);
            LeanTween.moveLocal(TickList2[i], pos2 + (offset * (i / distance)), lerpSpeed).setEase(LeanTweenType.easeInOutSine);
        }
    }

    public void ResetAxis()
    {
        Ticks = 0;

        for (int i = 0; i < TickList1.Count; i++)
        {
            GameObject delete1 = TickList1[i];
            GameObject delete2 = TickList2[i];

            LeanTween.moveLocal(delete1, pos1, lerpSpeed).setEase(LeanTweenType.easeInOutSine).setOnComplete(() =>
            {
                Destroy(delete1);
            });
            LeanTween.moveLocal(delete2, pos2, lerpSpeed).setEase(LeanTweenType.easeInOutSine).setOnComplete(() =>
            {
                Destroy(delete2);
            });
        }

        TickList1.Clear();
        TickList2.Clear();
        axisLabel.GetComponent<TextMesh>().text = "";
    }

    public void SetLabelText(GameObject tick, int index, float pos)
    {
        string text = "";

        string attribute = Plot.plot.attributeMapping[T].columnName;
        bool measure = Plot.plot.attributeMapping[T].isMeasure;

        if (measure)
        {
            float min = (float)Plot.plot.datasetObject.GetMinimumOfColumn(attribute);
            float max = (float)Plot.plot.datasetObject.GetMaximumOfColumn(attribute);

            float val = min + ((max - min) * pos);
            text += string.Format("{0:0.00}", val);
        }
        else
        {
            List<string> unique = Plot.plot.datasetObject.GetUniqueValuesForColumn(attribute);
            text += unique[index];
        }

        tick.GetComponentInChildren<TextMesh>().text = text;
    }

}

public class ClampScale : MonoBehaviour
{
    Vector3 Free;
    float ClampSize;

    bool initialized;

    Vector3 prevLossy = Vector3.one;

    public void Init(Vector3 free, float clampSize)
    {
        Free = free;
        ClampSize = clampSize;
        initialized = true;
    }

    void LateUpdate()
    {
        if (!initialized)
            return;

        if (prevLossy != transform.lossyScale)
        {
            Transform parent = transform.parent;

            transform.SetParent(null, true);
            transform.localScale = ((Vector3.one - Free) * ClampSize) + Vector3.Scale(Free, transform.localScale);
            transform.SetParent(parent, true);
        }

        prevLossy = transform.lossyScale;
    }

}
