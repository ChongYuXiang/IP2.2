/* Author: Chong Yu Xiang  
 * Filename: ScreenshotToSupabase
 * Descriptions: For taking screenshot and sending to supabase
 */

using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.IO;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UIElements;

public class ScreenshotToSupabase : MonoBehaviour
{
    public string supabaseUrl = "https://mrjzpnoiqdnifempamof.supabase.co"; //Supabase URL
    public string supabaseAnonKey = "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJpc3MiOiJzdXBhYmFzZSIsInJlZiI6Im1yanpwbm9pcWRuaWZlbXBhbW9mIiwicm9sZSI6ImFub24iLCJpYXQiOjE3Mzc5NDYxMzEsImV4cCI6MjA1MzUyMjEzMX0.39LpnJhxYgOT6CbHPzj6tfbimiwmjmEiz6MfkUVWpPE"; //Supabase Anon Key
    public string bucketName = "images"; // Bucket name

    // Database object ref
    public GameObject database;

    private string screenshotsFolder;
    private string filePath;

    //settings
    private const string TerrainScreenShotsFolder = "ScreenShots";
    private const string UploadType = "image/png";
    private const string UploadFolder = "screenshots";

    private void Start()
    {
        screenshotsFolder = Path.Combine(Application.persistentDataPath, TerrainScreenShotsFolder);
        if (!Directory.Exists(screenshotsFolder))
        {
            Directory.CreateDirectory(screenshotsFolder);
            Debug.Log($"Created folder: {screenshotsFolder}");
        }
    }

    public async void capture()
    {
        string fileName = $"Screenshot_{DateTime.Now:yyyy-MM-dd_HH-mm-ss}.png";
        filePath = Path.Combine(screenshotsFolder, fileName);

        await screenshot();

        Debug.Log(filePath);
        if (File.Exists(filePath))
        {
            await UploadFileUsingPost(filePath);
        }
        else
        {
            Debug.LogError("Failed to save the screenshot.");
        }
    }

    public async Task screenshot()
    {
        ScreenCapture.CaptureScreenshot(filePath, 2);

        while (!File.Exists(filePath))
        {
            await Task.Yield();
        }
    }

    public void SendToFirebase(string URL)
    {
        database = GameObject.Find("Database");

        database.SendMessage("WriteScreenshotURL", URL);
    }

    public async Task UploadFileUsingPost(string filePath)
    {
        if (!File.Exists(filePath))
        {
            Debug.LogError($"File does not exist: {filePath}");
            return;
        }

        byte[] fileData = File.ReadAllBytes(filePath);
        string fileName = Path.GetFileName(filePath);

        //although we have public in the url path, it is excluded in this context
        //https://<yoursupabaseURL/storage/v1/object/<your_bucket>/<your_folder>/<specified_filename_to_save>
        string uploadUrl = $"{supabaseUrl}/storage/v1/object/{bucketName}/{UploadFolder}/{fileName}";

        Debug.Log($"Uploading to URL: {uploadUrl}");

        try
        {

            // Create a multipart form to upload the file
            WWWForm form = new WWWForm();
            form.AddBinaryData("file", fileData, fileName, UploadType);

            using (UnityWebRequest request = UnityWebRequest.Post(uploadUrl, form))
            {
                // Set required headers
                request.SetRequestHeader("Authorization", $"Bearer {supabaseAnonKey}");

                // Send the request
                var operation = request.SendWebRequest();

                while (!operation.isDone)
                {
                    await Task.Yield();
                }

                if (request.result == UnityWebRequest.Result.Success)
                {
                    Debug.Log($"File uploaded successfully \u2705: {fileName}");
                    SendToFirebase(fileName);
                }
                else
                {
                    Debug.LogError($"Upload failed: {request.error}");
                    Debug.LogError($"Response: {request.downloadHandler.text}");
                }
            }
        }
        catch (Exception ex)
        {
            Debug.LogError($"Error uploading file: {ex.Message}");
        }
    }
}
