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

    void Start()
    {
        inputDisplay.onValueChanged.AddListener(delegate { CheckNumberInput(); });

        numberDisplay.text = currentNumber.ToString(); // Display current number
        numberExampleImg.SendMessage("ChangeDisplay", currentNumber.ToString()); // Display example sign

        //nextButton.gameObject.SetActive(false); // Hide the next button at the start
        nextButton.onClick.AddListener(GenerateNextNumber); // Set up the button to call GenerateNextNumber
    }

    void GenerateNextNumber()
    {
        //nextButton.gameObject.SetActive(false); // Hide the next button when generating a new number

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
        progressBar.fillAmount = (float)barProgress / 10f;
        if (barProgress >= 10) // If progress is complete, unlock number mode
        {
            unlockWords.SetActive(false);
            wordButton.interactable = true;
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

                barProgress += 1;
                progressBar.fillAmount = (float)barProgress / 10f; // Update progress bar
                if (barProgress >= 10) // If progress is complete, unlock number mode
                {
                    unlockWords.SetActive(false);
                    wordButton.interactable = true;
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