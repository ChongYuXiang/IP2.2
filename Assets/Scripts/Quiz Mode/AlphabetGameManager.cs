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

    public void CheckLetterInput()
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
        Debug.Log("Green hand");

        ChangeHandMaterial(correctMat);

        yield return new WaitForSeconds(1.5f);

        ChangeHandMaterial(defaultMat);
    }

    IEnumerator DisplayHandsWrong() // Change hands to a red material
    {
        Debug.Log("Red hand");

        ChangeHandMaterial(wrongMat);

        yield return new WaitForSeconds(1.5f);

        ChangeHandMaterial(defaultMat);
    }

    void ChangeHandMaterial(Material mat)
    {
        SkinnedMeshRenderer leftRenderer = leftHand.GetComponent<SkinnedMeshRenderer>();
        SkinnedMeshRenderer rightRenderer = rightHand.GetComponent<SkinnedMeshRenderer>();

        if (leftRenderer == null || rightRenderer == null)
        {
            Debug.LogError("SkinnedMeshRenderer component missing on left or right hand!");
            return;
        }

        Material[] leftMaterials = leftRenderer.materials;
        Material[] rightMaterials = rightRenderer.materials;

        if (leftMaterials.Length > 1) leftMaterials[1] = mat;
        else leftMaterials[0] = mat;

        if (rightMaterials.Length > 1) rightMaterials[1] = mat;
        else rightMaterials[0] = mat;

        leftRenderer.materials = leftMaterials;
        rightRenderer.materials = rightMaterials;

        Debug.Log($"Hand material changed to {mat.name}");
    }

    void EndGame()
    {
        gameOverPanel.SetActive(true);
        feedbackDisplay.text = "Time's up! Game over!";
        scoreDisplay.text = "Final Score: " + score;

        // Find and tell database to create alphabet game data
        FirebaseWebQuery database;
        database = FirebaseWebQuery.instance;
        StartCoroutine(database.PostQuizData("alphabet_game", score));
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

    public void SkipLetter()
    {
        GenerateLetter();
        score -= 5;
    }
}
