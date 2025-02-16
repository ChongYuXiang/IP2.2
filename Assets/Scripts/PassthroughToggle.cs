using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.OpenXR.Features.Meta;

public class PassthroughToggle : MonoBehaviour
{
    public GameObject cameraRig;

    void Start()
    {
        cameraRig = GameObject.Find("Main Camera");
        cameraRig.GetComponent<ARCameraManager>().enabled = false;
    }

    public void TogglePassthrough()
    {
        cameraRig.GetComponent<ARCameraManager>().enabled = !cameraRig.GetComponent<ARCameraManager>().enabled;
    }
}

