/* Author: Wang Johnathan Zhiwen 
* Filename: AlphabetGameManager
* Descriptions: functions for alphabet_game
*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class AlphabetGameManager : MonoBehaviour
{
    public int highscore;
    public int correct;
    public int wrong;
    public int time_taken;
    public int average_time_per_letter;

    private char currentLetter;  // The current random letter
    public TextMeshPro letterDisplay;  // UI text component that will display the letter
    public TextMeshPro scoreDisplay;   // UI text component that will display the score
    public TextMeshPro feedbackDisplay; // For displaying feedback (correct/incorrect)
    public TMP_InputField inputDisplay;  // Input field for player's input

    void Start()
    {
        GenerateRandomLetter();  // Start with a random letter
        inputDisplay.onValueChanged.AddListener(delegate { CheckLetterInput(); }); // Listen for input changes
    }

    void GenerateRandomLetter()
    {
        // Generate a random letter between A and Z
        currentLetter = (char)Random.Range(65, 91); // ASCII 65 = 'A', 91 is exclusive
        letterDisplay.text = currentLetter.ToString(); // Display the letter
    }

    void CheckLetterInput()
    {
        if (inputDisplay.text.Length > 0) // Ensure there's input before checking
        {
            char enteredChar = inputDisplay.text[inputDisplay.text.Length - 1]; // Get the last entered character

            if (char.ToUpper(enteredChar) == currentLetter)  // Check if the player typed the correct letter
            {
                correct++;  // Increase the correct count
                feedbackDisplay.text = "Correct!";
                GenerateRandomLetter();  // Generate a new random letter
            }
            else
            {
                wrong++;  // Increase the wrong count
                feedbackDisplay.text = "Incorrect. Try again!";
            }

            // Update the score display
            scoreDisplay.text = "Correct: " + correct + "\nWrong: " + wrong;
        }
    }


}
