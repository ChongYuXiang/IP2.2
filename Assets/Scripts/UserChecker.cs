/* Author: Chong Yu Xiang  
 * Filename: UserChecker
 * Descriptions: Applied on door to gamemodes to check if user is logged in to an account
 */

using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class UserChecker : MonoBehaviour
{
    private GameObject database;

    // UI elements
    public GameObject warning;
    public GameObject menu;

    public void AuthCheck()
    {
        database = GameObject.Find("Database");
        if (database.GetComponent<Database>().uuid != null)
        {
            menu.SetActive(true);
            warning.SetActive(false);
        }
        else
        {
            menu.SetActive(false);
            warning.SetActive(true);
        }
    }

    public void AuthChanged()
    {
        menu.SetActive(false);
        warning.SetActive(false);
    }
}
