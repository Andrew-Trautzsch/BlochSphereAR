using System.Collections;
using System.Collections.Generic;
// using UnityEditor.SceneManagement;
using UnityEngine;

public class CameraOrbit : MonoBehaviour
{

    public Transform objectToOrbit;
    public float radius;
    public float sensitivity;
    private bool dragActive;

    private float rotationX = 223;
    private float rotationY = 33;

    private Vector3 currentRotation;
    private Vector3 smoothVelocity = Vector3.zero;

    private float smoothTime = 0f;

    // Start is called before the first frame update
    void Start()
    {
        Screen.orientation = ScreenOrientation.LandscapeLeft;

        Vector3 nextRotation = new Vector3(rotationY, rotationX);
        currentRotation = Vector3.SmoothDamp(currentRotation, nextRotation, ref smoothVelocity, smoothTime);
        transform.localEulerAngles = currentRotation;
        smoothTime = 0.2f;
    }

    // Update is called once per frame
    void Update()
    {

        if (Input.GetMouseButtonDown(1))
        {
            dragActive = true;
        }

        if (Input.GetMouseButtonUp(1))
        {
            dragActive = false;
        }

        if (dragActive)
        {
            rotationX += Input.GetAxis("Mouse X") * sensitivity;
            rotationY += Input.GetAxis("Mouse Y") * sensitivity;
        }

        // To add zooming
        // radius -= Input.mouseScrollDelta.y;
        // transform.position = objectToOrbit.position - (transform.forward * radius);

        Vector3 nextRotation = new Vector3(rotationY, rotationX);
        currentRotation = Vector3.SmoothDamp(currentRotation, nextRotation, ref smoothVelocity, smoothTime);

        // this is the issue starting camera at origin, need to fix
        transform.localEulerAngles = currentRotation;
        // transform.localEulerAngles = new Vector3(rotationY, rotationX);

        transform.position = objectToOrbit.position - transform.forward * radius;

    }
}
