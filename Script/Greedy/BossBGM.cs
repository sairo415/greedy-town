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
    
    int nextSceneIndex;

    void Awake()
    {
        GetComponent<AudioSource>().clip = scene0BGM;
        GetComponent<AudioSource>().loop = true;
        GetComponent<AudioSource>().Play();
    }

    void Start()
    {
        nextSceneIndex = SceneManager.GetActiveScene().buildIndex + 1;
        //DontDestroyOnLoad(this.gameObject);
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if(scene.buildIndex == nextSceneIndex)
        {
            AudioClip changeBGM;
            switch(nextSceneIndex)
            {
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

            nextSceneIndex++;
        }
    }
}
