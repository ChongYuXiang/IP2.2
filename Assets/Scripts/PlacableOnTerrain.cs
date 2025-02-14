using UnityEngine;
using UnityEngine.XR;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Management;

public class PlacableOnTerrain : MonoBehaviour
{
    private Rigidbody rb;
    private XRGrabInteractable grabInteractable;

    [SerializeField] private LayerMask terrainLayer; // Ensure the terrain has the correct layer

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        grabInteractable = GetComponent<XRGrabInteractable>();

        if (grabInteractable == null)
        {
            grabInteractable = gameObject.AddComponent<XRGrabInteractable>();
        }

        // Ensure rigidbody settings for grab behavior
        rb.isKinematic = true;
        grabInteractable.selectEntered.AddListener(OnGrab);
        grabInteractable.selectExited.AddListener(OnRelease);
    }

    private void OnGrab(SelectEnterEventArgs args)
    {
        rb.isKinematic = true; // Prevent physics while grabbing
    }

    private void OnRelease(SelectExitEventArgs args)
    {
        rb.isKinematic = false; // Re-enable physics for a moment
        StartCoroutine(SnapToTerrain());
    }

    private System.Collections.IEnumerator SnapToTerrain()
    {
        yield return new WaitForSeconds(0.1f); // Small delay to allow physics resolution

        if (Physics.Raycast(transform.position + Vector3.up * 0.5f, Vector3.down, out RaycastHit hit, 2f, terrainLayer))
        {
            transform.position = new Vector3(transform.position.x, hit.point.y, transform.position.z);
            rb.isKinematic = true; // Lock in place
        }
    }
}
