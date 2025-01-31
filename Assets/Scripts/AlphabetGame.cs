/* Author: Chong Yu Xiang  
 * Filename: AlphabetGame
 * Descriptions: Class for alphabet_game
 */

using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;  // For UI elements

public class AlphabetGame : MonoBehaviour
{
    public int highscore;
    public int correct;
    public int wrong;
    public int time_taken;
    public int average_time_per_letter;

    public AlphabetGame(int highscore, int correct, int wrong, int time_taken, int average_time_per_letter)
    {
        this.highscore = highscore;
        this.correct = correct;
        this.wrong = wrong;
        this.time_taken = time_taken;
        this.average_time_per_letter = average_time_per_letter;
    }

    private char currentLetter;  // The current random letter
    public TextMeshProUGUI letterDisplay;  // Reference to the UI text component that will display the letter
    public TextMeshProUGUI scoreDisplay;   // Reference to the UI text component that will display the score
    public TextMeshProUGUI feedbackDisplay; // For displaying feedback (correct/incorrect)

    private float startTime;  // Time when the letter was shown

    void Start()
    {
        GenerateRandomLetter();  // Start with a random letter
        startTime = Time.time;   // Track the start time
    }

    void Update()
    {
        if (Input.anyKeyDown) // Check if any key is pressed
        {
            CheckLetterInput();  // Check the player's input
        }
    }

    void GenerateRandomLetter()
    {
        // Generate a random letter between A and Z
        currentLetter = (char)Random.Range(65, 91); // ASCII 65 = 'A', 91 is exclusive
        letterDisplay.text = currentLetter.ToString(); // Display the letter
    }

    void CheckLetterInput()
    {
        if (Input.inputString.ToUpper() == currentLetter.ToString())  // Check if the player typed the correct letter
        {
            correct++;  // Increase the correct count
            feedbackDisplay.text = "Correct!";
            GenerateRandomLetter();  // Generate a new random letter
            startTime = Time.time;  // Restart the timer
        }
        else
        {
            wrong++;  // Increase the wrong count
            feedbackDisplay.text = "Incorrect. Try again!";
        }

        // Update the score display
        scoreDisplay.text = "Correct: " + correct + "\nWrong: " + wrong;
    }

    // You can call this method to calculate the average time per letter after the game ends
    void CalculateAverageTime()
    {
        time_taken = (int)(Time.time - startTime);
        average_time_per_letter = correct > 0 ? time_taken / correct : 0;
    }
}
