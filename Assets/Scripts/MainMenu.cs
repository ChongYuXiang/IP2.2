/* Author: Chong Yu Xiang  
 * Filename: MainMenu
 * Descriptions: Functions for main menu scene canvas
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{
    public Slider _VolumeSlider;

    // Tell audio manager to toggle BGM
    public void ToggleBGM()
    {
        AudioManager.instance.ToggleBGM();
    }

    // Tell audio manager to toggle SFX
    public void ToggleSFX()
    {
        AudioManager.instance.ToggleSFX();
    }

    // Tell audio manager to adjust master volume based on slider value
    public void Volume()
    {
        AudioManager.instance.Volume(_VolumeSlider.value);
    }

    private void Start()
    {
        // Play menu BGM on start
        AudioManager.instance.PlayBGM("Menu");
    }

    public void StopBGM()
    {
        // Stop playing menu BGM when changing scene
        AudioManager.instance.BGMSource.Stop();
    }

    // Quit Button
    public void QuitGame()
    {
        // Close unity build
        Application.Quit();
    }
}
