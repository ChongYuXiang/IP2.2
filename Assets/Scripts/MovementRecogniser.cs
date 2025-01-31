using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;
using UnityEngine.XR.Hands;
using UnityEngine.XR.Hands.Gestures;
using PDollarGestureRecognizer;
using System.IO;
using UnityEngine.Events;
using UnityEngine.XR.Management;

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

    public XRHandPose requiredPose;
    public float newPositionThresholdDistance = 0.01f;

    private List<Gesture> trainingSet = new List<Gesture>();
    private bool isMoving = false;
    private List<Vector3> positionsList = new List<Vector3>();

    void Start()
    {
        string[] gestureFiles = Directory.GetFiles(Application.persistentDataPath, "*.xml");

        foreach (var file in gestureFiles)
        {
            trainingSet.Add(GestureIO.ReadGestureFromFile(file));
        }
    }

    private void Update()
    {
        XRHandSubsystem handSubsystem = XRGeneralSettings.Instance?.Manager?.activeLoader?.GetLoadedSubsystem<XRHandSubsystem>();

        if (handSubsystem == null)
        {
            Debug.LogWarning("No XRHandSubsystem found!");
            return;
        }

        XRHand hand = (inputSource == XRNode.LeftHand) ? handSubsystem.leftHand : handSubsystem.rightHand;

        if (hand == null || !hand.isTracked)
        {
            Debug.LogWarning($"Hand is not tracked! Hand: {inputSource}");
            return;
        }

        var handJointsUpdatedEventArgs = new XRHandJointsUpdatedEventArgs { hand = hand };
        bool matchesPose = requiredPose?.CheckConditions(handJointsUpdatedEventArgs) ?? false;

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
        positionsList.Clear();
        positionsList.Add(movementSource.position);

        if (debugCubePrefab)
        {
            Instantiate(debugCubePrefab, movementSource.position, Quaternion.identity);
        }
    }

    private void UpdateMovement()
    {
        if (positionsList.Count == 0) return;

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

    private void EndMovement()
    {
        isMoving = false;

        if (positionsList.Count == 0) return;

        Point[] pointArray = new Point[positionsList.Count];
        for (int i = 0; i < positionsList.Count; i++)
        {
            pointArray[i] = new Point(positionsList[i].x, positionsList[i].y, 0);
        }

        Gesture newGesture = new Gesture(pointArray);

        if (creationMode)
        {
            newGesture.Name = newGestureName;
            trainingSet.Add(newGesture);
            string fileName = Application.persistentDataPath + "/" + newGestureName + ".xml";
            GestureIO.WriteGesture(pointArray, newGestureName, fileName);
        }
        else
        {
            Result result = PointCloudRecognizer.Classify(newGesture, trainingSet.ToArray());
            if (result.Score > recognitionThreshold)
            {
                OnGestureRecognised.Invoke(result.GestureClass);
            }
        }
    }
}