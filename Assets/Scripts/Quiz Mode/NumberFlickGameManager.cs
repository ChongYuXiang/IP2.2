/*
* Author: Sung Yeji
* Date: 02/04/2025
* Description: This script is to manage the Game of Number Flick
* */

using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SocialPlatforms.Impl;
using UnityEngine.SceneManagement;


public class NumberFlickGameManager : MonoBehaviour
{
    public int NF_score;


    private int currentNumber;  // The current random number prompt
    public TextMeshPro numberDisplay;  // UI text component that will display the number
    public TextMeshPro scoreDisplay;   // UI text component that will display the score
    public TextMeshPro feedbackDisplay; // For displaying feedback (correct/incorrect)
    public TMP_InputField inputDisplay;  // Input field for player's input
    public TextMeshPro timerDisplay; // UI text component to display the timer
    public GameObject gameOverPanel;  // Panel to display when the game ends

    private float timeRemaining = 60f;  // Start timer with 60 seconds
    private bool isGameOver = false;

    public GameObject leftHand;
    public GameObject rightHand;
    public Material defaultMat;
    public Material correctMat;
    public Material wrongMat;

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
                StartCoroutine("DisplayHandsCorrect");
                GenerateRandomNumber();  // Generate a new random letter
            }
            else
            {
                StartCoroutine("DisplayHandsWrong");
                feedbackDisplay.text = "Incorrect. Try again!";
            }

            // Update the score display
            scoreDisplay.text = "Final Score: " + NF_score;
        }
    }

        IEnumerator DisplayHandsCorrect() // Change hands to a green material
    {
        leftHand.GetComponent<SkinnedMeshRenderer>().materials[1] = correctMat;
        rightHand.GetComponent<SkinnedMeshRenderer>().materials[1] = correctMat;
        yield return new WaitForSeconds(1.5f);
        leftHand.GetComponent<SkinnedMeshRenderer>().materials[1] = defaultMat;
        rightHand.GetComponent<SkinnedMeshRenderer>().materials[1] = defaultMat;
    }

    IEnumerator DisplayHandsWrong() // Change hands to a red material
    {
        leftHand.GetComponent<SkinnedMeshRenderer>().materials[1] = wrongMat;
        rightHand.GetComponent<SkinnedMeshRenderer>().materials[1] = wrongMat;
        yield return new WaitForSeconds(1.5f);
        leftHand.GetComponent<SkinnedMeshRenderer>().materials[1] = defaultMat;
        rightHand.GetComponent<SkinnedMeshRenderer>().materials[1] = defaultMat;
    }

    void EndGame()
    {
        isGameOver = true;  // Prevent further input
        gameOverPanel.SetActive(true);  // Show the game over panel
        // Show the final score or a message when the game ends
        feedbackDisplay.text = "Time's up! Game over!";
        scoreDisplay.text = "Final Score: " + NF_score;

        // Find and tell database to create word game data
        GameObject database;
        database = GameObject.Find("Database");
        database.GetComponent<Database>().WriteNumberGameData(NF_score);
    }

    public void RestartGame()
    {
        NF_score = 0;  // Reset the score
        timeRemaining = 60f;  // Reset the timer
        isGameOver = false;  // Allow input
        gameOverPanel.SetActive(false);  // Hide the game over panel
        GenerateRandomNumber();  // Start a new game
        scoreDisplay.text = "Score: " + NF_score;  // Reset the score display
        timerDisplay.text = "Time: 60s";  // Reset the timer display
        feedbackDisplay.text = "";  // Clear the feedback display
        inputDisplay.text = "";  // Clear the input field
        inputDisplay.ActivateInputField();  // Refocus on the input field
    }
}
