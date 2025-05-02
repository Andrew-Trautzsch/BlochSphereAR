using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Unity.VisualScripting;
using UnityEngine.Analytics;

public class RotationInput : MonoBehaviour
{
    public Slider xSlider, ySlider, zSlider;
    public TMP_InputField rotation, azimuthal, polar;
    public GameObject objectToRotate, sphereObject;
    private float rotationLimit = 360f;
    public GameObject gimbalX, gimbalY, gimbalZ, lineObject, gimbalRings, polarPoints;
    private float rotationSpeed = 600f;
    private bool sliderUpdate = false;
    private Vector3 lastSphereRotation;
    private string customAxis = "horizontal";

    public void Start()
    {
        customAxis = "horizontal";

        lastSphereRotation = sphereObject.transform.eulerAngles;

        polar.text = 90.ToString();
        azimuthal.text = 0.ToString();
        rotation.text = 90.ToString();

        objectToRotate.transform.eulerAngles = new Vector3
        (
            SaveVars.xAngle,
            SaveVars.yAngle,
            SaveVars.zAngle
        );

        xSlider.value = SaveVars.xSlider;
        ySlider.value = SaveVars.ySlider;
        zSlider.value = SaveVars.zSlider;
    }

    //
    //////////////////////// needs to stay unique
    //

    public void UpdateGimbalX()
    {
        gimbalX.transform.eulerAngles = new Vector3
        (
            0 + sphereObject.transform.eulerAngles.x - 90,
            SaveVars.yAngle + 90 + sphereObject.transform.eulerAngles.y,
            0
        );
    }
    public void UpdateGimbalY()
    {
        gimbalY.transform.eulerAngles = new Vector3
        (
            SaveVars.xAngle + sphereObject.transform.eulerAngles.x - 90,
            SaveVars.yAngle + sphereObject.transform.eulerAngles.y,
            0
        );
    }
    public void UpdateGimbalZ()
    {
        gimbalZ.transform.eulerAngles = new Vector3
        (
            -90 + sphereObject.transform.eulerAngles.x - 90,
            SaveVars.yAngle + sphereObject.transform.eulerAngles.y,
            0
        );
    }

    //
    ///////////////////
    //

    public void lineCheck()
    {
        if (SaveVars.lineCheck) SaveVars.lineCheck = false;
        else SaveVars.lineCheck = true;
    }

    public void gimbalCheck()
    {
        if (SaveVars.gimbalCheck) SaveVars.gimbalCheck = false;
        else SaveVars.gimbalCheck = true;
    }

    public void polarCheck()
    {
        if (SaveVars.polarCheck) SaveVars.polarCheck = false;
        else SaveVars.polarCheck = true;
    }

    private void updateVisual()
    {
        if (SaveVars.lineCheck) lineObject.SetActive(true);
        else lineObject.SetActive(false);

        if (SaveVars.gimbalCheck) gimbalRings.SetActive(true);
        else gimbalRings.SetActive(false);

        if (SaveVars.polarCheck) polarPoints.SetActive(true);
        else polarPoints.SetActive(false);
    }

    //
    ///////////////////
    //

    public void SliderRotate()
    {
        if (!sliderUpdate)
        {
            SaveVars.xSlider = xSlider.value * rotationLimit;
            SaveVars.ySlider = ySlider.value * rotationLimit;
            SaveVars.zSlider = zSlider.value * rotationLimit;

            SaveVars.xAngle = xSlider.value * rotationLimit;
            SaveVars.yAngle = ySlider.value * rotationLimit;
            SaveVars.zAngle = zSlider.value * rotationLimit;
        }
        else sliderUpdate = false;
    }

    public void ChangeAxis(string input = "horizontal")
    {
        customAxis = input;
    }

    public void ApplyAngle()
    {
        SaveVars.rotationAngle = int.Parse(rotation.text);
        SaveVars.polarAngle = int.Parse(polar.text);
        SaveVars.azimuthalAngle = int.Parse(azimuthal.text);

        if (customAxis == "horizontal")
        {
            SaveVars.xAngle += SaveVars.rotationAngle;
            SaveVars.yAngle = SaveVars.azimuthalAngle;
            SaveVars.zAngle = 90 - SaveVars.polarAngle;
        }
        if (customAxis == "vertical")
        {
            SaveVars.xAngle = 90 - SaveVars.polarAngle;
            SaveVars.yAngle += SaveVars.rotationAngle;
        }

    }

    // This is going to be inefficient and need to fix later
    // button implementation
    public void ButtonRotateX(int input)
    {
        SaveVars.xAngle += input;
    }

    public void ButtonRotateY(int input)
    {
        SaveVars.yAngle += input;
    }

    public void ButtonRotateZ(int input)
    {
        SaveVars.zAngle += input;
    }

    // Update is called once per frame
    void Update()
    {
        UpdateGimbalX();
        UpdateGimbalY();
        UpdateGimbalZ();

        updateVisual();

        SaveVars.xAngle %= rotationLimit;
        SaveVars.yAngle %= rotationLimit;
        SaveVars.zAngle %= rotationLimit;

        // Calculate the target rotation
        Quaternion targetRotation = Quaternion.Euler
            (
                SaveVars.xAngle + sphereObject.transform.eulerAngles.x - 90,
                SaveVars.yAngle + sphereObject.transform.eulerAngles.y,
                SaveVars.zAngle // + sphereObject.transform.eulerAngles.z
            );

        // Rotate gradually towards the target rotation
        objectToRotate.transform.rotation = Quaternion.RotateTowards(objectToRotate.transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
    }
}
