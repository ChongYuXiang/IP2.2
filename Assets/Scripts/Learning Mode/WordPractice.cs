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

    // Hands color swapping
    public GameObject leftHand;
    public GameObject rightHand;
    public Material defaultMat;
    public Material correctMat;
    public Material wrongMat;

    // End popup
    public GameObject endPopup;
    public GameObject popupPage;

    void OnEnable()
    {
        GetNewWord();  
        inputDisplay.onValueChanged.AddListener(delegate { ValidateWord(); });
        nextButton.onClick.AddListener(GetNewWord);
    }

    void OnDisable()
    {
        inputDisplay.onValueChanged.RemoveListener(delegate { ValidateWord(); });
        nextButton.onClick.RemoveListener(GetNewWord);
    }

    public void PlayConfetti()
    {
        confetti.Play(); // Trigger confetti effect
    }

    void GetNewWord()
    {
        nextButton.gameObject.SetActive(false); // Hide the next button when generating a new word

        if (wordList.Count > nextWordIndex)
        {
            currentWord = wordList[nextWordIndex];
        }
        else
        {
            nextWordIndex = 0;
            currentWord = wordList[nextWordIndex];
        }
        nextWordIndex += 1;
        wordDisplay.text = currentWord;

        progress += 1;
        progressBar.fillAmount = (float)progress / wordList.Count;

        if (progress >= wordList.Count && GameManager.instance.numbersUnlocked == false)
        {
            PlayConfetti();
            AudioManager.instance.PlaySFX("Confetti");
            GameManager.instance.numbersUnlocked = true;
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
            nextButton.gameObject.SetActive(true);
        }
        else if (nextButton.gameObject.activeSelf == false)
        {
            StartCoroutine(DisplayHandsWrong());
            AudioManager.instance.PlaySFX("Wrong");
        }
    }

    IEnumerator DisplayHandsCorrect()
    {
        ChangeHandMaterial(correctMat);
        yield return new WaitForSeconds(1.5f);
        ChangeHandMaterial(defaultMat);
    }

    IEnumerator DisplayHandsWrong()
    {
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
    }
}
