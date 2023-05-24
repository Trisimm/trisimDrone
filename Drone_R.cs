
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;
using UnityEngine.UI;


public class Drone_R : UdonSharpBehaviour
{
    public GameObject[] objectsToRotate;
    public float rotationSpeed;
    public Quaternion originalRotation;
    public Slider theSlider;
    private float myotherPrev;

    public Camera mainCamera;
    public Slider fovSlider;
    private float previousValue;
    void Start()
    {
        originalRotation = mainCamera.transform.rotation; // store the original rotation of the game object

        previousValue = fovSlider.value;
    }

    public void SetRotation(float value)
    {
            float rotationValue = value;
            mainCamera.transform.localRotation = Quaternion.Euler(rotationValue + 0f, 0f, 0f);
    }

    private void OnFOVSliderValueChanged(float value)
    {
        // Update the camera's field of view based on the slider's value
        mainCamera.fieldOfView = value;
    }

    void Update()
    {
        // Rotate the first two objects clockwise
        objectsToRotate[0].transform.Rotate(Vector3.up, rotationSpeed * Time.deltaTime);
        objectsToRotate[1].transform.Rotate(Vector3.up, rotationSpeed * Time.deltaTime);

        // Rotate the second two objects counter-clockwise
        objectsToRotate[2].transform.Rotate(Vector3.up, -rotationSpeed * Time.deltaTime);
        objectsToRotate[3].transform.Rotate(Vector3.up, -rotationSpeed * Time.deltaTime);

        if (previousValue != fovSlider.value)
        {
            OnFOVSliderValueChanged(fovSlider.value);
            previousValue = fovSlider.value;
        }

        if (myotherPrev != theSlider.value)
        {
            SetRotation(theSlider.value);
            myotherPrev = theSlider.value;
        }


    }
}
