/* Author: Chong Yu Xiang  
 * Filename: HDBnumber
 * Descriptions: randomly generate block number for HDB
 */

using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class HDBnumber : MonoBehaviour
{
    private void Awake()
    {
        // Randomly generate a number from 1 to 999
        System.Random rng = new System.Random();
        int numRNG = rng.Next(1, 999);

        gameObject.GetComponent<TextMeshPro>().text = numRNG.ToString();
    }
}
