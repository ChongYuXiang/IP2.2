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
    public long score;
    public long highscore;
    public string username;

    // Input fields
    [SerializeField]
    private TMP_InputField inputEmail;
    [SerializeField]
    private TMP_InputField inputPassword;

    // UI Objects
    public GameObject errorText;

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
    public void WriteNewPlayer(int score, int highscore, string username)
    {
        Player player = new Player(score, highscore, username);
        string json = JsonUtility.ToJson(player);
        dataRef.Child("players").Child(uuid).SetRawJsonValueAsync(json);
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
                score = p.score;
                highscore = p.highscore;
                username = p.username;
                Debug.LogFormat("Player data of {0}: score:{1} highscore:{2}", username, score, highscore);
            }
        });
    }

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


    public void Login()
    {
        // Retrieve input values
        email = inputEmail.text;
        password = inputPassword.text;
        Debug.LogFormat("Sign in values: {0} {1}", email, password);

        // Reset input fields
        inputEmail.text = "";
        inputPassword.text = "";

        auth.SignInWithEmailAndPasswordAsync(email, password).ContinueWithOnMainThread(task =>
        {
            if (task.IsFaulted)
            {
                Debug.Log("SignInilAndPasswordAsync error " + task.Exception);
                errorText.SetActive(true);
            }
            if (task.IsCompleted)
            {
                AuthResult result = task.Result;
                Debug.LogFormat("User logged in successfully: {0} {1}", result.User.Email, result.User.UserId);
                errorText.SetActive(false);
                uuid = result.User.UserId;
                //ReadPlayerData();
            }
        });
    }

    public void SignUp()
    {
        // Retrieve input values
        email = inputEmail.text;
        password = inputPassword.text;
        Debug.LogFormat("Sign up values: {0} {1}", email, password);

        // Reset input fields
        inputEmail.text = "";
        inputPassword.text = "";

        auth.CreateUserWithEmailAndPasswordAsync(email, password).ContinueWithOnMainThread(task =>
        {
            if (task.IsFaulted)
            {
                Debug.LogFormat("CreateUserWithEmailAndPasswordAsync error " + task.Exception);
                errorText.SetActive(true);
            }
            else if (task.IsCompleted)
            {
                AuthResult result = task.Result;
                Debug.LogFormat("Firebase user created successfully: {0} {1}", result.User.Email, result.User.UserId);
                errorText.SetActive(false);
                uuid = result.User.UserId;
                //WriteNewPlayer(0, 0, username);
            }
        });
    }

    public void SignOutPlayer()
    {
        auth.SignOut();
        Debug.Log("Signed out");
    }
}
