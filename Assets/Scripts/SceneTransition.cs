//Author: Lim Rui Xi Coleman
//Filename: SceneTransition
//Description: Add transition during scene change


using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneTransition : MonoBehaviour
{
    public Animator transitionAnim;

    public void transitionScene(int index)
    {
        StartCoroutine(LoadScene(index));
    }

    IEnumerator LoadScene(int index)
    {
        transitionAnim.SetTrigger("end");
        yield return new WaitForSeconds(1.5f);
        ChangeSceneByIndex(index);
    }
    private void ChangeSceneByIndex(int sceneIndex)
    {
        SceneManager.LoadScene(sceneIndex);

        if (sceneIndex == 1)
        {
            Database.instance.LinkObjects();
        }
    }
}
