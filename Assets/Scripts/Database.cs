/* Author: Chong Yu Xiang  
 * Filename: Database
 * Descriptions: Communicate with firebase database and auth
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Firebase;
using Firebase.Database;
using Firebase.Extensions;
using TMPro;
using UnityEngine.SocialPlatforms.Impl;
using System.Threading.Tasks;
using Firebase.Auth;
using UnityEngine.SceneManagement;
using UnityEngine.Analytics;
using UnityEngine.UI;
using System;
using System.Net.Mail;

public class Database : MonoBehaviour
{
    DatabaseReference dataRef;
    FirebaseAuth auth;

    // User data
    private string email;
    private string password;
    public string uuid;
    public string username;
    public string gender;
    public string race;

    // Input fields
    [SerializeField]
    private TMP_InputField inputName;
    [SerializeField]
    private TMP_InputField inputEmail1;
    [SerializeField]
    private TMP_InputField inputPassword1;
    [SerializeField]
    private TMP_InputField inputEmail2;
    [SerializeField]
    private TMP_InputField inputPassword2;
    [SerializeField]
    private TMP_InputField inputEmailReset;
    [SerializeField]
    private TMP_Dropdown inputGender;
    [SerializeField]
    private TMP_Dropdown inputRace;

    // UI Objects
    public TextMeshProUGUI errorText1;
    public TextMeshProUGUI errorText2;
    public TextMeshProUGUI resetText;

    // Instance
    public static Database instance;

    private void Awake()
    {
        // Dont destroy on load
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else if (instance != null && instance != this)
        {
            Destroy(gameObject);
        }

        // Set up dataRef
        dataRef = FirebaseDatabase.DefaultInstance.RootReference;
        auth = FirebaseAuth.DefaultInstance;

        //ReadPlayerData();
    }

    // Create player data
    public void WriteNewPlayer(string username, string email, string gender, string race, bool active_status)
    {
        string timestamp = DateTimeOffset.UtcNow.ToUnixTimeSeconds().ToString();

        Player player = new Player(username, email, gender, race, active_status, timestamp, timestamp);
        string json = JsonUtility.ToJson(player);
        dataRef.Child("players").Child(uuid).SetRawJsonValueAsync(json);
        Debug.Log(json);
        ReadPlayerData();
    }

    // Write alphabet quiz results data under player's uuid
    public void WriteAlphaGameData(int correct, int wrong, int time_taken, int average_time_per_letter)
    {
        string timestamp = DateTimeOffset.UtcNow.ToUnixTimeSeconds().ToString();

        AlphabetGame alphabetgame = new AlphabetGame(correct, wrong, time_taken, average_time_per_letter);
        string json = JsonUtility.ToJson(alphabetgame);
        dataRef.Child("alphabet_game").Child(uuid).Child(timestamp).SetRawJsonValueAsync(json);
        Debug.Log(json);
    }

    // Write number quiz results data under player's uuid
    public void WriteNumberGameData(int correct, int wrong, int time_taken, int average_time_per_number)
    {
        string timestamp = DateTimeOffset.UtcNow.ToUnixTimeSeconds().ToString();

        NumberGame numbergame = new NumberGame(correct, wrong, time_taken, average_time_per_number);
        string json = JsonUtility.ToJson(numbergame);
        dataRef.Child("number_game").Child(uuid).Child(timestamp).SetRawJsonValueAsync(json);
        Debug.Log(json);
    }

    // Write word quiz results data under player's uuid
    public void WriteWordGameData(int correct, int wrong, int time_taken, int average_time_per_word)
    {
        string timestamp = DateTimeOffset.UtcNow.ToUnixTimeSeconds().ToString();

        WordGame wordgame = new WordGame(correct, wrong, time_taken, average_time_per_word);
        string json = JsonUtility.ToJson(wordgame);
        dataRef.Child("number_game").Child(uuid).Child(timestamp).SetRawJsonValueAsync(json);
        Debug.Log(json);
    }

    // Write screenshot url under player's uuid
    public void WriteScreenshotURL(string URL)
    {
        string timestamp = DateTimeOffset.UtcNow.ToUnixTimeSeconds().ToString();

        ScreenshotURL screenshot = new ScreenshotURL(URL);
        string json = JsonUtility.ToJson(screenshot);
        dataRef.Child("screenshots").Child(uuid).Child(timestamp).SetRawJsonValueAsync(json);
        Debug.Log(json);
    }

    // Read player data
    public void ReadPlayerData()
    {
        FirebaseDatabase.DefaultInstance.GetReference("players/" + uuid).GetValueAsync().ContinueWithOnMainThread(task =>
        {
            if (task.IsFaulted) // If read fails
            {
                Debug.Log("error");
                return;
            }
            else if (task.IsCompleted) // If read successfull
            {
                DataSnapshot snapshot = task.Result; // Take snapshot of data

                string playerDetails = snapshot.GetRawJsonValue(); // Convert snapshot to json
                Debug.Log("raw json data of player" + playerDetails);

                Player p = JsonUtility.FromJson<Player>(playerDetails);
                // Save data
                username = p.username;
                email = p.email;
                gender = p.gender;
                Debug.LogFormat("Player data of {0}: email:{1} gender:{2} race:{3} status:{4}", username, email, gender, race, p.active_status);
            }
        });
    }

    public void Login()
    {
        // Retrieve input values
        email = inputEmail1.text;
        password = inputPassword1.text;
        Debug.LogFormat("Log in values: {0} {1}", email, password);

        // Reset input fields
        inputEmail1.text = "";
        inputPassword1.text = "";

        auth.SignInWithEmailAndPasswordAsync(email, password).ContinueWithOnMainThread(task =>
        {
            if (task.IsFaulted) // If log in fails
            {
                Debug.Log("SignInilAndPasswordAsync error " + task.Exception);
                errorText1.text = "Error: Check that email and password are input correctly"; // Display error text
            }
            if (task.IsCompleted) // If lon in is successfull
            {
                AuthResult result = task.Result;
                Debug.LogFormat("User logged in successfully: {0} {1}", result.User.Email, result.User.UserId);
                errorText1.text = ""; // Hide error text
                uuid = result.User.UserId; // Save uuid
                ReadPlayerData(); // Read to retrieve data
            }
        });
    }

    public void SignUp()
    {
        // Retrieve input values
        username = inputName.text;
        email = inputEmail2.text;
        password = inputPassword2.text;
        gender = inputGender.options[inputGender.value].text;
        race = inputRace.options[inputRace.value].text;
        Debug.LogFormat("Sign in values: {0} {1} {2} {3} {4}", username, email, password, gender, race);

        // Reset input fields
        inputName.text = "";
        inputEmail2.text = "";
        inputPassword2.text = "";

        auth.CreateUserWithEmailAndPasswordAsync(email, password).ContinueWithOnMainThread(task =>
        {
            if (task.IsFaulted) // If account creation failed
            {
                Debug.LogFormat("CreateUserWithEmailAndPasswordAsync error " + task.Exception);
                errorText2.text = "Error: Check that all fields have been input correctly"; // Display error text
            }
            else if (task.IsCompleted) // If account creation is successfull
            {
                AuthResult result = task.Result;
                Debug.LogFormat("Firebase user created successfully: {0} {1}", result.User.Email, result.User.UserId);
                errorText2.text = ""; // Hide error text
                uuid = result.User.UserId; // Save uuid
                WriteNewPlayer(username, email, gender, race, true); // Write player with sign up data
            }
        });
    }

    // Sign out function
    public void SignOutPlayer()
    {
        // Set player's status to not active
        Dictionary<string, object> childUpdates = new Dictionary<string, object>();
        childUpdates[uuid + "/active_status"] = false;
        FirebaseDatabase.DefaultInstance.GetReference("players").UpdateChildrenAsync(childUpdates);

        auth.SignOut(); // Sign out user

        Debug.Log("Signed out");
    }

    // When game is closed
    void OnApplicationQuit()
    {
        // Set player's status to not active
        Dictionary<string, object> childUpdates = new Dictionary<string, object>();
        childUpdates[uuid + "/active_status"] = false;
        FirebaseDatabase.DefaultInstance.GetReference("players").UpdateChildrenAsync(childUpdates);
    }

    public void forgotPassword()
    {
        email = inputEmailReset.text; // Retrieve email from input field

        auth.SendPasswordResetEmailAsync(email).ContinueWith(task => {
            if (task.IsFaulted) // If email cannot be sent
            {
                Debug.LogError("SendPasswordResetEmailAsync encountered an error: " + task.Exception);
                resetText.text = "Error: Email not found"; // Display error text
            }
            else if (task.IsCompleted) // If email is sent successfully
            {
                Debug.Log("Password reset email sent successfully");
                resetText.text = "Email sent successfully"; // Display confirmation text
            }
        });
    }
}
