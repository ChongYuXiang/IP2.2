/* Author: Chong Yu Xiang, Wang Johnathan Zhiwen
* Filename: FireBaseWebQuery
* Descriptions: Communication with firebase
*/

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

    // Database Ref
    private string apiKey = "AIzaSyAXIFk44UR-2siXFHZnh9iwsBydKid86hY"; // Replace with your Firebase API Key
    private string databaseURL = "https://ip-holohands-default-rtdb.asia-southeast1.firebasedatabase.app/"; // Replace with your Firebase Database URL

    // Auth Ref
    private string signUpUrl = "https://identitytoolkit.googleapis.com/v1/accounts:signUp?key=";
    private string signInUrl = "https://identitytoolkit.googleapis.com/v1/accounts:signInWithPassword?key=";

    // 
    public string idToken = ""; // Token received after authentication
    public string userId = "";  // Firebase UID (localId), used as the database key
    public GameObject SignUpPage;
    public GameObject LogInPage;

    // Feedback text
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

    // Firebase Sign UP
    public IEnumerator SignUpUser(string email, string password, string username, string gender, string age)
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

            yield return StartCoroutine(PostData(userId, email, username, gender, age)); // Create user data
            yield return StartCoroutine(GetData()); // Get user data
            // Close sign up page
            SignUpPage = GameObject.Find("Sign Up Page");
            SignUpPage.SetActive(false);
        }
        else
        {
            Debug.LogError("Sign up error: " + request.error);
            Debug.LogError("Response: " + request.downloadHandler.text);

            message.text = "Error: Check that email and password are input correctly"; // Display error message
        }
    }

    // Firebase Sign IN
    public IEnumerator LogInUser(string email, string password)
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

            yield return StartCoroutine(GetData()); // Get user data

            // Close sign up page
            LogInPage = GameObject.Find("Log In Page");
            LogInPage.SetActive(false);

            yield return StartCoroutine(UpdateStatus(userId, true)); // Wait until status changed
        }
        else
        {
            Debug.LogError("Sign in error: " + request.error);
            Debug.LogError("Response: " + request.downloadHandler.text);

            message.text = "Error: Check that email and password are input correctly"; // Display error message
        }
    }

    // Firebase READ
    public IEnumerator GetData()
    {
        string url = databaseURL + "players/" + userId + ".json?auth=" + idToken;
        UnityWebRequest request = UnityWebRequest.Get(url);

        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success)
        {
            // Parse the JSON response
            string jsonResponse = request.downloadHandler.text;
            Debug.Log("Data received: " + jsonResponse);

            // Parse the JSON response using Newtonsoft.Json
            JObject json = JObject.Parse(jsonResponse);

            // Access specific data values
            string email = json["email"]?.ToString(); // Safe check in case the key doesn't exist
            string username = json["username"]?.ToString();
            string gender = json["gender"]?.ToString();
            string age = json["age"]?.ToString();
            int date = (int)json["account_creation_date"];

            // Display the values
            TextMeshProUGUI displayName = GameObject.Find("DisplayName").GetComponent<TextMeshProUGUI>();
            displayName.text = "Welcome, " + username + "!";
            TextMeshProUGUI displayEmail = GameObject.Find("DisplayEmail").GetComponent<TextMeshProUGUI>();
            displayEmail.text = email;
            TextMeshProUGUI displayGender = GameObject.Find("DisplayGender").GetComponent<TextMeshProUGUI>();
            displayGender.text = "Gender: " + gender;
            TextMeshProUGUI displayAge = GameObject.Find("DisplayAge").GetComponent<TextMeshProUGUI>();
            displayAge.text = "Age: " + age;
            TextMeshProUGUI displayDate = GameObject.Find("DisplayDate").GetComponent<TextMeshProUGUI>();
            displayDate.text = "Account Made: " + DateTimeOffset.FromUnixTimeSeconds(date).DateTime;
        }
        else
        {
            Debug.LogError("Error fetching data: " + request.error);
        }
    }


    // Firebase CREATE player data
    public IEnumerator PostData(string userId, string email, string username, string gender, string age)
    {
        string url = databaseURL + "players/" + userId + ".json?auth=" + idToken;
        int timestamp = (int)DateTimeOffset.UtcNow.ToUnixTimeSeconds();

        Dictionary<string, object> userData = new Dictionary<string, object>
        {
            { "email", email },
            { "username", username },
            { "gender", gender },
            { "age", age },
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

    // Firebase CREATE player data for quiz modes
    public IEnumerator PostQuizData(string userId, string mode ,int score)
    {
        int timestamp = (int)DateTimeOffset.UtcNow.ToUnixTimeSeconds(); // Get current UTC time
        string url = databaseURL + mode + "/" + userId + timestamp.ToString() + ".json?auth=" + idToken;

        Dictionary<string, object> userData = new Dictionary<string, object>
        {
            { "score", score },
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
    
    // Firebase CREATE player data for screenshot
    public IEnumerator PostScreenshotData(string ssURL)
    {
        int timestamp = (int)DateTimeOffset.UtcNow.ToUnixTimeSeconds(); // Get current UTC time
        string url = databaseURL + "screenshots/" + userId + timestamp.ToString() + ".json?auth=" + idToken;

        Dictionary<string, object> userData = new Dictionary<string, object>
        {
            { "URL", ssURL },
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

    public IEnumerator SignOut()
    {
        yield return StartCoroutine(UpdateStatus(userId, false)); // Wait until status changed

        // Clear any user-specific data
        idToken = "";
        userId = "";
    }

    // Firebase UPDATE for active_status
    public IEnumerator UpdateStatus(string userId, bool active)
    {
        string url = databaseURL + "players/" + userId + ".json?auth=" + idToken;

        // Create a dictionary with the fields you want to update
        Dictionary<string, object> updatedData = new Dictionary<string, object>
        {
            { "active_status",  active}
        };

        // Convert dictionary to JSON
        string json = Newtonsoft.Json.JsonConvert.SerializeObject(updatedData);

        UnityWebRequest request = new UnityWebRequest(url, "PATCH");
        byte[] bodyRaw = Encoding.UTF8.GetBytes(json);
        request.uploadHandler = new UploadHandlerRaw(bodyRaw);
        request.downloadHandler = new DownloadHandlerBuffer();
        request.SetRequestHeader("Content-Type", "application/json");

        Debug.Log("Updating data to: " + url);

        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success)
        {
            Debug.Log("Data updated successfully: " + request.downloadHandler.text);
        }
        else
        {
            Debug.LogError("Error updating data: " + request.error);
            Debug.LogError("Response: " + request.downloadHandler.text);
            Debug.LogError("Response Code: " + request.responseCode); // Log the HTTP status code
        }
    }

    // Detect when the game is closed
    void OnApplicationQuit()
    {
        // Set active_status to false
        StartCoroutine(UpdateStatus(userId, false));
    }

    // Send forgot password email
    public IEnumerator SendPasswordResetEmail(string email)
    {
        message = GameObject.Find("Reset Feedback Text").GetComponent<TextMeshProUGUI>(); // Find error message display

        if (string.IsNullOrEmpty(email))
        {
            message.text = "Please enter an email address.";
            yield break;
        }

        string url = $"https://identitytoolkit.googleapis.com/v1/accounts:sendOobCode?key={apiKey}";

        // Prepare the JSON data
        string json = $"{{\"requestType\":\"PASSWORD_RESET\",\"email\":\"{email}\"}}";

        // Create the UnityWebRequest
        UnityWebRequest request = new UnityWebRequest(url, "POST");
        byte[] bodyRaw = Encoding.UTF8.GetBytes(json);
        request.uploadHandler = new UploadHandlerRaw(bodyRaw);
        request.downloadHandler = new DownloadHandlerBuffer();
        request.SetRequestHeader("Content-Type", "application/json");

        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success)
        {
            Debug.Log("Password reset email sent successfully.");
            message.text = "Email sent successfully"; // Display completion message
        }
        else
        {
            Debug.LogError("Error sending password reset email: " + request.error);
            message.text = "Error: Email not found"; // Display error message
        }
    }
}
