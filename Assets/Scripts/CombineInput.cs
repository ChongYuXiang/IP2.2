using UnityEngine;
using UnityEngine.UI;

public class CombineInput : MonoBehaviour
{
    public InputField inputField1;  // Reference to the first input field
    public InputField inputField2;  // Reference to the second input field
    public InputField finalInputField;  // Reference to the final input field

    // This method combines the input fields' text and sets the result to the final input
    public void CombineText()
    {
        string combinedText = inputField1.text + inputField2.text;  // Combine the two inputs with a space
        finalInputField.text = combinedText;  // Set the final input field with the combined text
    }
}