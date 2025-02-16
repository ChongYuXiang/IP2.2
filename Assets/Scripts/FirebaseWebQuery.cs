using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json.Linq; // Install Newtonsoft.Json via NuGet or Unity Package Manager

public class FirebaseWebQuery : MonoBehaviour
{
    private string apiKey = "AIzaSyAXIFk44UR-2siXFHZnh9iwsBydKid86hY"; // Replace with your Firebase API Key
    private string databaseURL = "https://ip-holohands-default-rtdb.asia-southeast1.firebasedatabase.app/"; // Replace with your Firebase Database URL

    private string signUpUrl = "https://identitytoolkit.googleapis.com/v1/accounts:signUp?key=";
    private string signInUrl = "https://identitytoolkit.googleapis.com/v1/accounts:signInWithPassword?key=";

    public string idToken = ""; // Token received after authentication
    public string userId = "";  // Firebase UID (localId), used as the database key

    void Start()
    {
        // Example Usage:
        // StartCoroutine(SignUpUser("Test@gmail.com", "Test123"));
        // StartCoroutine(SignInUser("Test@gmail.com", "Test123"));
    }

    /// <summary>
    /// Sign up a new user with email and password.
    /// </summary>
    public IEnumerator SignUpUser(string email, string password)
    {
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
            yield return StartCoroutine(PostData(userId, "testName", 10));
        }
        else
        {
            Debug.LogError("Sign up error: " + request.error);
            Debug.LogError("Response: " + request.downloadHandler.text);
        }
    }

    /// <summary>
    /// Sign in an existing user and get an authentication token.
    /// </summary>
    public IEnumerator SignInUser(string email, string password)
    {
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
        }
        else
        {
            Debug.LogError("Sign in error: " + request.error);
            Debug.LogError("Response: " + request.downloadHandler.text);
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
    public IEnumerator PostData(string userId, string name, int score)
    {
        string url = databaseURL + "players/" + userId + ".json?auth=" + idToken;

        Dictionary<string, object> userData = new Dictionary<string, object>
        {
            { "name", name },
            { "score", score }
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
