using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveManager : MonoBehaviour
{

    #region Attributes

    public float movementSpeed;

    public enum RotationAxes { MouseXAndY = 0, MouseX = 1, MouseY = 2 }
    public RotationAxes axes = RotationAxes.MouseXAndY;
    public float sensitivityX = 15F;
    public float sensitivityY = 15F;

    public float minimumX = -360F;
    public float maximumX = 360F;

    public float minimumY = -60F;
    public float maximumY = 60F;
    public float h = -180;

    float rotationY = 0F;

    #endregion

    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKey(KeyCode.W))
            MoveForward();

        if (Input.GetKey(KeyCode.S))
            MoveBackward();

        if (Input.GetKey(KeyCode.A))
            MoveLeft();

        if (Input.GetKey(KeyCode.D))
            MoveRight();

        if (Input.GetKey(KeyCode.E))
            MoveUp();

        if (Input.GetKey(KeyCode.Q))
            MoveDown();

        if (Input.GetMouseButton(1))
            RotateCamera();
    }

    #region Keyboard movement controls

    private void MoveDown()
    {
        Camera.main.transform.position += (Camera.main.transform.rotation * (Vector3.down * Time.deltaTime)) * movementSpeed;
    }

    private void MoveUp()
    {
        Camera.main.transform.position += (Camera.main.transform.rotation * (Vector3.up * Time.deltaTime)) * movementSpeed;
    }

    private void MoveRight()
    {
        Camera.main.transform.position += (Camera.main.transform.rotation * (Vector3.right * Time.deltaTime)) * movementSpeed;
    }

    private void MoveLeft()
    {
        Camera.main.transform.position += (Camera.main.transform.rotation * (Vector3.left * Time.deltaTime)) * movementSpeed;
    }

    private void MoveBackward()
    {
        Camera.main.transform.position += (Camera.main.transform.rotation * (Vector3.back * Time.deltaTime)) * movementSpeed;
    }

    private void MoveForward()
    {
        Camera.main.transform.position += (Camera.main.transform.rotation * (Vector3.forward * Time.deltaTime)) * movementSpeed;
    }

    #endregion

    #region Mouse movement controls

    private void RotateCamera()
    {
        if (axes == RotationAxes.MouseXAndY)
        {
            float rotationX = Camera.main.transform.localEulerAngles.y + Input.GetAxis("Mouse X") * sensitivityX;
            rotationY += Input.GetAxis("Mouse Y") * sensitivityY;
            rotationY = Mathf.Clamp(rotationY, minimumY, maximumY);
            Camera.main.transform.localEulerAngles = new Vector3(-rotationY, rotationX, 0);
        }
        else
        {
            rotationY += Input.GetAxis("Mouse Y") * sensitivityY;
            rotationY = Mathf.Clamp(rotationY, minimumY, maximumY);
            Camera.main.transform.localEulerAngles = new Vector3(-rotationY, Camera.main.transform.localEulerAngles.y, 0);
        }
    }

    #endregion    
}
