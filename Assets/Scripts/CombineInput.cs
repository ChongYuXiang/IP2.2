using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CombineInput : MonoBehaviour
{
    public TMP_InputField inputField1;  // Reference to the first input field
    public TMP_InputField inputField2;  // Reference to the second input field
    public TMP_InputField finalInputField;  // Reference to the final input field

    // This method combines the input fields' text and sets the result to the final input
    void Start()
    {
        inputField1.onValueChanged.AddListener(delegate { CombineText(); });
        inputField2.onValueChanged.AddListener(delegate { CombineText(); });
    }

    public void CombineText()
    {
        string combinedText = inputField1.text + inputField2.text;  // Combine the two inputs with a space
        finalInputField.text = combinedText;  // Set the final input field with the combined text
    }
}