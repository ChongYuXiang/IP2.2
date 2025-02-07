/* Author: Wang Johnathan Zhiwen 
* Filename: SpawnObject
* Descriptions: functions for spawning objects
*/

using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class SpawnObject : MonoBehaviour
{
    public TMP_InputField inputField; // Reference to the TMP Input Field
    public Transform spawnPoint; // The position where the prefab will be spawned
    public List<GameObject> prefabList; // Assign prefabs in the inspector

    private Dictionary<string, GameObject> prefabDictionary = new Dictionary<string, GameObject>();

    void Start()
    {
        // Populate the dictionary using prefab names as keys
        foreach (var prefab in prefabList)
        {
            prefabDictionary[prefab.name.ToLower()] = prefab;
        }
        inputField.onValueChanged.AddListener(delegate { CheckInput(); }); // Listen for input changes

    }


    void CheckInput()
    {
        if (inputField.text.Length > 0)
        {
            SpawnPrefab();
        }
    }

    public void SpawnPrefab()
    {
        string inputText = inputField.text.Trim().ToLower(); // Get input and normalize it

        if (prefabDictionary.ContainsKey(inputText))
        {
            Instantiate(prefabDictionary[inputText], spawnPoint.position, Quaternion.identity);
        }
        else
        {
            Debug.LogWarning("No matching prefab found for input: " + inputText);
        }
    }
}

