using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    [Header("# Game Control")]
    public float gameTime;
    public float maxGameTime = 30 * 10f;

    [Header("# Player Info")]
    public int health;
    public int maxHealth;
    public int level;
    public int kill;
    public int exp;
    //각 레벨당 요구량
    public int[] nextExp;

    [Header("# Game Object")]
    public PoolManager pool;
    public Player player;
    public GameObject canvas;

    private void Awake()
    {
        Time.timeScale = 0;
        instance = this;
        maxHealth = 100;
        health = maxHealth;
        canvas = GameObject.Find("Canvas");


        nextExp = new int[30];
        for(int i=0; i<30; i++)
        {
            nextExp[i] = 3 + i * 4;
        }
    }

    void Update()
    {
        gameTime += Time.deltaTime;

        if (gameTime > maxGameTime)
        {
            //gameWin
            gameTime = maxGameTime;
            canvas.transform.Find("Victory").gameObject.SetActive(true);
            canvas.transform.Find("Restart").gameObject.SetActive(true);
            Time.timeScale = 0;
        }
    }

    public void GetExp()
    {
        exp++;
        if(exp == nextExp[level])
        {
            level++;
            exp = 0;
            //레벨업 로직
            LevelUp();
        }
    }

    //캔버스에서 증강체 3개를 가져오는 방식
    public void LevelUp()
    {
        Time.timeScale = 0;

        GameObject augment = canvas.transform.Find("LevelUp").gameObject;

        int max = augment.transform.childCount;
        List<int> list = new List<int>();
        while (list.Count < 3)
        {
            int now = Random.Range(0, max);
            if (!list.Contains(now))
                list.Add(now);
        }


        foreach (int num in list)
        {
            augment.transform.GetChild(num).gameObject.SetActive(true);
        }
    }

    public void GetDamage(int damage)
    {
        health -= damage;
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
        Time.timeScale = 1;
        canvas.transform.Find("Title").gameObject.SetActive(false);
        canvas.transform.Find("Start").gameObject.SetActive(false);

        
        GameObject augment = canvas.transform.Find("LevelUp").gameObject;
        augment.transform.GetChild(10).gameObject.GetComponent<Item>().OnClick();

        //LevelUp();
    }

    public void ReStart()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        Time.timeScale = 1;
    }

}
