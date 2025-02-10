using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorLock : MonoBehaviour
{
    public bool isLocked = true;
    public float openAngle = 10f; // The angle considered "fully open"
    private HingeJoint hinge;
    private bool isTransitionTriggered = false;
    //public LevelLoader levelLoader;

    public GameObject warningUI;

    private void Awake()
    {
        hinge = GetComponent<HingeJoint>();
        UpdateLockState();
    }

    public void SetLockState(bool locked)
    {
        isLocked = locked;
        UpdateLockState();
    }

    private void UpdateLockState()
    {
        JointLimits limits = hinge.limits;
        if (isLocked)
        {
            limits.min = 0f;
            limits.max = 0f;
        }
        else
        {
            limits.min = 0f;  // Adjust as needed
            limits.max = openAngle; // Adjust as needed
        }
        hinge.limits = limits;
        hinge.useLimits = true;
    }

        private void Update()
    {
        if (!isLocked && !isTransitionTriggered && IsDoorFullyOpen())
        {
            isTransitionTriggered = true;
            StartSceneTransition();
        }
    }

        private bool IsDoorFullyOpen()
    {
        // Check if the door's current rotation has reached the open angle
        float currentAngle = hinge.angle;
        return Mathf.Abs(currentAngle - openAngle) < 1f; // Allow for slight imprecision
    }

    private void StartSceneTransition()
    {
        Debug.Log("Door fully opened. Starting scene transition...");

        //if (levelLoader != null)
        //{
        //    levelLoader.LoadNextLevel();
        //}
        //else
        //{
        //    Debug.LogError("SceneLoader is not assigned or found in the scene.");
        //}
    }

    public void onHoverEnter_DoorLocked()
    {
        if(isLocked == true)
        {
            warningUI.SetActive(true);
        }
    }

    public void onHoverExit()
    {
        warningUI.SetActive(false);
    }
}


