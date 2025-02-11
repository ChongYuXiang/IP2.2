/* Author: Chong Yu Xiang  
* Filename: NumberGame
* Descriptions: Class for number_game
*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NumberGame : MonoBehaviour
{
    public int correct;
    public int wrong;
    public int time_taken;
    public int average_time_per_number;

    public NumberGame(int correct, int wrong, int time_taken, int average_time_per_number)
    {
        this.correct = correct;
        this.wrong = wrong;
        this.time_taken = time_taken;
        this.average_time_per_number = average_time_per_number;
    }
}
