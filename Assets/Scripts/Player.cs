/* Author: Chong Yu Xiang  
 * Filename: Player
 * Descriptions: Class for player
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player
{
    public int score;
    public int highscore;
    public string username;

    public Player(int score, int highscore, string username)
    {
        this.score = score;
        this.highscore = highscore;
        this.username = username;
    }
}