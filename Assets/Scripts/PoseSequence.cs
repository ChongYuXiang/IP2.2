using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using TMPro;
using System;

public class PoseSequence : MonoBehaviour
{
    // Your input field and sequence name
    public TMP_InputField inputField;
    public TMP_InputField finalInputField;
    public string[] validSequenceWords;
    public string sequenceName;

    // Color array
    private string[] colours = { "Red", "Green", "Blue" };

    // Current word index the player needs to input
    private int currentWordIndex = 0;

    // To track whether the current word has been correctly entered
    private bool wordEntered = false;

    // Events
    public static event Action OnPoseSequenceCompleted;
    public static event Action<string> OnSequenceMatched;

    // Start is called before the first frame update
    void Start()
    {
        // Automatically subscribe to the input field's onValueChanged event
        inputField.onValueChanged.AddListener(OnInputValueChanged);
    }

    // This method will be called when the input field value changes
    private void OnInputValueChanged(string text)
    {
        // Only check the sequence if the word hasn't been entered yet and the text is not empty
        if (!wordEntered && !string.IsNullOrEmpty(text))
        {
            CheckSequence(text);
        }
    }

    // This method checks if the sequence matches
    private void CheckSequence(string inputText)
    {
        // If the current input matches the expected word
        if (inputText == validSequenceWords[currentWordIndex])
        {
            Debug.Log($"Word {currentWordIndex + 1} in sequence matched!");

            // Move to the next word in the sequence
            currentWordIndex++;

            // Check if the entire sequence is completed
            if (currentWordIndex >= validSequenceWords.Length)
            {
                CompleteSequence();
            }
            
            // Mark this word as entered
            wordEntered = false; // Allow checking for the next word
        }
        else
        {
            // Sequence mismatch - give feedback and reset the sequence
            Debug.Log("Incorrect word. Sequence reset.");
            ResetSequence();
        }
    }

    // This method is called when the entire sequence is matched
    private void CompleteSequence()
    {
        // Pick a random color from the array
        string randomColour = colours[UnityEngine.Random.Range(0, colours.Length)];
        // Update the input field with the sequence name + random color
        finalInputField.text = sequenceName + randomColour;
        Debug.Log(sequenceName + randomColour);
        currentWordIndex = 0;

        // Log and invoke events
        Debug.Log("Pose sequence completed!");
        OnPoseSequenceCompleted?.Invoke();
        OnSequenceMatched?.Invoke("Pose sequence matched!");

        // Update the input field with the sequence name + random color
        inputField.text = sequenceName + randomColour;
        Debug.Log($"Sequence name: {sequenceName} {randomColour}");

        // Complete the sequence and reset for the next round
        ResetSequence();
    }

    // Resets the sequence if the player inputs the wrong word
    private void ResetSequence()
    {
        // Clear the input fields
        inputField.text = string.Empty;
        finalInputField.text = string.Empty;

        // Reset the index to 0 to start over the sequence
        currentWordIndex = 0;

        // Reset wordEntered flag to allow the sequence to start over
        wordEntered = false;

        Debug.Log("Sequence reset. Try again.");
    }
}
