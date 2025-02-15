/* Author: Chong Yu Xiang  
 * Filename: Player
 * Descriptions: Class for players
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player
{
    public string username;
    public string email;
    public string gender;
    public string race;
    public bool active_status;
    public int account_creation_date;
    public int last_logged_in_time;

    public Player(string username, string email, string gender, string race, bool active_status, int account_creation_date, int last_logged_in_time)
    {
        this.username = username;
        this.email = email;
        this.gender = gender;
        this.race = race;
        this.active_status = active_status;
        this.account_creation_date = account_creation_date;
        this.last_logged_in_time = last_logged_in_time;
    }
}