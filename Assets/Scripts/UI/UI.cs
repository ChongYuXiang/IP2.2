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
{
    public int sceneIndex;
    public TextMeshProUGUI titleOfViewSetting;
    public TextMeshProUGUI descOfViewSetting;
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
