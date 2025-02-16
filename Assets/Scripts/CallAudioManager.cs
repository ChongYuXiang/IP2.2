using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CallAudioManager : MonoBehaviour
{
   public void callAudioManager(string Button)
    {
        AudioManager.instance.PlaySFX(Button);
    }
}
