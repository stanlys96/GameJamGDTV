using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioLoader : MonoBehaviour
{
    public AudioManager audioManager;

    private void Awake()
    {
        if (AudioManager.instance == null)
        {
            AudioManager newAudioManager = Instantiate(audioManager);
            AudioManager.instance = newAudioManager;
            DontDestroyOnLoad(newAudioManager.gameObject);
        }
    }
}