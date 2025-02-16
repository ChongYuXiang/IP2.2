using System.Collections;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class NumberPractice : MonoBehaviour
{
    private int currentNumber = 0;
    private int barProgress = 0;
    public TextMeshPro numberDisplay;
    public ApplyTextureToPanel numberExampleImg;
    public TMP_InputField inputDisplay;
    public Image progressBar;
    public Button nextButton;

    public GameObject leftHand;
    public GameObject rightHand;
    public Material defaultMat;
    public Material correctMat;
    public Material wrongMat;

    public GameObject unlockWords;
    public Button wordButton;

    public ParticleSystem confetti;

    public GameObject endPopup;
    public GameObject popupPage;

    void OnEnable()
    {
        GenerateNumber();  
        inputDisplay.onValueChanged.AddListener(delegate { CheckNumberInput(); });
        nextButton.onClick.AddListener(GenerateNextNumber);
    }

    void OnDisable()
    {
        inputDisplay.onValueChanged.RemoveListener(delegate { CheckNumberInput(); });
        currentNumber = -1;
        nextButton.onClick.RemoveListener(GenerateNextNumber);
    }

    void GenerateNumber()
    {
        numberDisplay.text = currentNumber.ToString();
        numberExampleImg.SendMessage("ChangeDisplay", currentNumber.ToString());
        progressBar.fillAmount = (float)barProgress / 10f;
    }

    void SkipNumber()
    {
        GenerateNextNumber();
    }

    void GenerateNextNumber()
    {
        nextButton.gameObject.SetActive(false);

        currentNumber = (currentNumber < 9) ? currentNumber + 1 : 0;
        
        GenerateNumber();
        
        barProgress += 1;
        progressBar.fillAmount = (float)barProgress / 10f;
        if (barProgress >= 10 && GameManager.instance.wordsUnlocked == false)
        {
            PlayConfetti();
            AudioManager.instance.PlaySFX("Confetti");

            unlockWords.SetActive(false);
            wordButton.interactable = true;

            GameManager.instance.wordsUnlocked = true;
            endPopup.SetActive(true);
            popupPage.SetActive(true);
        }
    }

    void CheckNumberInput()
    {
        if (inputDisplay.text.Length > 0)
        {
            char enteredChar = inputDisplay.text[inputDisplay.text.Length - 1];

            if (char.GetNumericValue(enteredChar) == currentNumber)
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
