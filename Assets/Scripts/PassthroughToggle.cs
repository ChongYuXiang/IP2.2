//Author: Wang Johnathan Zhi Wen
//Filename: PassthroughToggle
//Description: MR camera


using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.OpenXR.Features.Meta;


/// <summary>
/// This class controls the toggling of the AR camera's passthrough mode.
/// It enables or disables the ARCameraManager component on the "Main Camera" object.
/// </summary>

public class PassthroughToggle : MonoBehaviour
{
    public GameObject cameraRig;

    void Start()
    {
        cameraRig = GameObject.Find("Main Camera");
        cameraRig.GetComponent<ARCameraManager>().enabled = false;
    }


    /// <summary>
    /// Initializes the camera rig and disables the ARCameraManager on startup.
    /// </summary>

    public void TogglePassthrough()
    {
        cameraRig.GetComponent<ARCameraManager>().enabled = !cameraRig.GetComponent<ARCameraManager>().enabled;
    }
}

