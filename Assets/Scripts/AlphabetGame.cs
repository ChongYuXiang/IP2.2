/* Author: Chong Yu Xiang  
* Filename: AlphabetGame
* Descriptions: Class for alphabet_game
*/

using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;  // For UI elements

public class AlphabetGame : MonoBehaviour
{
    public int correct;
    public int wrong;
    public int time_taken;
    public int average_time_per_letter;

    public AlphabetGame(int correct, int wrong, int time_taken, int average_time_per_letter)
    {
        this.correct = correct;
        this.wrong = wrong;
        this.time_taken = time_taken;
        this.average_time_per_letter = average_time_per_letter;
    }
}
