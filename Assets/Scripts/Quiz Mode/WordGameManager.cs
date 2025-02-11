/* Author: Wang Johnathan Zhiwen
* Filename: WordGameManager
* Description: Manages a word quiz game with randomized words.
*/

using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class WordQuiz : MonoBehaviour
{
    public int score;
    public TMP_InputField inputField;
    public TextMeshProUGUI feedbackText;
    public TextMeshProUGUI wordDisplayText;
    public TextMeshProUGUI timerText; // Added: UI for displaying remaining time
    public List<string> wordList;

    private HashSet<string> usedWords = new HashSet<string>();
    private string currentWord = "";
    
    private float timeRemaining = 60f;
    private bool isGameOver = false;

    void Start()
    {
        if (wordList.Count == 0)
        {
            Debug.LogError("Word list is empty!");
            return;
        }
        score = 0;
        inputField.onSubmit.AddListener(delegate { ValidateWord(); });
        GetNewWord();
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
            feedbackText.text = "Correct!";
            score += 10;

            GetNewWord();
        }
        else
        {
            feedbackText.text = "Incorrect. Try again!";
        }

        inputField.text = ""; // Added: Clear input field
        inputField.ActivateInputField(); // Added: Refocus on input field
    }

    void EndGame()
    {
        feedbackText.text = "Game Over! Final Score: " + score;
    }
}
