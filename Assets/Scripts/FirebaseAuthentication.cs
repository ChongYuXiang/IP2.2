/* Author: Chong Yu Xiang, Wang Johnathan Zhiwen
* Filename: FirebaseAuthentication
* Descriptions: Save inputs and buttons for FirebaseWebQuery
*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Analytics;
using System.Reflection;

public class FirebaseAuthentication : MonoBehaviour
{
    // Inputs for auth
    public TMP_InputField emailInput;
    public TMP_InputField passwordInput;
    public TMP_InputField usernameInput;
    public TMP_Dropdown inputGender;
    public TMP_InputField inputAge;

    //public TMP_InputField inputEmailReset;

    // Auth buttons
    public Button loginButton;
    public Button signUpButton;

    // Menu Pages
    public GameObject SignUpPage;
    public GameObject LogInPage;

    private FirebaseWebQuery firebaseAuth;

    void Start()
    {
        firebaseAuth = FirebaseWebQuery.instance;
    }

    public void SignUpFunc(){
        StartCoroutine(SignUpUser());
    }

    public void LogInFunc()
    {
        StartCoroutine(LogInUser());
    }

    public void SignOutFunc()
    {
        StartCoroutine(firebaseAuth.SignOut());
    }

    //Send password reset to email
    public void EmailResetFunc()
    {
        emailInput = GameObject.Find("Email Input Reset").GetComponent<TMP_InputField>();
        string email = emailInput.text;
        StartCoroutine(firebaseAuth.SendPasswordResetEmail(email));
    }

    IEnumerator SignUpUser()
    {
        // Find inputs
        emailInput = GameObject.Find("Email Input 2").GetComponent<TMP_InputField>();
        passwordInput = GameObject.Find("Password Input 2").GetComponent<TMP_InputField>();
        usernameInput = GameObject.Find("Username Input").GetComponent<TMP_InputField>();
        inputGender = GameObject.Find("inputGender").GetComponent<TMP_Dropdown>();
        inputAge = GameObject.Find("inputAge").GetComponent<TMP_InputField>();

        // Save inputs
        string email = emailInput.text;
        string password = passwordInput.text;
        string username = usernameInput.text;
        string gender = inputGender.options[inputGender.value].text;
        string age = inputAge.text;

        // Attempt sign up with inputs
        Debug.Log("Signing up user with email: " + email);
        yield return StartCoroutine(firebaseAuth.SignUpUser(email, password, username, gender, age));
    }

    IEnumerator LogInUser()
    {
        // Find inputs
        emailInput = GameObject.Find("Email Input 1").GetComponent<TMP_InputField>();
        passwordInput = GameObject.Find("Password Input 1").GetComponent<TMP_InputField>();

        // Save inputs
        string email = emailInput.text;
        string password = passwordInput.text;

        Debug.Log("Signing in user with email: " + email);
        yield return StartCoroutine(firebaseAuth.LogInUser(email, password));
    }
}
