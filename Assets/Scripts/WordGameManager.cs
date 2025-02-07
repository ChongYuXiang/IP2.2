/* Author: Wang Johnathan Zhiwen 
* Filename: WordGameManager
* Description: Manages a word quiz game with randomized words.
*/

using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class WordQuiz : MonoBehaviour
{
    public int highscore;
    public TMP_InputField inputField;
    public TextMeshProUGUI feedbackText;
    public TextMeshProUGUI scoreText;
    public TextMeshProUGUI wordDisplayText; // UI to show the current word
    public List<string> wordList;

    private HashSet<string> usedWords = new HashSet<string>(); // Track used words
    private string currentWord = "";
    private int score = 0;

    void Start()
    {
        if (wordList.Count == 0)
        {
            Debug.LogError("Word list is empty!");
            return;
        }

        inputField.onEndEdit.AddListener(delegate { ValidateWord(); });
        GetNewWord();
        UpdateScoreUI();
    }

    void GetNewWord()
    {
        if (usedWords.Count >= wordList.Count) // Reset if all words are used
        {
            usedWords.Clear();
        }

        List<string> availableWords = new List<string>(wordList);
        availableWords.RemoveAll(word => usedWords.Contains(word));

        if (availableWords.Count > 0)
        {
            currentWord = availableWords[Random.Range(0, availableWords.Count)];
            wordDisplayText.text = currentWord; // Show word on screen
            usedWords.Add(currentWord);
        }
        else
        {
            wordDisplayText.text = "No more words!";
        }
    }

    public void ValidateWord()
    {
        string inputText = inputField.text.Trim();

        if (string.IsNullOrEmpty(inputText))
            return;

        if (inputText.Equals(currentWord, System.StringComparison.OrdinalIgnoreCase))
        {
            feedbackText.text = "Correct!";
            score += 10;

            if (score > highscore)
            {
                highscore = score;
            }

            GetNewWord(); // Move to the next word
        }
        else
        {
            feedbackText.text = "Incorrect. Try again!";
        }

        inputField.text = ""; // Clear input field
        UpdateScoreUI();
    }

    private void UpdateScoreUI()
    {
        scoreText.text = $"Score: {score}  Highscore: {highscore}";
    }
}
