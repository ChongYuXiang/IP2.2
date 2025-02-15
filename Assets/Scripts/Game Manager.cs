/* Author: Chong Yu Xiang  
 * Filename: Game Manager
 * Descriptions: Save player data between scenes
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    // Instance
    public static GameManager instance;

    public bool terrainTask1 = false;
    public bool terrainTask2 = false;
    public bool terrainTask3 = false;
    public bool terrainTask4 = false;
    public bool terrainTask5 = false;

    public bool numbersUnlocked = false;
    public bool wordsUnlocked = false;

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

    public void TaskDone(int index)
    {
        if (index == 1)
        {
            terrainTask1 = true;
        }
        if (index == 2)
        {
            terrainTask2 = true;
        }
        if (index == 3)
        {
            terrainTask3 = true;
        }
        if (index == 4)
        {
            terrainTask4 = true;
        }
        if (index == 5)
        {
            terrainTask5 = true;
        }
    }

    public void UpdateLearningModes(string mode)
    {
        if (mode == "number")
        {
            numbersUnlocked = true;
        }
        if (mode == "word")
        {
            wordsUnlocked = true;
        }
    }
}
