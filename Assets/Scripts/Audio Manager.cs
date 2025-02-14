/* Author: Chong Yu Xiang  
 * Filename: Audio Manager
 * Descriptions: Controls all audio related functions
 */

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager instance;

    public Sound[] BGMSounds, SFXSounds;
    public AudioSource BGMSource, SFXSource;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else if (instance != null && instance != this)
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        BGMSource.volume = 1f;
        SFXSource.volume = 1f;
    }

    public void ToggleBGM()
    {
        BGMSource.mute = !BGMSource.mute;
    }
    public void ToggleSFX()
    {
        SFXSource.mute = !SFXSource.mute;
    }
    public void Volume(float volume)
    {
        BGMSource.volume = volume / 10 * 5;
        SFXSource.volume = volume / 10 * 5;
    }

    public void PlayBGM(string name)
    {
        Sound s = Array.Find(BGMSounds, x => x.name == name);

        BGMSource.clip = s.clip;
        BGMSource.Play();
    }

    public void PlaySFX(string name)
    {
        Sound s = Array.Find(SFXSounds, x => x.name == name);

        SFXSource.PlayOneShot(s.clip);
    }
}
