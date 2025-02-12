/* Author: Chong Yu Xiang  
 * Filename: ApplyTextureToPanel
 * Descriptions: Apply texture on UI image from supabase URL
 */

using System;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class ApplyTextureToPanel : MonoBehaviour
{
    public string imageUrl = ""; // Replace with your image URL
    public Image targetImage; // Assign the Panel's Image component here

    public void Start()
    {
        Display(); // Displays default image
    }

    public void ChangeDisplay(string newUrl) // Call to switch with a new URL
    {
        imageUrl = newUrl; // Switch to new given URL
        Display(); // Update displayed image
    }

    private async void Display()
    {
        if (targetImage == null)
        {
            Debug.LogError("Target Image is not assigned.");
            return;
        }

        Debug.Log("Fetching texture for Panel...");
        try
        {
            Texture2D texture = await GetTextureFromURL(imageUrl);

            if (texture != null)
            {
                // Convert the texture to a Sprite and apply it to the Image component
                Sprite sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f));
                targetImage.sprite = sprite;
                Debug.Log("Texture applied successfully to Panel.");
            }
            else
            {
                Debug.LogError("Failed to load texture.");
            }
        }
        catch (Exception e)
        {
            Debug.LogError($"Error downloading texture: {e.Message}");
        }
    }

    private async Task<Texture2D> GetTextureFromURL(string url)
    {
        using (UnityWebRequest request = UnityWebRequestTexture.GetTexture(url))
        {
            var asyncOperation = request.SendWebRequest();

            while (!asyncOperation.isDone)
            {
                await Task.Yield();
            }

            if (request.result == UnityWebRequest.Result.Success)
            {
                return DownloadHandlerTexture.GetContent(request);
            }
            else
            {
                Debug.LogError($"Error in UnityWebRequest: {request.error}");
                return null;
            }
        }
    }
}