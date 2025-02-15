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
    // Learning UI elements
    private char currentLetter = 'A';
    private int progress = 0;
    public TextMeshPro letterDisplay;
    public ApplyTextureToPanel letterExampleImg;
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
    public GameObject unlockNumbers;
    public Button numberButton;

    // End popup
    public GameObject endPopup;
    public GameObject popupPage;

    void Start()
    {
        if (GameManager.instance.numbersUnlocked == true) // Check GameManager if numbers are already unlocked
        {
            unlockNumbers.SetActive(false);
            numberButton.interactable = true;
            progress = 24;
        }
    }

    public void BeginLearning()
    {
        inputDisplay.onValueChanged.AddListener(delegate { CheckLetterInput(); });

        letterDisplay.text = currentLetter.ToString(); // Display current number
        letterExampleImg.SendMessage("ChangeDisplay", currentLetter.ToString()); // Display example sign

        nextButton.gameObject.SetActive(false); // Hide the next button at the start
        nextButton.onClick.AddListener(GenerateNextLetter); // Set up the button to call GenerateLetter

        progressBar.fillAmount = (float)progress / 24f; // Update progress bar
    }

    void GenerateNextLetter()
    {
        nextButton.gameObject.SetActive(false); // Hide the next button when generating a new letter

        // Skip 'J' and 'R', and cycle back to 'A' after 'Z'
        if (currentLetter < 'Z')
        {
            do
            {
                currentLetter++; // Next letter
            }
            while (currentLetter == 'J' || currentLetter == 'R'); // Skip 'J' and 'R'
        }
        else
        {
            currentLetter = 'A'; // Reset to 'A' after 'Z'
        }

        letterDisplay.text = currentLetter.ToString();
        letterExampleImg.SendMessage("ChangeDisplay", currentLetter.ToString());
    }

    void CheckLetterInput()
    {
        if (inputDisplay.text.Length > 0)
        {
            char enteredChar = inputDisplay.text[inputDisplay.text.Length - 1];

            if (char.ToUpper(enteredChar) == currentLetter)
            {
                StartCoroutine("DisplayHandsCorrect");

                progress += 1;
                progressBar.fillAmount = (float)progress / 24f; // Update progress bar
                if (progress == 24 && GameManager.instance.numbersUnlocked == false) // If progress is complete, unlock number mode
                {
                    unlockNumbers.SetActive(false);
                    numberButton.interactable = true;
                    GameManager.instance.numbersUnlocked = true;

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

