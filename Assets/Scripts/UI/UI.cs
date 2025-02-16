/* Author: Sung Yeji
* Date: 02/02/2025
* Descriptions: UI buttons
*/

using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UI : MonoBehaviour
{   /// <summary>
    /// The index of the scene to be loaded.
    /// </summary>
    public int sceneIndex;

    /// <summary>
    /// The title text of the view setting.
    /// </summary>
    public TextMeshProUGUI titleOfViewSetting;

    /// <summary>
    /// The description text of the view setting.
    /// </summary>
    public TextMeshProUGUI descOfViewSetting;

    /// <summary>
    /// Indicates whether passthrough mode is enabled.
    /// </summary>
    bool passthroughMode = false;

    public static UI instance;

    // Change View Mode Buotton Ref
    public Button viewSetting_Button;

    // Change scene by index
    public void ChangeSceneByIndex(int sceneIndex)
    {
        SceneManager.LoadScene(sceneIndex);

        if (sceneIndex == 1)
        {
            Database.instance.LinkObjects();
        }
    }

    /// <summary>
    /// Initializes the UI and sets the interactability of the view setting button.
    /// </summary>
    private void Awake()
    {
        string currentScene = SceneManager.GetActiveScene().name;


        if (currentScene != "Living Room")
        {
            viewSetting_Button.interactable = true;
            Debug.Log("view setting button is interactive");
        }
        else
        {
            viewSetting_Button.interactable = false;
            Debug.Log("view setting button is not interactive");
            descOfViewSetting.text = "Enter a game mode to activate";
        }
    }

    /// <summary>
    /// Updates the UI text based on the current view mode.
    /// </summary>
    private void Update()
    {
        if (passthroughMode == false)
        {
            titleOfViewSetting.text = "Passthrough Mode";
            descOfViewSetting.text = "Change view to Quest 3 Camera";
        }
        else
        {
            titleOfViewSetting.text = "Virtual Room Mode";
            descOfViewSetting.text = "Change view to 3D Virtual Room";
        }
    }

    /// <summary>
    /// Toggles the view setting between passthrough mode and virtual room mode.
    /// </summary>
    public void viewSettingButton()
    {
        if(passthroughMode == false)
        {
            passthroughMode = true;
        }
        else
        {
            passthroughMode = false;
        }
    }
}
