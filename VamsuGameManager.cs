using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class VamsuGameManager : MonoBehaviour
{
    public static VamsuGameManager instance;

    [Header("# Game Control")]
    public bool isLive;
    public float gameTime;
    public float maxGameTime = 30 * 10f;

    [Header("# Player Info")]
    public float health;
    public float maxHealth;
    public int level;
    public int kill;
    public float exp;
    //각 레벨당 요구량
    public int[] nextExp;

    [Header("# Game Object")]
    public PoolManager pool;
    public VamsuPlayer player;
    public GameObject canvas;
    public LevelUp uiLevelUp;

    //10퍼센트 -> 0.1로 치환. 백분율이 추가되는 방식
    [Header("# Player Stat")]
    public float extraDamage;//ok
    public float extraCoolDown;
    public float extraArmor;//ok
    public float extraExp;//ok
    public float extraGold;
    public float extraHealth;//ok
    public float extraSpeed;//ok

    private void Awake()
    {
        Time.timeScale = 0;
        instance = this;
        maxHealth = 100 * (1 + extraHealth);
        health = maxHealth;
        player.speed *= (1 + extraSpeed);


        nextExp = new int[30];
        for(int i=0; i<30; i++)
        {
            nextExp[i] = 5 + i * 12;
        }
    }

    void Update()
    {
        if (!isLive)
            return;

        gameTime += Time.deltaTime;

        if (gameTime > maxGameTime)
        {
            //game win
            canvas.transform.Find("Victory").gameObject.SetActive(true);
            canvas.transform.Find("Restart").gameObject.SetActive(true);
            Time.timeScale = 0;
        }
    }

    public void GetExp()
    {
        exp += (1+ extraExp);
        if(exp >= nextExp[Mathf.Min(level, nextExp.Length-1)])
        {
            level++;
            exp = 0;
            //레벨업 로직
            uiLevelUp.Show();
        }
    }

    public void GetDamage(float damage)
    {
        health -= (damage * (1 - extraArmor));
        if(health <= 0)
        {
            //gameOver
            canvas.transform.Find("Dead").gameObject.SetActive(true);
            canvas.transform.Find("Restart").gameObject.SetActive(true);
            Time.timeScale = 0;
        }
    }

    public void GameStart()
    {
        Resume();

        canvas.transform.Find("Title").gameObject.SetActive(false);
        canvas.transform.Find("Start").gameObject.SetActive(false);
        canvas.transform.Find("HUD").gameObject.SetActive(true);

        //총 선택
        uiLevelUp.Select(1);

        //uiLevelUp.Show();
    }

    public void ReStart()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        Time.timeScale = 1;
    }

    public void Stop()
    {
        isLive = false;
        Time.timeScale = 0;
    }

    public void Resume()
    {
        isLive = true;
        Time.timeScale = 1;
    }
}
