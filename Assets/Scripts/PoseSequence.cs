using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.XR.Hands;
using UnityEngine.XR.Hands.Gestures;
using TMPro;

public class XRHandPoseSequence : MonoBehaviour
{
    public XRHandSubsystem handSubsystem;
    public List<XRHandPose> targetPoseSequence;
    private Queue<XRHandPose> detectedPoses = new Queue<XRHandPose>();
    public float sequenceTimeout = 5f; // Time to complete the sequence
    private float lastPoseTime;

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
        lastPoseTime = Time.time;

        // Proper initialization of targetPoseSequence
        targetPoseSequence = new List<XRHandPose>
        {
            ScriptableObject.CreateInstance<XRHandPose>(),
            ScriptableObject.CreateInstance<XRHandPose>(),
            ScriptableObject.CreateInstance<XRHandPose>()
        };
    }

    void Update()
    {
        if (Time.time - lastPoseTime > sequenceTimeout)
        {
            detectedPoses.Clear();
        }
    }

    public void OnHandPoseRecognized(XRHandPose recognizedPose)
    {
        lastPoseTime = Time.time;
        detectedPoses.Enqueue(recognizedPose);

        if (detectedPoses.Count > targetPoseSequence.Count)
        {
            detectedPoses.Dequeue();
        }

        if (IsSequenceMatched())
        {
            string randomColour = colours[Random.Range(0, colours.Length)];
            Debug.Log("Pose sequence completed!");
            OnPoseSequenceCompleted?.Invoke();
            OnSequenceMatched?.Invoke("Pose sequence matched!");
            inputField.text = sequenceName + randomColour;
            detectedPoses.Clear();
        }
    }

    private bool IsSequenceMatched()
    {
        if (detectedPoses.Count != targetPoseSequence.Count)
            return false;

        XRHandPose[] detectedArray = detectedPoses.ToArray();
        for (int i = 0; i < targetPoseSequence.Count; i++)
        {
            if (!detectedArray[i].Equals(targetPoseSequence[i]))
                return false;
        }
        return true;
    }
}
