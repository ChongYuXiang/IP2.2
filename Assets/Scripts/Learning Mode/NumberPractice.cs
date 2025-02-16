/* Author: Chong Yu Xiang  
 * Filename: NumberPractice
 * Descriptions: For number learning mode
 */

using System.Collections;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class NumberPractice : MonoBehaviour
{
    // Learning UI elements
    private int currentNumber = 0;
    private int barProgress = 0;
    public TextMeshPro numberDisplay;
    public ApplyTextureToPanel numberExampleImg;
    public TMP_InputField inputDisplay;
    public Image progressBar;
    public Button nextButton;

    // Hands color swapping
    public GameObject leftHand;
    public GameObject rightHand;
    public Material defaultMat;
    public Material correctMat;
    public Material wrongMat;

    // For unlocking next mode
    public GameObject unlockWords;
    public Button wordButton;

    // Confetti VFX
    public ParticleSystem confetti;

    // End popup
    public GameObject endPopup;
    public GameObject popupPage;

    void Start()
    {
        if (GameManager.instance.wordsUnlocked == true) // Check GameManager if words are already unlocked
        {
            unlockWords.SetActive(false);
            wordButton.interactable = true;
            barProgress = 10;
        }

        confetti.Stop(); //Ensure Confetti does not happen at the start
    }

    public void BeginLearning()
    {
        inputDisplay.onValueChanged.AddListener(delegate { CheckNumberInput(); });

        numberDisplay.text = currentNumber.ToString(); // Display current number
        numberExampleImg.SendMessage("ChangeDisplay", currentNumber.ToString()); // Display example sign

        nextButton.gameObject.SetActive(false); // Hide the next button at the start
        nextButton.onClick.AddListener(GenerateNextNumber); // Set up the button to call GenerateNextNumber

        progressBar.fillAmount = (float)barProgress / 10f; // Update progress bar
    }

    public void PlayConfetti()
    {
        confetti.Play(); // Trigger confetti effect
    }

    void GenerateNextNumber()
    {
        nextButton.gameObject.SetActive(false); // Hide the next button when generating a new number

        // Cycle back to 0 after 9
        if (currentNumber < 9)
        {
            currentNumber++; // Next number
        }
        else
        {
            currentNumber = 0; // Reset to 0
        }

        numberDisplay.text = currentNumber.ToString();
        numberExampleImg.SendMessage("ChangeDisplay", currentNumber.ToString());    
        
        barProgress += 1;
        progressBar.fillAmount = (float)barProgress / 10f; // Update progress bar
        if (barProgress >= 10 && GameManager.instance.wordsUnlocked == false) // If progress is complete, unlock number mode
        {
            PlayConfetti(); // Trigger confetti when game ends
            AudioManager.instance.PlaySFX("Confetti");

            unlockWords.SetActive(false);
            wordButton.interactable = true;

            GameManager.instance.wordsUnlocked = true;

            // Show popup
            endPopup.SetActive(true);
            popupPage.SetActive(true);
        }
    }

    void CheckNumberInput()
    {
        if (inputDisplay.text.Length > 0)
        {
            char enteredChar = inputDisplay.text[inputDisplay.text.Length - 1];

            if (char.ToUpper(enteredChar) == currentNumber)
            {
                StartCoroutine("DisplayHandsCorrect");
                AudioManager.instance.PlaySFX("Correct");

                nextButton.gameObject.SetActive(true); // Show the "Next" button
            }
            else if (nextButton.gameObject.activeSelf == false)
            {
                StartCoroutine("DisplayHandsWrong");
                AudioManager.instance.PlaySFX("Wrong");
            }
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