using System.Collections;
using UnityEngine;
using TMPro;

public class AlphabetGameManager : MonoBehaviour
{
    public int score;

    private char currentLetter = 'A';
    public TextMeshPro letterDisplay;
    public TextMeshPro scoreDisplay;
    public TextMeshPro feedbackDisplay;
    public TMP_InputField inputDisplay;
    public TextMeshPro timerDisplay;
    public GameObject gameOverPanel;

    private float timeRemaining = 60f;
    private bool isGameOver = false;
    public bool isSequential = true; // Toggle between sequential and random mode

    public GameObject leftHand;
    public GameObject rightHand;
    public Material defaultMat;
    public Material correctMat;
    public Material wrongMat;

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
                StartCoroutine("DisplayHandsCorrect");
                GenerateLetter();
            }
            else
            {
                StartCoroutine("DisplayHandsWrong");
                feedbackDisplay.text = "Incorrect. Try again!";
            }

            scoreDisplay.text = "Score: " + score;
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
        gameOverPanel.SetActive(true);
        feedbackDisplay.text = "Time's up! Game over!";
        scoreDisplay.text = "Final Score: " + score;

        // Find and tell database to create word game data
        GameObject database;
        database = GameObject.Find("Database");
        database.GetComponent<Database>().WriteAlphaGameData(score);
    }

    public void RestartGame()
    {
        score = 0;
        timeRemaining = 60f;
        isGameOver = false;
        gameOverPanel.SetActive(false);
        GenerateLetter();
        scoreDisplay.text = "Score: " + score;
        feedbackDisplay.text = "";
    }
}
