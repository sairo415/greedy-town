using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VamsuSoundManager : MonoBehaviour
{
    public static VamsuSoundManager instance;

    public AudioSource audioSource;
    public AudioSource bgmSource;
    public AudioClip clipLevelUp;
    public AudioClip bgmBasic;
    public AudioClip bgmBoss;

    void Awake()
    {
        instance = this;
        PlayBasicBGM();
        bgmSource.loop = true;
    }
    
    public void PlayLevelUpSound()
    {
        audioSource.PlayOneShot(clipLevelUp);
    }

    public void PlayBasicBGM()
    {
        bgmSource.clip = bgmBasic;
        bgmSource.Play();
    }

    public void PlayBossBGM()
    {
        bgmSource.clip = bgmBoss;
        bgmSource.Play();
    }
}
