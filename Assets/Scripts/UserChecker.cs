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


    /// <summary>
    /// Checks the user's authentication status by checking the UUID from the database.
    /// If authenticated, the menu is shown, and the warning is hidden. Otherwise, the warning is shown.
    /// </summary>
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


    /// <summary>
    /// Resets the UI elements by hiding both the menu and warning.
    /// This is typically called when authentication status changes.
    /// </summary>
    public void AuthChanged()
    {
        menu.SetActive(false);
        warning.SetActive(false);
    }
}
