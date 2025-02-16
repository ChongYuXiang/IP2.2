/* Author: Chong Yu Xiang  
* Filename: WordPractice
* Descriptions: For word learning mode
*/

using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class WordPractice : MonoBehaviour
{
    // Learning UI elements
    private int progress = 0;
    public TextMeshPro wordDisplay;
    public TMP_InputField inputDisplay;
    public Image progressBar;
    public Button nextButton;

    // Word list variables
    public List<string> wordList;
    private string currentWord = "";
    private int nextWordIndex = 0;

    // Confetti VFX
    public ParticleSystem confetti;

    // End popup
    public GameObject endPopup;
    public GameObject popupPage;

    // Hands color swapping
    public GameObject leftHand;
    public GameObject rightHand;
    public Material defaultMat;
    public Material correctMat;
    public Material wrongMat;

    void Start()
    {
        if (GameManager.instance.wordsComplete == true) // Check GameManager if numbers are already unlocked
        {
            progress = wordList.Count;
        }

        if (wordList.Count == 0)
        {
            Debug.Log("Word list is empty!");
        }

        confetti.Stop(); //Ensure Confetti does not happen at the start
    }

    public void BeginLearning()
    {
        inputDisplay.onValueChanged.AddListener(delegate { ValidateWord(); });

        currentWord = wordList[nextWordIndex];
        wordDisplay.text = currentWord; // Display current word

        nextButton.onClick.AddListener(GetNewWord); // Set up the button to call GenNewWord
        nextButton.gameObject.SetActive(false); // Hide the next button at the start

        GetNewWord(); // Get the first word
    }

    public void PlayConfetti()
    {
        confetti.Play(); // Trigger confetti effect
    }


    void GetNewWord()
    {
        nextButton.gameObject.SetActive(false); // Hide the next button when generating a new word (Delete line for testing)

        if (wordList.Count > nextWordIndex) // If next word if within the list
        {
            currentWord = wordList[nextWordIndex];
            wordDisplay.text = currentWord; // Display current word
        }
        else
        {
            nextWordIndex = 0; // Go back to first word
            currentWord = wordList[nextWordIndex];
            wordDisplay.text = currentWord; // Display current word
        }
        nextWordIndex += 1; // Increment word count

        wordDisplay.text = currentWord.ToString();

        progress += 1;
        progressBar.fillAmount = (float)progress / wordList.Count; // Update progress bar
        if (progress >= 24 && GameManager.instance.numbersUnlocked == false) // If progress is complete, unlock number mode
        {
            PlayConfetti(); // Trigger confetti when game ends
            AudioManager.instance.PlaySFX("Confetti");

            GameManager.instance.numbersUnlocked = true;

            // Show popup
            endPopup.SetActive(true);
            popupPage.SetActive(true);
        }
    }

    public void ValidateWord()
    {
        string inputText = inputDisplay.text.Trim();

        if (string.IsNullOrEmpty(inputText)) return;

        if (inputText.Equals(currentWord, System.StringComparison.OrdinalIgnoreCase))
        {
            StartCoroutine(DisplayHandsCorrect());
            AudioManager.instance.PlaySFX("Correct");

            nextButton.gameObject.SetActive(true); // Show the "Next" button
        }
        else if (nextButton.gameObject.activeSelf == false)
        {
            StartCoroutine(DisplayHandsWrong());
            AudioManager.instance.PlaySFX("Wrong");
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
}
