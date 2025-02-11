using System.Collections;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class AlphabetPractice : MonoBehaviour
{
    private char currentLetter = 'A';
    public TextMeshPro letterDisplay;
    public TextMeshPro feedbackDisplay;
    public TMP_InputField inputDisplay;

    public Button nextButton; // Reference to the "Next" button

    void Start()
    {
        GenerateLetter();
        inputDisplay.onValueChanged.AddListener(delegate { CheckLetterInput(); });

        nextButton.gameObject.SetActive(false); // Hide the next button at the start
        nextButton.onClick.AddListener(GenerateLetter); // Set up the button to call GenerateLetter
    }

    void GenerateLetter()
    {
        GenerateNextLetter();

        nextButton.gameObject.SetActive(false); // Hide the next button when generating a new letter
    }

    void GenerateNextLetter()
    {
        // Skip 'J' and 'R', and cycle back to 'A' after 'Z'
        if (currentLetter < 'Z')
        {
            do
            {
                currentLetter++;
            }
            while (currentLetter == 'J' || currentLetter == 'R'); // Skip 'J' and 'R'
        }
        else
        {
            currentLetter = 'A'; // Reset to 'A' after 'Z'
        }

        letterDisplay.text = currentLetter.ToString();
    }

    void CheckLetterInput()
    {
        if (inputDisplay.text.Length > 0)
        {
            char enteredChar = inputDisplay.text[inputDisplay.text.Length - 1];

            if (char.ToUpper(enteredChar) == currentLetter)
            {
                feedbackDisplay.text = "Correct!";
                nextButton.gameObject.SetActive(true); // Show the "Next" button
            }
            else
            {
                feedbackDisplay.text = "Incorrect. Try again!";
                nextButton.gameObject.SetActive(false); // Hide the "Next" button if the input is wrong
            }
        }
    }
}

