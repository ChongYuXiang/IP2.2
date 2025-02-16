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

    // Confetti VFX
    public ParticleSystem confetti;

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

        confetti.Stop(); //Ensure Confetti does not happen at the start
    }

    public void BeginLearning()
    {
        inputDisplay.onValueChanged.AddListener(delegate { CheckLetterInput(); });

        letterDisplay.text = currentLetter.ToString(); // Display current number
        letterExampleImg.SendMessage("ChangeDisplay", currentLetter.ToString()); // Display example sign

        nextButton.onClick.AddListener(GenerateNextLetter); // Set up the button to call GenerateLetter
        nextButton.gameObject.SetActive(false); // Hide the next button at the start

        progressBar.fillAmount = (float)progress / 24f; // Update progress bar
    }

    public void PlayConfetti()
    {
        confetti.Play(); // Trigger confetti effect
    }

    void GenerateNextLetter()
    {
        nextButton.gameObject.SetActive(false); // Hide the next button when generating a new letter (Delete line for testing)

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

        progress += 1;
        progressBar.fillAmount = (float)progress / 24f; // Update progress bar
        if (progress >= 24 && GameManager.instance.numbersUnlocked == false) // If progress is complete, unlock number mode
        {
            PlayConfetti(); // Trigger confetti when game ends

            unlockNumbers.SetActive(false);
            numberButton.interactable = true;

            GameManager.instance.numbersUnlocked = true;

            // Show popup
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

                nextButton.gameObject.SetActive(true); // Show the "Next" button
            }
            else
            {
                StartCoroutine(DisplayHandsWrong());
                nextButton.gameObject.SetActive(false); // Hide the "Next" button if the input is wrong
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
