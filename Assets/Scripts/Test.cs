using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;
using UnityEngine.XR.Interaction.Toolkit;

public class Test : MonoBehaviour
{
    public XRNode inputSource;
    public InputHelpers.Button inputButton;
    public float inputThreshold = 0.1f;
    public Transform movementSource;

    public float newPositionThresholdDistance = 0.05f;
    public GameObject debugCubePrefab;

    private bool isMoving = false;
    private List<Vector3> positionsList = new List<Vector3>();


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        InputHelpers.IsPressed(InputDevices.GetDeviceAtXRNode(inputSource), inputButton, out bool isPressed, inputThreshold);

        //Start Movement
        if (isPressed && !isMoving)
        {
            StartMovement();
        }
        //End Movement
        else if (!isPressed && isMoving)
        {
            EndMovement();
        }
        else if (isPressed && isMoving)
        {
            UpdateMovement();
        }
    }

    private void StartMovement()
    {
        isMoving = true;
        Debug.Log("Start Moving");
        positionsList.Clear();
        positionsList.Add(movementSource.position);
        if(debugCubePrefab){
            
            Instantiate(debugCubePrefab, movementSource.position, Quaternion.identity);
        }
    }

    private void EndMovement()
    {
        isMoving = false;
        Debug.Log("End moving");
    }

    private void UpdateMovement()
    {
        Debug.Log("Update");
        Vector3 lastPosition = positionsList[positionsList.Count - 1];
        if(Vector3.Distance(movementSource.position,lastPosition) > newPositionThresholdDistance)
        {
            positionsList.Add(movementSource.position);
            if(debugCubePrefab){
                Instantiate(debugCubePrefab, movementSource.position, Quaternion.identity);
            }
        }
    }
}