using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;
using UnityEngine.XR.Hands;
using UnityEngine.XR.Hands.Gestures;
using PDollarGestureRecognizer;
using System.IO;
using UnityEngine.Events;

public class MovementRecogniser : MonoBehaviour
{
    public XRNode inputSource; // Specify LeftHand or RightHand
    public Transform movementSource;
    public GameObject debugCubePrefab;
    public bool creationMode = false;
    public string newGestureName;
    
    public float recognitionThreshold = 0.9f;

    [System.Serializable]
    public class UnityStringEvent : UnityEvent<string> { }
    public UnityStringEvent OnGestureRecognised;

    public XRHandPose requiredPose; // Reference to the XRHandPose asset
    public float newPositionThresholdDistance = 0.05f;

    private List<Gesture> trainingSet = new List<Gesture>();
    private XRHandSubsystem handSubsystem; // Hand tracking subsystem
    private bool isMoving = false;
    private List<Vector3> positionsList = new List<Vector3>();

    void Start()
    {
        string[] gestureFiles = Directory.GetFiles(Application.persistentDataPath, "*.xml");

        foreach (var file in gestureFiles)
        {
            trainingSet.Add(GestureIO.ReadGestureFromFile(file));
        }

        // Initialize the XRHandSubsystem
        var subsystems = new List<XRHandSubsystem>();
        SubsystemManager.GetInstances(subsystems);
        if (subsystems.Count > 0)
        {
            handSubsystem = subsystems[0];
            // Subscribe to the updatedHands event
            handSubsystem.updatedHands += OnHandsUpdated;
        }
        else
        {
            Debug.LogWarning("No XRHandSubsystem found!");
        }
    }

    void OnDestroy()
    {
        if (handSubsystem != null)
        {
            // Unsubscribe from the updatedHands event to avoid memory leaks
            handSubsystem.updatedHands -= OnHandsUpdated;
        }
    }

    // Correct method signature to match the event delegate
    private void OnHandsUpdated(XRHandSubsystem handSubsystem, XRHandSubsystem.UpdateSuccessFlags successFlags, XRHandSubsystem.UpdateType updateType)
    {
        if (requiredPose == null)
            return;

        // Retrieve the correct hand data (left or right) from the handSubsystem
        XRHand hand = (inputSource == XRNode.LeftHand) ? handSubsystem.leftHand : handSubsystem.rightHand;

        // Ensure the hand is tracked
        if (hand == null || !hand.isTracked)
            return;

        // Now use the hand directly to check conditions
        var handJointsUpdatedEventArgs = new XRHandJointsUpdatedEventArgs();
        handJointsUpdatedEventArgs.hand = hand;
        bool matchesPose = requiredPose.CheckConditions(handJointsUpdatedEventArgs);

        if (matchesPose && !isMoving)
        {
            StartMovement();
        }
        else if (!matchesPose && isMoving)
        {
            EndMovement();
        }
        else if (matchesPose && isMoving)
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

        if (debugCubePrefab)
        {
            Instantiate(debugCubePrefab, movementSource.position, Quaternion.identity);
        }
    }

    private void EndMovement()
    {
        isMoving = false;
        Debug.Log("End moving");

        //Create Gesture From Position
        Point[] pointArray = new Point[positionsList.Count];
        for (int i = 0; i < positionsList.Count; i++)
        {
            Vector2 screenPosition = Camera.main.WorldToScreenPoint(positionsList[i]);
            pointArray[i] = new Point(positionsList[i].x, positionsList[i].y, 0);
        }

        Gesture newGesture = new Gesture(pointArray);

        //Add New Gesture to Training Set
        if (creationMode)
        {
            newGesture.Name = newGestureName;
            trainingSet.Add(newGesture);

            //Save Training Set
            string fileName = Application.persistentDataPath + "/" + newGestureName + ".xml";
            GestureIO.WriteGesture(pointArray, newGestureName, fileName);
        }
        //Recognise Gesture
        else
        {
            Result result = PointCloudRecognizer.Classify(newGesture, trainingSet.ToArray());
            Debug.Log("Gesture Recognised: " + result.GestureClass + " " + result.Score);
            if (result.Score > recognitionThreshold)
            {
                OnGestureRecognised.Invoke(result.GestureClass);
            }
        }
    }

    private void UpdateMovement()
    {
        Debug.Log("Update Movement");
        Vector3 lastPosition = positionsList[positionsList.Count - 1];
        if (Vector3.Distance(movementSource.position, lastPosition) > newPositionThresholdDistance)
        {
            positionsList.Add(movementSource.position);

            if (debugCubePrefab)
            {
                Instantiate(debugCubePrefab, movementSource.position, Quaternion.identity);
            }
        }
    }
}
