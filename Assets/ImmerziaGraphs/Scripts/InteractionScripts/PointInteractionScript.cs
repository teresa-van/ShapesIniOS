using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class PointInteractionScript : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    int uniqueID = -1;
    bool selected = false;
    Color originalColor { get; set; }
    Transform panel = null;

    // public void CreateInformationPanel()
    // {
    //     if (panel == null)
    //     {
    //         panel = Instantiate(Resources.Load("Prefabs/PointInformationPanel") as GameObject, this.gameObject.transform);
    //         //panel.transform.localPosition = new Vector3(0, 250 * panel.transform.localScale.y, 0);
    ////panel.GetComponent<Canvas>().rootCanvas.worldCamera = ViveInputModule.Instance.UICamera;
    //BroadcastMessage("PopulateInformationPanel", gameObject.name);

    //         Vector3 currentPos = this.transform.position;

    //         Transform xPlane = this.transform.parent.parent.Find("axisParent").Find("xPlane");
    //         CreateLineToPlane("xPlane", new Vector3[] { new Vector3(currentPos.x, currentPos.y, currentPos.z), new Vector3(currentPos.x, currentPos.y, xPlane.position.z) });

    //         Transform yPlane = this.transform.parent.parent.Find("axisParent").Find("yPlane");
    //         CreateLineToPlane("yPlane", new Vector3[] { new Vector3(currentPos.x, currentPos.y, currentPos.z), new Vector3(yPlane.position.x, currentPos.y, currentPos.z) });

    //         Transform zPlane = this.transform.parent.parent.Find("axisParent").Find("zPlane");
    //         CreateLineToPlane("zPlane", new Vector3[] { new Vector3(currentPos.x, currentPos.y, currentPos.z), new Vector3(currentPos.x, zPlane.position.y, currentPos.z) });

    //         this.transform.rotation = this.transform.root.rotation;
    //         panel.transform.localScale = new Vector3(panel.transform.localScale.x / transform.localScale.x, panel.transform.localScale.y / transform.localScale.y, panel.transform.localScale.z / transform.localScale.z);
    //     }
    // }

    public void OnPointerClick(PointerEventData eventData)
    {
        //if (!selected)
        //{
        //    selected = true;

        //    this.GetComponent<Renderer>().material.color = new Color(1, 1, 0);

        //    panel = Instantiate(Resources.Load("Prefabs/PointInformationPanel") as GameObject).transform;
        //    Vector3 scale = panel.lossyScale;
        //    scale.x /= this.transform.localScale.x;
        //    scale.y /= this.transform.localScale.y;
        //    scale.z /= this.transform.localScale.z;

        //    panel.SetParent(this.transform);
        //    panel.localPosition = new Vector3(0, 0, 0);
        //    panel.localScale = scale;
        //    //panel.GetComponent<Canvas>().rootCanvas.worldCamera = ViveInputModule.Instance.UICamera;
        //    BroadcastMessage("PopulateInformationPanel", gameObject.name);
        //}

        //else
        //{
        //    selected = false;

        //    this.GetComponent<Renderer>().material.color = originalColor;

        //    Destroy(panel);
        //    panel = null;
        //}
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        //uniqueID = LeanTween.color(this.gameObject, new Color(1, 1, 0), 1f).uniqueId;
        //this.transform.SetAsLastSibling();
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        //if (!selected)
        //{
        //    LeanTween.cancel(uniqueID);
        //    this.GetComponent<Renderer>().material.color = originalColor;
        //}
    }

    public void ChangeOriginalColor()
    {
        //originalColor = this.GetComponent<Renderer>().material.color;
    }

    //private void CreateLineToPlane(string planeName, Vector3[] positions)
    //{
    //    GameObject line = new GameObject(this.gameObject.name + planeName);
    //    line.transform.SetParent(this.gameObject.transform);
    //    LineRenderer lr = line.AddComponent<LineRenderer>();
    //    lr.useWorldSpace = false;
    //    lr.startWidth = 0.01f;
    //    lr.endWidth = 0.04f;

    //    lr.SetPositions(positions);
    //    lr.generateLightingData = true;
    //    Material whiteDiffuseMat = new Material(Shader.Find("Standard"));
    //    whiteDiffuseMat.color = Color.cyan;
    //    lr.material = whiteDiffuseMat;

    //    GameObject sprite = GameObject.Instantiate(Resources.Load("Prefabs/ShadowPoint") as GameObject);
    //    sprite.transform.SetParent(line.transform);
    //    sprite.transform.localPosition = positions[1];
    //    sprite.transform.localScale = this.gameObject.transform.localScale * 5;

    //    switch (planeName)
    //    {
    //        case "yPlane":
    //            sprite.transform.eulerAngles = new Vector3(0, 90, 0);
    //            break;
    //        case "zPlane":
    //            sprite.transform.eulerAngles = new Vector3(90, 0, 0);
    //            break;
    //    }

    //    SpriteRenderer sr = sprite.GetComponent<SpriteRenderer>();
    //    sr.color = this.gameObject.GetComponent<PointEventScript>().originalColor;
    //}
}
