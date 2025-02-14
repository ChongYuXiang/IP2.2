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
    public DoorLock doorLock1;// Reference to DoorLock script
    public DoorLock doorLock2;// Reference to DoorLock script
    public DoorLock doorLock3;// Reference to DoorLock script



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
    public void WriteAlphaGameData(int score)
    {
        string timestamp = DateTimeOffset.UtcNow.ToUnixTimeSeconds().ToString();

        Quiz quiz = new Quiz(score);
        string json = JsonUtility.ToJson(quiz);
        dataRef.Child("alphabet_game").Child(uuid).Child(timestamp).SetRawJsonValueAsync(json);
        Debug.Log(json);
    }

    // Write number quiz results data under player's uuid
    public void WriteNumberGameData(int score)
    {
        string timestamp = DateTimeOffset.UtcNow.ToUnixTimeSeconds().ToString();

        Quiz numbergame = new Quiz(score);
        string json = JsonUtility.ToJson(numbergame);
        dataRef.Child("number_game").Child(uuid).Child(timestamp).SetRawJsonValueAsync(json);
        Debug.Log(json);
    }

    // Write word quiz results data under player's uuid
    public void WriteWordGameData(int score)
    {
        string timestamp = DateTimeOffset.UtcNow.ToUnixTimeSeconds().ToString();

        Quiz wordgame = new Quiz(score);
        string json = JsonUtility.ToJson(wordgame);
        dataRef.Child("word_game").Child(uuid).Child(timestamp).SetRawJsonValueAsync(json);
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
        // Find input objects
        inputEmail1 = GameObject.Find("Email Input 1").GetComponent<TMP_InputField>();
        inputPassword1 = GameObject.Find("Password Input 1").GetComponent<TMP_InputField>();

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
                errorText1 = GameObject.Find("Error Text 1").GetComponent<TextMeshProUGUI>(); // Find error object
                errorText1.text = "Error: Check that email and password are input correctly"; // Display error text
            }
            if (task.IsCompleted) // If lon in is successfull
            {
                AuthResult result = task.Result;
                Debug.LogFormat("User logged in successfully: {0} {1}", result.User.Email, result.User.UserId);

                errorText1 = GameObject.Find("Error Text 1").GetComponent<TextMeshProUGUI>(); // Find error object
                errorText1.text = ""; // Hide error text

                uuid = result.User.UserId; // Save uuid
                ReadPlayerData(); // Read to retrieve data
                doorLock1.SetLockState(false); // Unlock the door
                doorLock2.SetLockState(false); // Unlock the door
                doorLock3.SetLockState(false); // Unlock the door
                UpdateDoors();
            }
        });
    }

    public void SignUp()
    {
        // Find input objects
        inputName = GameObject.Find("Username Input").GetComponent<TMP_InputField>();
        inputEmail2 = GameObject.Find("Email Input 2").GetComponent<TMP_InputField>();
        inputPassword2 = GameObject.Find("Password Input 2").GetComponent<TMP_InputField>();
        inputGender = GameObject.Find("inputGender").GetComponent<TMP_Dropdown>();
        inputRace = GameObject.Find("inputRace").GetComponent<TMP_Dropdown>();

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
                errorText2 = GameObject.Find("Error Text 2").GetComponent<TextMeshProUGUI>(); // Find error object
                errorText2.text = "Error: Check that all fields have been input correctly"; // Display error text
            }
            else if (task.IsCompleted) // If account creation is successfull
            {
                AuthResult result = task.Result;
                Debug.LogFormat("Firebase user created successfully: {0} {1}", result.User.Email, result.User.UserId);

                errorText2 = GameObject.Find("Error Text 2").GetComponent<TextMeshProUGUI>(); // Find error object
                errorText2.text = ""; // Hide error text

                uuid = result.User.UserId; // Save uuid
                WriteNewPlayer(username, email, gender, race, true); // Write player with sign up data
                doorLock1.SetLockState(false); // Unlock the door
                doorLock2.SetLockState(false); // Unlock the door
                doorLock3.SetLockState(false); // Unlock the door
                UpdateDoors();
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

        // Reset user data
        email = null;
        uuid = null;
        username = null;
        gender = null; 
        race = null;

        auth.SignOut(); // Sign out user

        doorLock1.SetLockState(true); // Lock the door
        doorLock2.SetLockState(true); // Lock the door
        doorLock3.SetLockState(true); // Lock the door
        UpdateDoors();
        Debug.Log("Signed out");
    }

    // Detect when the game is closed
    void OnApplicationQuit()
    {
        // Set player's status to not active
        Dictionary<string, object> childUpdates = new Dictionary<string, object>();
        childUpdates[uuid + "/active_status"] = false;
        FirebaseDatabase.DefaultInstance.GetReference("players").UpdateChildrenAsync(childUpdates);
    }

    // For Forgot Password button
    public void forgotPassword()
    {
        inputEmailReset = GameObject.Find("Email Input Reset").GetComponent<TMP_InputField>(); // Find input field
        email = inputEmailReset.text; // Retrieve email from input field

        resetText = GameObject.Find("Feedback Text").GetComponent<TextMeshProUGUI>(); // Find reset feedback
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

    // Remove popups on doors in main scene
    public void UpdateDoors()
    {
        GameObject door;

        door = GameObject.Find("Quiz Mode Door"); // Select quiz door
        door.SendMessage("AuthChanged");

        door = GameObject.Find("Terrain Mode Door"); // Select terrain door
        door.SendMessage("AuthChanged");

        door = GameObject.Find("Learning Mode Door"); // Select learning door
        door.SendMessage("AuthChanged");
    }
}
