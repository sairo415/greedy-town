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
    public float health;
    public float maxHealth;
    public int level;
    public int kill;
    public float exp;
    //�� ������ �䱸��
    public int[] nextExp;

    [Header("# Game Object")]
    public PoolManager pool;
    public VamsuPlayer player;
    public GameObject canvas;

    //10�ۼ�Ʈ -> 0.1�� ġȯ
    [Header("# Player Stat")]
    public float extraDamage;//ok
    public float extraHealth;//ok
    public float extraSpeed;//ok
    public float extraArmor;//ok
    public float extraCoolDown;
    public float extraExp;//ok
    public float extraGold;

    private void Awake()
    {
        Time.timeScale = 0;
        instance = this;
        maxHealth = 100 * (1+ extraHealth);
        health = maxHealth;
        player.speed *= (1 + extraSpeed);
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
        exp += (1+ extraExp);
        if(exp >= nextExp[level])
        {
            level++;
            exp = 0;
            //������ ����
            LevelUp();
        }
    }

    //ĵ�������� ����ü 3���� �������� ���
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
        Time.timeScale = 1;
        canvas.transform.Find("Title").gameObject.SetActive(false);
        canvas.transform.Find("Start").gameObject.SetActive(false);

        
        GameObject augment = canvas.transform.Find("LevelUp").gameObject;
        augment.transform.GetChild(10).gameObject.GetComponent<Item>().OnClick();
        augment.transform.GetChild(10).gameObject.GetComponent<Item>().OnClick();
        augment.transform.GetChild(10).gameObject.GetComponent<Item>().OnClick();
        augment.transform.GetChild(10).gameObject.GetComponent<Item>().OnClick();
        augment.transform.GetChild(10).gameObject.GetComponent<Item>().OnClick();
        augment.transform.GetChild(10).gameObject.GetComponent<Item>().OnClick();


        //LevelUp();
    }

    public void ReStart()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        Time.timeScale = 1;
    }

}
