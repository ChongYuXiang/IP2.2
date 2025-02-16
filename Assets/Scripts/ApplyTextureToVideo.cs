/* Author: Chong Yu Xiang  
 * Filename: ApplyTextureToVideo
 * Descriptions: Apply texture on UI image from supabase URL
 */

using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;
using System;
using System.Collections;
using TMPro;

public class ApplyTextureToVideo : MonoBehaviour
{
    public string videoUrl = ""; // Replace with your video URL
    public RawImage targetRawImage; // Assign the RawImage component here
    public VideoPlayer videoPlayer; // Assign the VideoPlayer component here

    public TextMeshPro currentObject;
    private string currentURL;

    public void Start()
    {
        if (videoUrl != "")
        {
            Display(); // Displays the video
        }
    }

    public void ChangeDisplay(string newUrl) // Call to switch with a new video URL
    {
        videoUrl = "https://mrjzpnoiqdnifempamof.supabase.co/storage/v1/object/public/images/signs/" + newUrl + ".mp4"; // Modify for your video URL pattern
        Display(); // Update displayed video
    }

    private void Display()
    {
        if (targetRawImage == null || videoPlayer == null)
        {
            Debug.LogError("Target RawImage or VideoPlayer is not assigned.");
            return;
        }

        Debug.Log("Fetching video for Panel...");
        try
        {
            StartCoroutine(LoadAndPlayVideo(videoUrl));
        }
        catch (Exception e)
        {
            Debug.LogError($"Error loading video: {e.Message}");
        }
    }

    private IEnumerator LoadAndPlayVideo(string url)
    {
        videoPlayer.url = url;

        // Start preparing the video
        videoPlayer.Prepare();

        // Wait until the video is prepared
        while (!videoPlayer.isPrepared)
        {
            yield return null; // Wait until the next frame
        }

        if (videoPlayer.isPrepared)
        {
            // Set the video texture to the RawImage and start playing the video
            targetRawImage.texture = videoPlayer.texture;
            videoPlayer.Play();
            Debug.Log("Video is playing on the panel.");
        }
        else
        {
            Debug.LogError("Failed to load the video.");
        }
    }

    private void Update() // check every frame if word selected changed
    {
        if (currentObject.text != "" && currentURL != currentObject.text)
        {
            currentURL = currentObject.text;
            targetRawImage.gameObject.SetActive(true);
            ChangeDisplay(currentObject.text); // Display the tutorial for the selected word
        }
        else if (currentObject.text == "" && currentURL != currentObject.text)
        {
            currentURL = currentObject.text;
            targetRawImage.gameObject.SetActive(false);
        }
    }
}