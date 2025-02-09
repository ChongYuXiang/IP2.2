/* Author: Wang Johnathan Zhiwen  
* Filename: MovementRecogniser
* Descriptions: Function for Gesture movement recogniser
*/

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
using TMPro;

public class MovementRecogniser : MonoBehaviour
{
    public XRNode inputSource; // Specify LeftHand or RightHand
    public Transform movementSource;
    public GameObject debugCubePrefab; // For debugging purposes
    public TMP_InputField inputDisplay;
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
        string[] gestureFiles = Resources.Load("Gestures").ToString().Split(new char[] { '\n', '\r' }, System.StringSplitOptions.RemoveEmptyEntries);
        Debug.Log("Gesture files found: " + string.Join(", ", gestureFiles));

        foreach (var file in gestureFiles)
        {
            trainingSet.Add(GestureIO.ReadGestureFromFile(file));
        }
    }

    private void Update()
    {
        XRHandSubsystem handSubsystem = XRGeneralSettings.Instance?.Manager?.activeLoader?.GetLoadedSubsystem<XRHandSubsystem>();


        XRHand hand = (inputSource == XRNode.LeftHand) ? handSubsystem.leftHand : handSubsystem.rightHand;

        var handJointsUpdatedEventArgs = new XRHandJointsUpdatedEventArgs { hand = hand };
        bool matchesPose = requiredPose?.CheckConditions(handJointsUpdatedEventArgs) ?? false;


        if (matchesPose && !isMoving)
        {
            Debug.Log("Starting movement...");
            StartMovement();
        }
        else if (!matchesPose && isMoving)
        {
            Debug.Log("Ending movement...");
            EndMovement();
        }
        else if (matchesPose && isMoving)
        {
            Debug.Log("Updating movement...");
            UpdateMovement();
        }
    }

    void StartMovement()
    {
        isMoving = true;
        positionsList.Clear();
        positionsList.Add(movementSource.position);
        
        // Debug the initial position of movementSource
        Debug.Log($"Initial movementSource position: {movementSource.position}");

        if (debugCubePrefab)
        {
            Instantiate(debugCubePrefab, movementSource.position, Quaternion.identity);
        }
    }

    private void UpdateMovement()
    {
        if (positionsList.Count == 0) return;

        Vector3 lastPosition = positionsList[positionsList.Count - 1];
        float distance = Vector3.Distance(movementSource.position, lastPosition);
        Debug.Log($"Distance: {distance} (Threshold: {newPositionThresholdDistance})");

        // Check if movementSource is updating
        Debug.Log($"Current movementSource position: {movementSource.position}");

        if (distance > newPositionThresholdDistance)
        {
            positionsList.Add(movementSource.position);
            Debug.Log($"New position added: {movementSource.position}");

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
            Debug.Log($"Saving new gesture: {newGestureName}");
            newGesture.Name = newGestureName;
            trainingSet.Add(newGesture);
            string fileName = Application.persistentDataPath + "/" + newGestureName + ".xml";
            GestureIO.WriteGesture(pointArray, newGestureName, fileName);
        }
        else
        {
            Result result = PointCloudRecognizer.Classify(newGesture, trainingSet.ToArray());
            Debug.Log($"Classification result: Score = {result.Score}, Gesture = {result.GestureClass}");
            if (result.Score > recognitionThreshold)
            {
                OnGestureRecognised.Invoke(result.GestureClass);
            }
            else
            {
                Debug.LogWarning("Gesture recognition score below threshold.");
            }
        }
    }

    public void RecogniseGesture(string gestureName)
    {
        inputDisplay.text = gestureName;
    }
}
