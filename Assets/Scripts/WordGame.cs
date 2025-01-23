/* Author: Chong Yu Xiang  
 * Filename: WordGame
 * Descriptions: Class for word_game
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WordGame
{
    public int highscore;
    public int correct;
    public int wrong;
    public int time_taken;
    public int average_time_per_word;

    public WordGame(int highscore, int correct, int wrong, int time_taken, int average_time_per_word)
    {
        this.highscore = highscore;
        this.correct = correct;
        this.wrong = wrong;
        this.time_taken = time_taken;
        this.average_time_per_word = average_time_per_word;
    }
}