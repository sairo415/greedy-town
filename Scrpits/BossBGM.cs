using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class BossBGM : MonoBehaviour
{
    public AudioClip scene0BGM;
    public AudioClip scene1BGM;
    public AudioClip scene2BGM;
    public AudioClip scene3BGM;
    public AudioClip scene4BGM;

    bool isPlay = false;

	private void Update()
	{
        if(isPlay)
            return;

        BossGameManager bossGameManager = GameObject.FindObjectOfType<BossGameManager>();

        if(bossGameManager != null)
        {
            isPlay = true;
            AudioClip changeBGM;
            switch(bossGameManager.stage)
            {
            case 0:
                changeBGM = scene1BGM;
                break;
            case 1:
                changeBGM = scene1BGM;
                break;
            case 2:
                changeBGM = scene2BGM;
                break;
            case 3:
                changeBGM = scene3BGM;
                break;
            case 4:
                changeBGM = scene4BGM;
                break;
            default:
                return;
            }

            // BGM 변경 코드
            AudioSource audioSource = GetComponent<AudioSource>();
            if(audioSource != null)
            {
                audioSource.Stop();
                audioSource.clip = changeBGM;
                audioSource.loop = true;
                audioSource.Play();
            }
        }
	}
}
