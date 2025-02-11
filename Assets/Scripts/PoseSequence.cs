using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.XR;
using UnityEngine.XR.Hands;
using UnityEngine.XR.Hands.Gestures;
using UnityEngine.XR.Management;
using TMPro;

public class XRHandPoseSequence : MonoBehaviour
{
    public XRNode inputSource; // Specify LeftHand or RightHand
    public XRHandSubsystem handSubsystem;
    public List<XRHandPose> targetPoseSequence;
    private Queue<XRHandPose> detectedPoses = new Queue<XRHandPose>();
    private XRHandPose lastDetectedPose; // Variable to track the last detected pose

    [System.Serializable]
    public class UnityStringEvent : UnityEvent<string> { }
    public UnityStringEvent OnSequenceMatched;

    public delegate void PoseSequenceCompleted();
    public static event PoseSequenceCompleted OnPoseSequenceCompleted;

    public TMP_InputField inputField;
    public Transform spawnPosition; // Position to spawn the prefab

    private string[] colours = { "red", "blue", "green" };
    public string sequenceName = "HDB_";

    void Start()
    {
        // Initialize the class-level handSubsystem
        handSubsystem = XRGeneralSettings.Instance?.Manager?.activeLoader?.GetLoadedSubsystem<XRHandSubsystem>();

        if (handSubsystem == null)
        {
            Debug.LogError("Hand Subsystem is not initialized!");
            return;
        }
        else
        {
            Debug.Log("Hand Subsystem initialized successfully.");
        }

        // Initialize the target pose sequence
        targetPoseSequence = new List<XRHandPose>
        {
            ScriptableObject.CreateInstance<XRHandPose>(),
            ScriptableObject.CreateInstance<XRHandPose>(),
            ScriptableObject.CreateInstance<XRHandPose>()
        };

        lastDetectedPose = null; // Initially, no pose has been detected
    }


    void Update()
    {
        if (handSubsystem == null)
        {
            Debug.LogError("Hand Subsystem is not initialized!");
            return;
        }
        if (targetPoseSequence == null || targetPoseSequence.Count == 0)
        {
            Debug.LogError("Target Pose Sequence is not initialized!");
            return;
        }

        XRHand hand = (inputSource == XRNode.LeftHand) ? handSubsystem.leftHand : handSubsystem.rightHand;
        var handJointsUpdatedEventArgs = new XRHandJointsUpdatedEventArgs { hand = hand };
        bool matchesPose = targetPoseSequence[0]?.CheckConditions(handJointsUpdatedEventArgs) ?? false;

        if (matchesPose)
        {
            OnHandPoseRecognized(targetPoseSequence[0]);
        }
    }

    public void OnHandPoseRecognized(XRHandPose recognizedPose)
    {

        // Skip if the same pose was detected consecutively
        if (recognizedPose.Equals(lastDetectedPose))
        {
            return; // Do not add this pose to the detected poses queue
        }

        detectedPoses.Enqueue(recognizedPose);
        lastDetectedPose = recognizedPose; // Update the last detected pose

        Debug.Log("Detected pose: " + recognizedPose.name);

        // Ensure that detectedPoses only holds the most recent poses up to the size of targetPoseSequence
        if (detectedPoses.Count > targetPoseSequence.Count)
        {
            detectedPoses.Dequeue();
        }

        // Check if the sequence is matched
        if (IsSequenceMatched())
        {
            string randomColour = colours[Random.Range(0, colours.Length)];
            Debug.Log("Pose sequence completed!");
            OnPoseSequenceCompleted?.Invoke();
            OnSequenceMatched?.Invoke("Pose sequence matched!");
            inputField.text = sequenceName + randomColour;

            // Clear the detected poses once the sequence is matched
            detectedPoses.Clear();
        }
    }

    private bool IsSequenceMatched()
    {
        // If the number of detected poses doesn't match the target sequence, return false
        if (detectedPoses.Count != targetPoseSequence.Count)
        {
            Debug.LogWarning("Pose count mismatch! Detected: " + detectedPoses.Count + " Target: " + targetPoseSequence.Count);
            return false;
        }

        // Convert detected poses to an array for easier comparison
        XRHandPose[] detectedArray = detectedPoses.ToArray();

        // Check if each detected pose matches the corresponding pose in the target sequence
        for (int i = 0; i < targetPoseSequence.Count; i++)
        {
            if (!detectedArray[i].Equals(targetPoseSequence[i]))
            {
                Debug.LogWarning("Pose mismatch at index " + i + ": Detected " + detectedArray[i].name + " vs Target " + targetPoseSequence[i].name);
                return false;
            }        
        }
        return true;
    }
}
