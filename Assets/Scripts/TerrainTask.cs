/* Author: Chong Yu Xiang  
 * Filename: TerrainTask
 * Descriptions: Update terrain tutorial checkboxes
 */

using System.Collections;
using System.Collections.Generic;
using Seagull.Interior_01;
using UnityEngine;

public class TerrainTask : MonoBehaviour
{
    // Checkboxes
    public GameObject check1;
    public GameObject check2;
    public GameObject check3;
    public GameObject check4;
    public GameObject check5;

    private void Start() // Check GameManager for which tasks done
    {
        if (GameManager.instance.terrainTask1 == true)
        {
            check1.SetActive(true);
        }
        if (GameManager.instance.terrainTask2 == true)
        {
            check2.SetActive(true);
        }
        if (GameManager.instance.terrainTask3 == true)
        {
            check3.SetActive(true);
        }
        if (GameManager.instance.terrainTask4 == true)
        {
            check4.SetActive(true);
        }
        if (GameManager.instance.terrainTask5 == true)
        {
            check5.SetActive(true);
        }
    }

    public void TaskDone(int task) // Update task
    {
        if (task == 1)
        {
            check1.SetActive(true);
            GameManager.instance.terrainTask1 = true;
        }
        if (task == 2)
        {
            check2.SetActive(true);
            GameManager.instance.terrainTask2 = true;
        }
        if (task == 3)
        {
            check3.SetActive(true);
            GameManager.instance.terrainTask3 = true;
        }
        if (task == 4)
        {
            check4.SetActive(true);
            GameManager.instance.terrainTask4 = true;
        }
        if (task == 5)
        {
            check5.SetActive(true);
            GameManager.instance.terrainTask5 = true;
        }
    }
}
