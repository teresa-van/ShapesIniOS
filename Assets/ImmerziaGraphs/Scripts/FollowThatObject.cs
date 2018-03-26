using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowThatObject : MonoBehaviour
{
	[Tooltip("The object that we're following!")]
	public Transform Follow;

	[Tooltip("The offset from the followed object, from the cameras perspective.")]
	public Vector3 Offset;

	Transform cam;
	public GameObject parent { get; private set; }

	void Awake()
	{
		cam = Camera.main.transform;
		parent = new GameObject("ToolbarParent");
	}

	void Update()
	{
		if (Follow != null)
		{
			transform.localPosition = Offset;

			parent.transform.position = Follow.position;
            //parent.transform.forward = (parent.transform.position - cam.position).normalized;
            parent.transform.forward = Follow.transform.forward;
            parent.transform.rotation = Follow.transform.rotation;


            //Vector3 fromFollowToCam = (cam.position - Follow.position).normalized;
            //Vector3 fromThisToCam = (cam.position - transform.position).normalized;

            //Vector3 local = cam.TransformVector(Offset);

            //Debug.DrawLine(Follow.position, Follow.position + local, Color.red, 5f);

            //Vector3 pos = Follow.position + local;
            //transform.position = pos;
        }
	}

	public void IGotYourObjectRightHerePal(Transform follow, Vector3 offset)
	{
		Follow = follow;
		Offset = offset;

		parent.transform.position = follow.position;
        //parent.transform.forward = (parent.transform.position - cam.position).normalized;
        parent.transform.forward = follow.transform.forward;


        transform.SetParent(parent.transform);


		transform.localPosition = Offset;
	}
}
