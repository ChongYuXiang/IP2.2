/*
* Author: Sung Yeji
* Date: 02/04/2025
* Description: This script is to manage the Game of Number Flick
* */

using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class NumberFlickGameManager : MonoBehaviour
{
    public int NF_highscore;
    public int NF_correct;
    public int NF_wrong;
    public int NF_time_taken;
    public int NF_average_time_per_letter;

    private int currentNumber;  // The current random number prompt
    public TextMeshPro numberDisplay;  // UI text component that will display the number
    public TextMeshPro scoreDisplay;   // UI text component that will display the score
    public TextMeshPro feedbackDisplay; // For displaying feedback (correct/incorrect)
    public TMP_InputField inputDisplay;  // Input field for player's input
    private float startTime;  // Time when the letter was shown


    // Update is called once per frame
    void Update()
    {
        
    }

    void Start()
    {
        GenerateRandomNumber();  // Start with a random Number
        startTime = Time.time;   // Track the start time
        inputDisplay.onValueChanged.AddListener(delegate { CheckNumberInput(); }); // Listen for input changes
    }

    void GenerateRandomNumber()
    {
        // Generate a random number between 0 to 9
        currentNumber = Random.Range(0, 10); // 0 is inclusive, 10 is exclusive
        numberDisplay.text = currentNumber.ToString(); // Display the number
    }

    void CheckNumberInput()
    {
        if (inputDisplay.text.Length > 0) // Ensure there's input before checking
        {
            char enteredChar = inputDisplay.text[inputDisplay.text.Length - 1]; // Get the last entered character

            if (char.ToUpper(enteredChar) == currentNumber)  // Check if the player typed the correct letter
            {
                NF_correct++;  // Increase the correct count
                feedbackDisplay.text = "Correct!";
                GenerateRandomNumber();  // Generate a new random letter
                startTime = Time.time;  // Restart the timer
            }
            else
            {
                NF_wrong++;  // Increase the wrong count
                feedbackDisplay.text = "Incorrect. Try again!";
            }

            // Update the score display
            scoreDisplay.text = "Correct: " + NF_correct + "\nWrong: " + NF_wrong;
        }
    }

    // You can call this method to calculate the average time per letter after the game ends
    void CalculateAverageTime()
    {
        NF_time_taken = (int)(Time.time - startTime);
        NF_average_time_per_letter = NF_correct > 0 ? NF_time_taken / NF_correct : 0;
    }
}
