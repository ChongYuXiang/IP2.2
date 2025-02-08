using System.Collections;
using UnityEngine;
using TMPro;

public class AlphabetGameManager : MonoBehaviour
{
    public int highscore;
    public int score;
    public int wrong;
    public int time_taken;
    public int average_time_per_letter;

    private char currentLetter = 'A';
    public TextMeshPro letterDisplay;
    public TextMeshPro scoreDisplay;
    public TextMeshPro feedbackDisplay;
    public TMP_InputField inputDisplay;
    public TextMeshPro timerDisplay;

    private float timeRemaining = 60f;
    private bool isGameOver = false;
    public bool isSequential = true; // Toggle between sequential and random mode

    void Start()
    {
        GenerateLetter();
        inputDisplay.onValueChanged.AddListener(delegate { CheckLetterInput(); });
    }

    void Update()
    {
        if (!isGameOver)
        {
            timeRemaining -= Time.deltaTime;
            timerDisplay.text = "Time: " + Mathf.Ceil(timeRemaining).ToString() + "s";

            if (timeRemaining <= 0f)
            {
                isGameOver = true;
                timeRemaining = 0f;
                EndGame();
            }
        }
    }

    public void SwitchMode()
    {
        if (isSequential)
        {
            isSequential = false;
            GenerateLetter();
        }
        else
        {
            isSequential = true;
            GenerateLetter();
        }
    }

    void GenerateLetter()
    {
        if (isSequential)
        {
            GenerateNextLetter();
        }
        else
        {
            GenerateRandomLetter();
        }
    }

    void GenerateNextLetter()
    {
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
            currentLetter = 'A';
        }

        letterDisplay.text = currentLetter.ToString();
    }

    void GenerateRandomLetter()
    {
        currentLetter = (char)Random.Range(65, 91); // Random letter between A and Z
        while (currentLetter == 'J' || currentLetter == 'R') // Skip 'J' and 'R'
        {
            currentLetter = (char)Random.Range(65, 91);
        }

        letterDisplay.text = currentLetter.ToString();
    }

    void CheckLetterInput()
    {
        if (inputDisplay.text.Length > 0 && !isGameOver)
        {
            char enteredChar = inputDisplay.text[inputDisplay.text.Length - 1];

            if (char.ToUpper(enteredChar) == currentLetter)
            {
                score += 10;
                feedbackDisplay.text = "Correct!";
                GenerateLetter();
            }
            else
            {
                wrong++;
                feedbackDisplay.text = "Incorrect. Try again!";
            }

            scoreDisplay.text = "Correct: " + score + "\nWrong: " + wrong;
        }
    }

    void EndGame()
    {
        feedbackDisplay.text = "Time's up! Game over!";
        scoreDisplay.text = "Final Score: " + score;
        if (score > highscore)
        {
            highscore = score;
        }
    }
}
