//Author: Lim Rui Xi Coleman
//Filename: CallAudioManager
//Description: Reference AudioManager from previous scripts


using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CallAudioManager : MonoBehaviour
{
    //Preserve and call on Audio Manager from another scene
   public void callAudioManager(string Button)
    {
        AudioManager.instance.PlaySFX(Button);
    }
}
