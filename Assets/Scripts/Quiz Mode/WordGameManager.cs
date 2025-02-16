/* Author: Wang Johnathan Zhiwen
* Filename: WordGameManager
* Description: Manages a word quiz game with randomized words.
*/

using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;

public class WordQuiz : MonoBehaviour
{
    public int score;
    public TMP_InputField inputField;
    public TextMeshPro scoreDisplay;
    public TextMeshPro wordDisplayText;
    public TextMeshPro timerText; // Added: UI for displaying remaining time
    public GameObject gameOverPanel; // Added: Panel to display when the game ends
    public List<string> wordList;
    public Button SkipButton;

    private HashSet<string> usedWords = new HashSet<string>();
    private string currentWord = "";
    
    private float timeRemaining = 60f;
    private bool isGameOver = false;

    public GameObject leftHand;
    public GameObject rightHand;
    public Material defaultMat;
    public Material correctMat;
    public Material wrongMat;

    void OnEnable()
    {
        GetNewWord();  
        inputField.onValueChanged.AddListener(delegate { ValidateWord(); });
        SkipButton.onClick.AddListener(SkipWord);
    }

    void OnDisable()
    {
        inputField.onValueChanged.RemoveListener(delegate { ValidateWord(); });
        SkipButton.onClick.RemoveListener(SkipWord);
    }

    void Update()
    {
        if (!isGameOver)
        {
            timeRemaining -= Time.deltaTime;
            timerText.text = "Time: " + Mathf.CeilToInt(timeRemaining); // Added: Update UI timer

            if (timeRemaining <= 0f)
            {
                timeRemaining = 0f;
                isGameOver = true;
                EndGame();
            }
        }
    }

    void GetNewWord()
    {
        if (usedWords.Count >= wordList.Count)
        {
            usedWords.Clear();
        }

        List<string> availableWords = new List<string>(wordList);
        availableWords.RemoveAll(word => usedWords.Contains(word));

        if (availableWords.Count > 0)
        {
            currentWord = availableWords[Random.Range(0, availableWords.Count)];
            wordDisplayText.text = currentWord;
            usedWords.Add(currentWord);
        }
        else
        {
            wordDisplayText.text = "No more words!";
        }
    }

    public void ValidateWord()
    {
        if (isGameOver) return; // Added: Prevent input after game ends

        string inputText = inputField.text.Trim();

        if (string.IsNullOrEmpty(inputText)) return;

        if (inputText.Equals(currentWord, System.StringComparison.OrdinalIgnoreCase))
        {
            scoreDisplay.text = score.ToString();
            score += 10;

            StartCoroutine("DisplayHandsCorrect");
            AudioManager.instance.PlaySFX("Correct");

            GetNewWord();
        }
        else
        {
            StartCoroutine("DisplayHandsWrong");
            AudioManager.instance.PlaySFX("Wrong");
        }

        inputField.text = ""; // Added: Clear input field
        inputField.ActivateInputField(); // Added: Refocus on input field
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
        isGameOver = true;
        gameOverPanel.SetActive(true);
        scoreDisplay.text = "Game Over! " + score;
        Debug.Log("Final Score: " + score);

        // Find and tell database to create word game data
        FirebaseWebQuery database;
        database = FirebaseWebQuery.instance;
        StartCoroutine(database.PostQuizData("word_game", score));
    }

    public void RestartGame()
    {
        score = 0;
        timeRemaining = 60f;
        isGameOver = false;
        gameOverPanel.SetActive(false);
        scoreDisplay.text = "Score: " + score;
        timerText.text = "Time: 60s";
        GetNewWord();
        inputField.text = "";
        inputField.ActivateInputField();
    }

    public void SkipWord()
    {
        GetNewWord();
        inputField.text = "";
        inputField.ActivateInputField();
        score -= 5;
    }
}
