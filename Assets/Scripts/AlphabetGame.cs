/* Author: Chong Yu Xiang  
 * Filename: AlphabetGame
 * Descriptions: Class for alphabet_game
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AlphabetGame
{
    public int highscore;
    public int correct;
    public int wrong;
    public int time_taken;
    public int average_time_per_letter;

    public AlphabetGame(int highscore, int correct, int wrong, int time_taken, int average_time_per_letter)
    {
        this.highscore = highscore;
        this.correct = correct;
        this.wrong = wrong; 
        this.time_taken = time_taken;
        this.average_time_per_letter = average_time_per_letter;
    }
}