//Author: Johnathan Wang ZhiWen
//Filename: AlphabetGameManager
//Description: Manages the alphabet game, including score, generation of letters and input handling




using System.Collections;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

/// <summary>
/// Manages the Alphabet game, including score, timer, input handling, and letter generation.
/// </summary>
public class AlphabetGameManager : MonoBehaviour
{
    /// <summary>Current game score.</summary>
    public int score;

    /// <summary>The current letter to be matched by the player.</summary>
    private char currentLetter = 'A';

    /// <summary>Displays the current letter.</summary>
    public TextMeshPro letterDisplay;
    /// <summary>Displays the player's score.</summary>
    public TextMeshPro scoreDisplay;
    /// <summary>Displays feedback on the player's input.</summary>
    public TextMeshPro feedbackDisplay;
    /// <summary>Handles user input.</summary>
    public TMP_InputField inputDisplay;
    /// <summary>Displays the remaining time.</summary>
    public TextMeshPro timerDisplay;
    /// <summary>Game over panel UI element.</summary>
    public GameObject gameOverPanel;

    public Button SkipButton;

    /// <summary>Time remaining in seconds.</summary>
    private float timeRemaining = 60f;
    /// <summary>Indicates whether the game is over.</summary>
    private bool isGameOver = false;
    /// <summary>Determines if the letters are generated sequentially or randomly.</summary>
    public bool isSequential = true;

    /// <summary>Represents the player's left hand object.</summary>
    public GameObject leftHand;
    /// <summary>Represents the player's right hand object.</summary>
    public GameObject rightHand;
    /// <summary>Default material for the hands.</summary>
    public Material defaultMat;
    /// <summary>Material to indicate a correct answer.</summary>
    public Material correctMat;
    /// <summary>Material to indicate a wrong answer.</summary>
    public Material wrongMat;

    void OnEnable()
    {
        GenerateLetter();  
        inputDisplay.onValueChanged.AddListener(delegate { CheckLetterInput(); });
        SkipButton.onClick.AddListener(SkipLetter);
    }

    void OnDisable()
    {
        inputDisplay.onValueChanged.RemoveListener(delegate { CheckLetterInput(); });
        currentLetter = ' ';
        SkipButton.onClick.RemoveListener(SkipLetter);
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

    /// <summary>
    /// Switches between sequential and random letter generation modes.
    /// </summary>
    public void SwitchMode()
    {
        isSequential = !isSequential;
        GenerateLetter();
    }

    /// <summary>
    /// Generates a new letter based on the current mode.
    /// </summary>
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

    /// <summary>
    /// Generates the next letter in sequence, skipping 'J' and 'R'.
    /// </summary>
    void GenerateNextLetter()
    {
        if (currentLetter < 'Z')
        {
            do
            {
                currentLetter++;
            }
            while (currentLetter == 'J' || currentLetter == 'R');
        }
        else
        {
            currentLetter = 'A';
        }

        letterDisplay.text = currentLetter.ToString();
    }

    /// <summary>
    /// Generates a random letter, ensuring 'J' and 'R' are skipped.
    /// </summary>
    void GenerateRandomLetter()
    {
        do
        {
            currentLetter = (char)Random.Range(65, 91);
        }
        while (currentLetter == 'J' || currentLetter == 'R');

        letterDisplay.text = currentLetter.ToString();
    }

    /// <summary>
    /// Checks the player's input against the current letter.
    /// </summary>
    public void CheckLetterInput()
    {
        if (inputDisplay.text.Length > 0 && !isGameOver)
        {
            char enteredChar = inputDisplay.text[inputDisplay.text.Length - 1];
            Debug.Log("Entered Character: " + enteredChar);
            Debug.Log("Current Letter: " + currentLetter);

            if (char.ToUpper(enteredChar) == currentLetter)
            {
                score += 10;
                feedbackDisplay.text = "Correct!";
                Debug.Log("Correct input! Score increased to: " + score);

                StartCoroutine(DisplayHandsCorrect());

                if (AudioManager.instance != null)
                {
                    Debug.Log("Playing SFX: Correct");
                    AudioManager.instance.PlaySFX("Correct");
                }
                else
                {
                    Debug.LogWarning("AudioManager instance is NULL!");
                }

                GenerateLetter();
            }
            else
            {
                feedbackDisplay.text = "Incorrect. Try again!";
                Debug.Log("Incorrect input!");

                StartCoroutine(DisplayHandsWrong());

                if (AudioManager.instance != null)
                {
                    Debug.Log("Playing SFX: Wrong");
                    AudioManager.instance.PlaySFX("Wrong");
                }
                else
                {
                    Debug.LogWarning("AudioManager instance is NULL!");
                }
            }

            scoreDisplay.text = "Score: " + score;
        }
        else
        {
            Debug.LogWarning("No input detected or game is over.");
        }
    }


    /// <summary>
    /// Changes hand material to correct and resets after delay.
    /// </summary>
    IEnumerator DisplayHandsCorrect()
    {
        ChangeHandMaterial(correctMat);
        yield return new WaitForSeconds(1.5f);
        ChangeHandMaterial(defaultMat);
    }

    /// <summary>
    /// Changes hand material to incorrect and resets after delay.
    /// </summary>
    IEnumerator DisplayHandsWrong()
    {
        ChangeHandMaterial(wrongMat);
        yield return new WaitForSeconds(1.5f);
        ChangeHandMaterial(defaultMat);
    }

    /// <summary>
    /// Changes the material of both hands.
    /// </summary>
    /// <param name="mat">The material to apply.</param>
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
    }

    /// <summary>
    /// Ends the game, displays game over screen, and logs score.
    /// </summary>
    void EndGame()
    {
        gameOverPanel.SetActive(true);
        feedbackDisplay.text = "Time's up! Game over!";
        scoreDisplay.text = "Score: " + score;
        GameObject database = GameObject.Find("Database");
        database.GetComponent<Database>().WriteAlphaGameData(score);
    }

    /// <summary>
    /// Restarts the game, resetting score and timer.
    /// </summary>
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

    /// <summary>
    /// Skips the current letter, reducing the score.
    /// </summary>
    public void SkipLetter()
    {
        GenerateLetter();
        score -= 5;
    }
}
