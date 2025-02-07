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
    public int NF_score;
    public int NF_wrong;
    public int NF_time_taken;
    public int NF_average_time_per_letter;

    private int currentNumber;  // The current random number prompt
    public TextMeshPro numberDisplay;  // UI text component that will display the number
    public TextMeshPro scoreDisplay;   // UI text component that will display the score
    public TextMeshPro feedbackDisplay; // For displaying feedback (correct/incorrect)
    public TMP_InputField inputDisplay;  // Input field for player's input
    public TextMeshPro timerDisplay; // UI text component to display the timer

    private float timeRemaining = 60f;  // Start timer with 60 seconds
    private bool isGameOver = false;


    // Update is called once per frame
    void Update()
    {
        if (!isGameOver)
        {
            // Update the timer and display it
            timeRemaining -= Time.deltaTime;

            // Display remaining time (round to 0 decimals)
            timerDisplay.text = "Time: " + Mathf.Ceil(timeRemaining).ToString() + "s";

            // Check if time is up
            if (timeRemaining <= 0f)
            {
                isGameOver = true;  // End the game
                timeRemaining = 0f;
                EndGame();
            }
        }
        
    }

    void Start()
    {
        GenerateRandomNumber();  // Start with a random Number
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
        if (inputDisplay.text.Length > 0 && !isGameOver) // Ensure there's input before checking
        {
            char enteredChar = inputDisplay.text[inputDisplay.text.Length - 1]; // Get the last entered character

            if (char.ToUpper(enteredChar) == currentNumber)  // Check if the player typed the correct letter
            {
                NF_score += 10;

                feedbackDisplay.text = "Correct!";
                GenerateRandomNumber();  // Generate a new random letter
            }
            else
            {
                NF_wrong++;  // Increase the wrong count
                feedbackDisplay.text = "Incorrect. Try again!";
            }

            // Update the score display
            scoreDisplay.text = "Final Score: " + NF_highscore;
        }
    }

    void EndGame()
    {
        // Show the final score or a message when the game ends
        feedbackDisplay.text = "Time's up! Game over!";
        scoreDisplay.text = "Final Score: " + NF_highscore;
        if (NF_score > NF_highscore)
        {
            NF_highscore = NF_score;
        }
    }

}
