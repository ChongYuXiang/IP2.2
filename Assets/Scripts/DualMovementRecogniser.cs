/* Author: Wang Johnathan Zhiwen  
* Filename: DualMovementRecogniser
* Descriptions: Function for two handed Gesture movement recogniser
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

public class DualMovementRecognizer : MonoBehaviour
{
    public Transform leftHandSource;
    public Transform rightHandSource;
    public GameObject debugCubePrefab;
    public TMP_InputField inputDisplay;
    public bool creationMode = false;
    public string newGestureName;
    public float recognitionThreshold = 0.9f;
    
    [System.Serializable]
    public class UnityStringEvent : UnityEvent<string> { }
    public UnityStringEvent OnGestureRecognised;

    public XRHandPose leftHandPose;
    public XRHandPose rightHandPose;
    public float newPositionThresholdDistance = 0.01f;

    private List<Gesture> trainingSet = new List<Gesture>();
    private bool isMoving = false;
    private List<Vector3> leftPositionsList = new List<Vector3>();
    private List<Vector3> rightPositionsList = new List<Vector3>();

    void Start()
    {
        string[] gestureFiles = Directory.GetFiles(Application.persistentDataPath, "*.xml");
        Debug.Log("Gesture files found: " + string.Join(", ", gestureFiles));

        foreach (var file in gestureFiles)
        {
            trainingSet.Add(GestureIO.ReadGestureFromFile(file));
        }
    }

    private void Update()
    {
        XRHandSubsystem handSubsystem = XRGeneralSettings.Instance?.Manager?.activeLoader?.GetLoadedSubsystem<XRHandSubsystem>();
        if (handSubsystem == null) return;

        CheckHandMovement(handSubsystem.leftHand, handSubsystem.rightHand);
    }

    private void CheckHandMovement(XRHand leftHand, XRHand rightHand)
    {
        var leftHandJointsUpdatedEventArgs = new XRHandJointsUpdatedEventArgs { hand = leftHand };
        var rightHandJointsUpdatedEventArgs = new XRHandJointsUpdatedEventArgs { hand = rightHand };

        bool leftMatchesPose = leftHandPose?.CheckConditions(leftHandJointsUpdatedEventArgs) ?? false;
        bool rightMatchesPose = rightHandPose?.CheckConditions(rightHandJointsUpdatedEventArgs) ?? false;

        if (leftMatchesPose && rightMatchesPose && !isMoving)
        {
            StartMovement();
        }
        else if ((!leftMatchesPose || !rightMatchesPose) && isMoving)
        {
            EndMovement();
        }
        else if (leftMatchesPose && rightMatchesPose && isMoving)
        {
            UpdateMovement();
        }
    }

    private void StartMovement()
    {
        isMoving = true;
        leftPositionsList.Clear();
        rightPositionsList.Clear();
        leftPositionsList.Add(leftHandSource.position);
        rightPositionsList.Add(rightHandSource.position);
        
        if (debugCubePrefab)
        {
            Instantiate(debugCubePrefab, leftHandSource.position, Quaternion.identity);
            Instantiate(debugCubePrefab, rightHandSource.position, Quaternion.identity);
        }
    }

    private void UpdateMovement()
    {
        if (leftPositionsList.Count == 0 || rightPositionsList.Count == 0) return;

        Vector3 lastLeftPosition = leftPositionsList[leftPositionsList.Count - 1];
        Vector3 lastRightPosition = rightPositionsList[rightPositionsList.Count - 1];

        if (Vector3.Distance(leftHandSource.position, lastLeftPosition) > newPositionThresholdDistance)
        {
            leftPositionsList.Add(leftHandSource.position);
            if (debugCubePrefab)
            {
                Instantiate(debugCubePrefab, leftHandSource.position, Quaternion.identity);
            }
        }

        if (Vector3.Distance(rightHandSource.position, lastRightPosition) > newPositionThresholdDistance)
        {
            rightPositionsList.Add(rightHandSource.position);
            if (debugCubePrefab)
            {
                Instantiate(debugCubePrefab, rightHandSource.position, Quaternion.identity);
            }
        }
    }

    private void EndMovement()
    {
        isMoving = false;
        if (leftPositionsList.Count == 0 || rightPositionsList.Count == 0) return;

        List<Point> combinedPoints = new List<Point>();
        for (int i = 0; i < leftPositionsList.Count; i++)
        {
            combinedPoints.Add(new Point(leftPositionsList[i].x, leftPositionsList[i].y, 0));
        }
        for (int i = 0; i < rightPositionsList.Count; i++)
        {
            combinedPoints.Add(new Point(rightPositionsList[i].x, rightPositionsList[i].y, 1)); // Different stroke ID
        }

        Gesture newGesture = new Gesture(combinedPoints.ToArray());
        if (creationMode)
        {
            Debug.Log($"Saving new gesture: {newGestureName}");
            newGesture.Name = newGestureName;
            trainingSet.Add(newGesture);
            string fileName = Application.persistentDataPath + "/" + newGestureName + ".xml";
            GestureIO.WriteGesture(combinedPoints.ToArray(), newGestureName, fileName);
        }
        else
        {
            Result result = PointCloudRecognizer.Classify(newGesture, trainingSet.ToArray());
            Debug.Log($"Classification result: Score = {result.Score}, Gesture = {result.GestureClass}");

            if (result.Score > recognitionThreshold)
            {
                OnGestureRecognised.Invoke(result.GestureClass);
            }
        }
    }

    public void RecogniseGesture(string gestureName)
    {
        inputDisplay.text = gestureName;
    }
}
