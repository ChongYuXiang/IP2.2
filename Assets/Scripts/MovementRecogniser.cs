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
    public float newPositionThresholdDistance = 0.01f; // Reduced threshold to detect smaller movements

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
            handSubsystem.updatedHands -= OnHandsUpdated;
        }
    }

    private void OnHandsUpdated(XRHandSubsystem handSubsystem, XRHandSubsystem.UpdateSuccessFlags successFlags, XRHandSubsystem.UpdateType updateType)
    {
        Debug.Log("OnHandsUpdated called");

        if (requiredPose == null)
        {
            Debug.LogWarning("No requiredPose assigned!");
            return;
        }

        XRHand hand = (inputSource == XRNode.LeftHand) ? handSubsystem.leftHand : handSubsystem.rightHand;

        if (hand == null || !hand.isTracked)
        {
            Debug.LogWarning($"Hand is not tracked! Hand: {inputSource}");
            return;
        }

        Debug.Log($"Hand {inputSource} is tracked.");

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

    void StartMovement()
    {
        isMoving = true;
        Debug.Log("Start Moving");
        positionsList.Clear();
        positionsList.Add(movementSource.position);

        Debug.Log($"Starting Position: {movementSource.position}");

        if (debugCubePrefab)
        {
            Instantiate(debugCubePrefab, movementSource.position, Quaternion.identity);
        }
    }

    private void UpdateMovement()
    {
        Debug.Log("Update Movement");

        if (positionsList.Count == 0)
        {
            Debug.LogWarning("positionsList is empty, can't calculate distance!");
            return;
        }

        Vector3 lastPosition = positionsList[positionsList.Count - 1];
        float distance = Vector3.Distance(movementSource.position, lastPosition);

        Debug.Log($"Last Position: {lastPosition}, Current Position: {movementSource.position}, Movement Distance: {distance}");

        if (distance > newPositionThresholdDistance)
        {
            positionsList.Add(movementSource.position);
            Debug.Log($"New Position Added: {movementSource.position}");

            if (debugCubePrefab)
            {
                Instantiate(debugCubePrefab, movementSource.position, Quaternion.identity);
            }
        }
        else
        {
            Debug.Log($"Movement too small: {distance} (Threshold: {newPositionThresholdDistance})");
        }
    }

    private void EndMovement()
    {
        isMoving = false;
        Debug.Log("End Moving");

        if (positionsList.Count == 0)
        {
            Debug.LogWarning("No movement data recorded!");
            return;
        }

        // Create Gesture From Position
        Point[] pointArray = new Point[positionsList.Count];
        for (int i = 0; i < positionsList.Count; i++)
        {
            Vector2 screenPosition = Camera.main.WorldToScreenPoint(positionsList[i]);
            pointArray[i] = new Point(positionsList[i].x, positionsList[i].y, 0);
        }

        Gesture newGesture = new Gesture(pointArray);

        if (creationMode)
        {
            newGesture.Name = newGestureName;
            trainingSet.Add(newGesture);

            string fileName = Application.persistentDataPath + "/" + newGestureName + ".xml";
            GestureIO.WriteGesture(pointArray, newGestureName, fileName);
            Debug.Log($"Gesture Saved: {fileName}");
        }
        else
        {
            Result result = PointCloudRecognizer.Classify(newGesture, trainingSet.ToArray());
            Debug.Log($"Gesture Recognised: {result.GestureClass}, Score: {result.Score}");

            if (result.Score > recognitionThreshold)
            {
                OnGestureRecognised.Invoke(result.GestureClass);
            }
        }
    }
}
