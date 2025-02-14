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

    public Button nextButton; // Reference to the "Next" button

    public GameObject leftHand;
    public GameObject rightHand;
    public Material defaultMat;
    public Material correctMat;
    public Material wrongMat;

    void Start()
    {
        inputDisplay.onValueChanged.AddListener(delegate { CheckLetterInput(); });

        //nextButton.gameObject.SetActive(false); // Hide the next button at the start
        nextButton.onClick.AddListener(GenerateLetter); // Set up the button to call GenerateLetter
    }

    public void GenerateLetter()
    {
        GenerateNextLetter();

        //nextButton.gameObject.SetActive(false); // Hide the next button when generating a new letter
    }

    void GenerateNextLetter()
    {
        // Skip 'J' and 'R', and cycle back to 'A' after 'Z'
        if (currentLetter < 'Z')
        {
            do
            {
                currentLetter++;
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
                progressBar.fillAmount = (float)progress / 24f;
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

