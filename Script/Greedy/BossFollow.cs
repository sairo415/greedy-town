using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class BossFollow : MonoBehaviour
{
    // 따라갈 목표와 위치 오프셋을 public 변수로 선언
    public Transform target;
    public Vector3 offset;

    public AudioClip scene1BGM;
    public AudioClip scene2BGM;
    public AudioClip scene3BGM;
    public AudioClip scene4BGM;

    // 씬 이동 관련
    int nextSceneIndex;

    void Awake()
    {
        GetComponent<AudioSource>().clip = scene1BGM;
        GetComponent<AudioSource>().loop = true;
        GetComponent<AudioSource>().Play();
    }

    void Start()
    {
        nextSceneIndex = SceneManager.GetActiveScene().buildIndex + 1;
        DontDestroyOnLoad(this.gameObject);
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void Update()
    {
        transform.position = target.position + offset;
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
                changeBGM = scene2BGM;
                break;
            case 2:
                changeBGM = scene3BGM;
                break;
            case 3:
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
