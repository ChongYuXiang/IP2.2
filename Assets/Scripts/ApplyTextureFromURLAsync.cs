using System;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;

public class ApplyTextureFromURLAsync : MonoBehaviour
{
    public string imageUrl = "https://sjomccxrprhjgvwxvcdp.supabase.co/storage/v1/object/public/images/04_chongYuXiang_ASG2_UVunwrap.PNG"; // Replace with your image URL
    public Renderer targetRenderer; // The renderer to which the texture will be applied

    // Start the process to download and apply the texture
    private async void Start()
    {
        Debug.Log("App started");
        if (targetRenderer == null)
        {
            Debug.LogError("Target Renderer is not assigned.");
            return;
        }

        try
        {
            Texture2D texture = await GetTextureFromURL(imageUrl);
            if (texture != null)
            {
                targetRenderer.material.mainTexture = texture;
                Debug.Log("Texture applied successfully at game start.");
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

    private async void DownloadAndApplyTexture(string url)
    {
        try
        {
            Texture2D texture = await GetTextureFromURL(url);

            if (texture != null)
            {
                if (targetRenderer != null)
                {
                    targetRenderer.material.mainTexture = texture;
                    Debug.Log("Texture applied successfully.");
                }
                else
                {
                    Debug.LogError("Target Renderer is not assigned.");
                }
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
                await Task.Yield(); // Yield until the operation is complete
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