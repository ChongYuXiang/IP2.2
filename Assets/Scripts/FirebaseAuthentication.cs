using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class FirebaseAuthentication : MonoBehaviour
{
    public TMP_InputField emailInput;
    public TMP_InputField passwordInput;
    public TextMeshProUGUI messageText;
    public Button loginButton;
    public Button signUpButton;

    private FirebaseWebQuery firebaseAuth;

    void Start()
    {
        firebaseAuth = gameObject.AddComponent<FirebaseWebQuery>();

        loginButton.onClick.RemoveAllListeners();
        signUpButton.onClick.RemoveAllListeners();

        // Add button click listeners
        loginButton.onClick.AddListener(() => StartCoroutine(SignInUser()));
        signUpButton.onClick.AddListener(() => StartCoroutine(SignUpUser()));
    }

    IEnumerator SignUpUser()
    {
        string email = emailInput.text;
        string password = passwordInput.text;

        Debug.Log("Signing up user with email: " + email);

        if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password))
        {
            messageText.text = "Please enter email and password.";
            yield break;
        }

        yield return StartCoroutine(firebaseAuth.SignUpUser(email, password));
        messageText.text = "Sign-up successful! Now log in.";
    }

    IEnumerator SignInUser()
    {
        string email = emailInput.text;
        string password = passwordInput.text;

        Debug.Log("Signing in user with email: " + email);

        if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password))
        {
            messageText.text = "Please enter email and password.";
            yield break;
        }

        yield return StartCoroutine(firebaseAuth.SignInUser(email, password));

        if (!string.IsNullOrEmpty(firebaseAuth.idToken))
        {
            messageText.text = "Login successful!";
            // Load next scene or enable game content here
        }
        else
        {
            messageText.text = "Login failed. Check your credentials.";
        }
    }
}
