/* Author: Chong Yu Xiang
* Filename: AlphabetPractice
* Descriptions: For alphabet learning mode
*/

using System.Collections;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class AlphabetPractice : MonoBehaviour
{
    private char currentLetter = 'A';
    private int progress = 0;
    public TextMeshPro letterDisplay;
    public ApplyTextureToPanel letterExampleImg;
    public TMP_InputField inputDisplay;
    public Image progressBar;
    public Button nextButton;

    public GameObject leftHand;
    public GameObject rightHand;
    public Material defaultMat;
    public Material correctMat;
    public Material wrongMat;

    public GameObject unlockNumbers;
    public Button numberButton;

    public ParticleSystem confetti;

    public GameObject endPopup;
    public GameObject popupPage;

    void OnEnable()
    {
        GenerateLetter();  
        inputDisplay.onValueChanged.AddListener(delegate { CheckLetterInput(); });
        nextButton.onClick.AddListener(GenerateNextLetter);
        nextButton.gameObject.SetActive(false);

        if (GameManager.instance.numbersUnlocked)
        {
            progress = 24;
            progressBar.fillAmount = (float)progress / 24f;
        }
    }

    void OnDisable()
    {
        inputDisplay.onValueChanged.RemoveListener(delegate { CheckLetterInput(); });
        currentLetter = ' ';
        nextButton.onClick.RemoveListener(GenerateNextLetter);
    }

    void GenerateLetter()
    {
        letterDisplay.text = currentLetter.ToString();
        letterExampleImg.SendMessage("ChangeDisplay", currentLetter.ToString());
        progressBar.fillAmount = (float)progress / 24f;
    }

    void GenerateNextLetter()
    {
        nextButton.gameObject.SetActive(false);

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

        GenerateLetter();
        
        progress += 1;
        progressBar.fillAmount = (float)progress / 24f;
        if (progress >= 24 && GameManager.instance.numbersUnlocked == false)
        {
            PlayConfetti();
            AudioManager.instance.PlaySFX("Confetti");

            unlockNumbers.SetActive(false);
            numberButton.interactable = true;

            GameManager.instance.numbersUnlocked = true;
            endPopup.SetActive(true);
            popupPage.SetActive(true);
        }
    }

    void CheckLetterInput()
    {
        if (inputDisplay.text.Length > 0)
        {
            char enteredChar = inputDisplay.text[inputDisplay.text.Length - 1];

            if (char.ToUpper(enteredChar) == currentLetter)
            {
                StartCoroutine(DisplayHandsCorrect());
                AudioManager.instance.PlaySFX("Correct");
                nextButton.gameObject.SetActive(true);
            }
            else if (!nextButton.gameObject.activeSelf)
            {
                StartCoroutine(DisplayHandsWrong());
                AudioManager.instance.PlaySFX("Wrong");
            }
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

    void PlayConfetti()
    {
        confetti.Play();
    }
}
