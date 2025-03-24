using System;
using UnityEngine;

public class PlayerCam : MonoBehaviour
{
    public float sensX;
    public float sensY;
    
    public Transform orientation;
    public GameObject player;
    public bool isUIOpened;

    private float _xRotation;
    private float _yRotation;

    private void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    private void Update()
    {
        if (isUIOpened)
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
        else
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
            MoveCamera();
        }
    }

    private void MoveCamera()
    {
        float mouseX = Input.GetAxis("Mouse X") * Time.deltaTime * sensX;
        float mouseY = Input.GetAxis("Mouse Y") * Time.deltaTime * sensY;
        
        _yRotation += mouseX;
        _xRotation -= mouseY;
        
        _xRotation = Mathf.Clamp(_xRotation, -90f, 90f);

        transform.rotation = Quaternion.Euler(_xRotation, _yRotation, 0);
        orientation.rotation = Quaternion.Euler(0f, _yRotation, 0f);
    }
}
