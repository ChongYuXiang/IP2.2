/* Author: Coleman Lim
* Filename: QuitButton
* Descriptions: Quit to main menu
*/



using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class QuitButton : MonoBehaviour
{
    //Change Scene based on name
    public void ChangeSceneByIndex(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }
}
