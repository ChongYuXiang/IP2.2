using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System;
using Newtonsoft.Json.Linq;
using TMPro; // Install Newtonsoft.Json via NuGet or Unity Package Manager

public class FirebaseWebQuery : MonoBehaviour
{
    // Instance
    public static FirebaseWebQuery instance;

    private string apiKey = "AIzaSyAXIFk44UR-2siXFHZnh9iwsBydKid86hY"; // Replace with your Firebase API Key
    private string databaseURL = "https://ip-holohands-default-rtdb.asia-southeast1.firebasedatabase.app/"; // Replace with your Firebase Database URL

    private string signUpUrl = "https://identitytoolkit.googleapis.com/v1/accounts:signUp?key=";
    private string signInUrl = "https://identitytoolkit.googleapis.com/v1/accounts:signInWithPassword?key=";

    public string idToken = ""; // Token received after authentication
    public string userId = "";  // Firebase UID (localId), used as the database key

    public TextMeshProUGUI message;

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
    }

    void Start()
    {
        // Example Usage:
        // StartCoroutine(SignUpUser("Test@gmail.com", "Test123"));
        // StartCoroutine(SignInUser("Test@gmail.com", "Test123"));
    }

    /// <summary>
    /// Sign up a new user with email and password.
    /// </summary>
    public IEnumerator SignUpUser(string email, string password, string username, string gender, string race)
    {
        message = GameObject.Find("Error Text 2").GetComponent<TextMeshProUGUI>(); // Find error message display

        if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password) || string.IsNullOrEmpty(username)) // If user didnt fill in all inputs
        {
            message.text = "Please fill in all the boxes"; // Display error message
            yield break;
        }

        string json = $"{{\"email\":\"{email}\",\"password\":\"{password}\",\"returnSecureToken\":true}}";
        UnityWebRequest request = new UnityWebRequest(signUpUrl + apiKey, "POST");
        byte[] bodyRaw = Encoding.UTF8.GetBytes(json);
        request.uploadHandler = new UploadHandlerRaw(bodyRaw);
        request.downloadHandler = new DownloadHandlerBuffer();
        request.SetRequestHeader("Content-Type", "application/json");

        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success)
        {
            string response = request.downloadHandler.text;
            JObject responseData = JObject.Parse(response);
            idToken = responseData["idToken"].ToString();
            userId = responseData["localId"].ToString(); // Store Firebase UID

            Debug.Log("User signed up successfully. UID: " + userId);
            message.text = ""; // Empty error message

            yield return StartCoroutine(PostData(userId, email ,username, gender, race)); // Create user data
        }
        else
        {
            Debug.LogError("Sign up error: " + request.error);
            Debug.LogError("Response: " + request.downloadHandler.text);

            message.text = "Error: Check that email and password are input correctly"; // Display error message
        }
    }

    /// <summary>
    /// Sign in an existing user and get an authentication token.
    /// </summary>
    public IEnumerator SignInUser(string email, string password)
    {
        message = GameObject.Find("Error Text 1").GetComponent<TextMeshProUGUI>(); // Find error message display

        if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password)) // If user didnt fill in all inputs
        {
            message.text = "Please fill in all the boxes"; // Display error message
            yield break;
        }

        string json = $"{{\"email\":\"{email}\",\"password\":\"{password}\",\"returnSecureToken\":true}}";
        UnityWebRequest request = new UnityWebRequest(signInUrl + apiKey, "POST");
        byte[] bodyRaw = Encoding.UTF8.GetBytes(json);
        request.uploadHandler = new UploadHandlerRaw(bodyRaw);
        request.downloadHandler = new DownloadHandlerBuffer();
        request.SetRequestHeader("Content-Type", "application/json");

        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success)
        {
            string response = request.downloadHandler.text;
            JObject responseData = JObject.Parse(response);
            idToken = responseData["idToken"].ToString();
            userId = responseData["localId"].ToString(); // Store Firebase UID

            Debug.Log("User signed in successfully. UID: " + userId);
            message.text = ""; // Empty error message
        }
        else
        {
            Debug.LogError("Sign in error: " + request.error);
            Debug.LogError("Response: " + request.downloadHandler.text);

            message.text = "Error: Check that email and password are input correctly"; // Display error message
        }
    }

    /// <summary>
    /// Send authenticated GET request to Firebase Database.
    /// </summary>
    public IEnumerator GetData()
    {
        string url = databaseURL + "users.json?auth=" + idToken;
        UnityWebRequest request = UnityWebRequest.Get(url);

        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success)
        {
            Debug.Log("Data received: " + request.downloadHandler.text);
        }
        else
        {
            Debug.LogError("Error fetching data: " + request.error);
        }
    }

    /// <summary>
    /// Send authenticated POST request to Firebase Database.
    /// </summary>
    public IEnumerator PostData(string userId, string email, string username, string gender, string race)
    {
        string url = databaseURL + "players/" + userId + ".json?auth=" + idToken;
        int timestamp = (int)DateTimeOffset.UtcNow.ToUnixTimeSeconds();

        Dictionary<string, object> userData = new Dictionary<string, object>
        {
            { "email", email },
            { "username", username },
            { "gender", gender },
            { "race", race },
            { "active_status", true },
            { "account_creation_date", timestamp },
            { "last_logged_in_time", timestamp },
        };

        string json = Newtonsoft.Json.JsonConvert.SerializeObject(userData);
        UnityWebRequest request = new UnityWebRequest(url, "PUT");
        byte[] bodyRaw = Encoding.UTF8.GetBytes(json);
        request.uploadHandler = new UploadHandlerRaw(bodyRaw);
        request.downloadHandler = new DownloadHandlerBuffer();
        request.SetRequestHeader("Content-Type", "application/json");

        Debug.Log("Posting data to: " + url);
        Debug.Log("Posting to path: " + "players/" + userId + ".json");


        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success)
        {
            Debug.Log("Data posted successfully: " + request.downloadHandler.text);
        }
        else
        {
            Debug.LogError("Error posting data: " + request.error);
            Debug.LogError("Response: " + request.downloadHandler.text);
            Debug.LogError("Response Code: " + request.responseCode); // Log the HTTP status code
        }
    }
}
