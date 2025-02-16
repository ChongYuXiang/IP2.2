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

    void Start()
    {
        inputDisplay.onValueChanged.AddListener(delegate { CheckLetterInput(); });

        letterDisplay.text = currentLetter.ToString(); // Display current letter
        letterExampleImg.SendMessage("ChangeDisplay", currentLetter.ToString()); // Display example sign

        nextButton.onClick.AddListener(GenerateNextLetter); // Set up the button to call GenerateNextLetter
    }

    void GenerateNextLetter()
    {
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

        if (progress >= 24) // If progress is complete, unlock number mode
        {
            unlockNumbers.SetActive(false);
            numberButton.interactable = true;
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

                progress += 1;
                progressBar.fillAmount = (float)progress / 24f; // Update progress bar

                if (progress >= 24) // If progress is complete, unlock number mode
                {
                    unlockNumbers.SetActive(false);
                    numberButton.interactable = true;
                }

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
