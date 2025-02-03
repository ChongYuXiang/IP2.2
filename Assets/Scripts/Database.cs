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

    [SerializeField]
    private GameObject UiManager;

    private string email;
    private string password;
    private string uuid;

    // User data
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

        AlphabetGame alphabetgame = new AlphabetGame(0, 0, 0, 0, 0);
        json = JsonUtility.ToJson(alphabetgame);
        dataRef.Child("alphabet_game").Child(uuid).Child(timestamp).SetRawJsonValueAsync(json);
        Debug.Log(json);

        WordGame wordgame = new WordGame(0, 0, 0, 0, 0);
        json = JsonUtility.ToJson(wordgame);
        dataRef.Child("word_game").Child(uuid).Child(timestamp).SetRawJsonValueAsync(json);
        Debug.Log(json);

        ReadPlayerData();
    }

    // Read player data
    public void ReadPlayerData()
    {
        FirebaseDatabase.DefaultInstance.GetReference("players/" + uuid).GetValueAsync().ContinueWithOnMainThread(task =>
        {
            if (task.IsFaulted)
            {
                Debug.Log("error");
                return;
            }
            else if (task.IsCompleted)
            {
                DataSnapshot snapshot = task.Result;

                string playerDetails = snapshot.GetRawJsonValue();
                Debug.Log("raw json data of player" + playerDetails);

                Player p = JsonUtility.FromJson<Player>(playerDetails);
                username = p.username;
                email = p.email;
                gender = p.gender;
                Debug.LogFormat("Player data of {0}: email:{1} gender:{2} race:{3} status:{4}", username, email, gender, race, p.active_status);
            }
        });
    }

    /*
    public void IncreaseScore(int scoreChange)
    {
        // Increase score when fruit is clicked

        int newScore = (int)score + scoreChange;
        int newHighscore = (int)highscore;

        if ((int)highscore <= (int)score)
        {
            Debug.Log("new highscore!");
            newHighscore = newScore;
        }
        Dictionary<string, object> childUpdates = new Dictionary<string, object>();
        childUpdates[uuid + "/score"] = newScore;
        childUpdates[uuid + "/highscore"] = newHighscore;
        FirebaseDatabase.DefaultInstance.GetReference("players").UpdateChildrenAsync(childUpdates);
        ReadPlayerData();
    }
    */


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
            if (task.IsFaulted)
            {
                Debug.Log("SignInilAndPasswordAsync error " + task.Exception);
                errorText1.text = "Error: Check that email and password are input correctly";
            }
            if (task.IsCompleted)
            {
                AuthResult result = task.Result;
                Debug.LogFormat("User logged in successfully: {0} {1}", result.User.Email, result.User.UserId);
                errorText1.text = "";
                uuid = result.User.UserId;
                ReadPlayerData();
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
            if (task.IsFaulted)
            {
                Debug.LogFormat("CreateUserWithEmailAndPasswordAsync error " + task.Exception);
                errorText2.text = "Error: Check that all fields have been input correctly";
            }
            else if (task.IsCompleted)
            {
                AuthResult result = task.Result;
                Debug.LogFormat("Firebase user created successfully: {0} {1}", result.User.Email, result.User.UserId);
                errorText2.text = "";
                uuid = result.User.UserId;
                WriteNewPlayer(username, email, gender, race, true);
            }
        });
    }

    public void SignOutPlayer()
    {
        auth.SignOut();
        Debug.Log("Signed out");
    }

    public void forgotPassword()
    {
        email = inputEmailReset.text;

        auth.SendPasswordResetEmailAsync(email).ContinueWith(task => {
            if (task.IsCanceled)
            {
                Debug.LogError("SendPasswordResetEmailAsync was canceled.");
                resetText.text = "Error: Email not found";
            }
            else if (task.IsFaulted)
            {
                Debug.LogError("SendPasswordResetEmailAsync encountered an error: " + task.Exception);
                resetText.text = "Email sent successfully";
            }

            Debug.Log("Password reset email sent successfully");
        });
    }
}
