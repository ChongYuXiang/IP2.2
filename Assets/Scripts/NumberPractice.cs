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
    }

    void CheckNumberInput()
    {
        if (inputDisplay.text.Length > 0)
        {
            char enteredChar = inputDisplay.text[inputDisplay.text.Length - 1];

            if (char.ToUpper(enteredChar) == currentNumber)
            {
                StartCoroutine("DisplayHandsCorrect");

                barProgress += 1;
                progressBar.fillAmount = (float)barProgress / 10f; // Update progress bar
                if (barProgress == 10 && GameManager.instance.wordsUnlocked == false) // If progress is complete, unlock word mode
                {
                    unlockWords.SetActive(false);
                    wordButton.interactable = true;
                    GameManager.instance.wordsUnlocked = true;

                    // Show popup
                    endPopup.SetActive(true);
                    popupPage.SetActive(true);
                }

                nextButton.gameObject.SetActive(true); // Show the "Next" button
            }
            else
            {
                StartCoroutine("DisplayHandsWrong");
                nextButton.gameObject.SetActive(false); // Hide the "Next" button if the input is wrong
            }
        }
    }

    IEnumerator DisplayHandsCorrect() // Change hands to a green material
    {
        leftHand.GetComponent<SkinnedMeshRenderer>().materials[1] = correctMat;
        rightHand.GetComponent<SkinnedMeshRenderer>().materials[1] = correctMat;
        yield return new WaitForSeconds(1.5f);
        leftHand.GetComponent<SkinnedMeshRenderer>().materials[1] = defaultMat;
        rightHand.GetComponent<SkinnedMeshRenderer>().materials[1] = defaultMat;
    }

    IEnumerator DisplayHandsWrong() // Change hands to a red material
    {
        leftHand.GetComponent<SkinnedMeshRenderer>().materials[1] = wrongMat;
        rightHand.GetComponent<SkinnedMeshRenderer>().materials[1] = wrongMat;
        yield return new WaitForSeconds(1.5f);
        leftHand.GetComponent<SkinnedMeshRenderer>().materials[1] = defaultMat;
        rightHand.GetComponent<SkinnedMeshRenderer>().materials[1] = defaultMat;
    }
}