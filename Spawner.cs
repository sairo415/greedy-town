using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawner : MonoBehaviour
{
    public Transform[] spawnPoint;
    public SpawnData[] spawnData;

    int level;
    float timer;

    void Awake()
    {
        spawnPoint = GetComponentsInChildren<Transform>();//�̰� �ڱ� �ڽŵ� ������

        spawnData = new SpawnData[GameManager.instance.pool.monsterPrefabs.Length * 5];

        /*spawnData[0].spawnTime = 0.4f;
        spawnData[0].spriteType = 0;
        spawnData[0].health = 10;
        spawnData[0].speed = 1.5f;
        spawnData[0].damage = 3;*/
        
        for(int i=0; i<GameManager.instance.pool.monsterPrefabs.Length * 5; i++)
        {
            spawnData[i] = new SpawnData();
            spawnData[i].spawnTime = Mathf.Max(0.3f - 0.01f * i, 0.1f);
            spawnData[i].spriteType = i % 5;
            spawnData[i].health = 15 + 4 * i;
            spawnData[i].speed = 1.8f + 0.05f * i;
            spawnData[i].damage = 3 + 0.5f * i;
        }

    }

    void Update()
    {
        if (!GameManager.instance.isLive)
            return;

        timer += Time.deltaTime;

        //����� 10�ʴ� ���Ͱ� �������� ���� -> ���߿� ų ���� Ÿ�̸� �� �� �ᵵ ������ ��?
        level = Mathf.FloorToInt(GameManager.instance.gameTime / 30f);//30f * 4

        if (timer > spawnData[level].spawnTime)
        {
            timer = 0;
            Spawn();
        }
    }

    void Spawn()
    {
        GameObject enemy = GameManager.instance.pool.Get(Mathf.FloorToInt(level / 5), true);//

        //�ڱ��ڽ� ������ 1����
        enemy.transform.position = spawnPoint[Random.Range(1,spawnPoint.Length)].position + new Vector3(0,-0.98f,0);
        enemy.GetComponent<Enemy>().Init(spawnData[level]);
    }
}

[System.Serializable]
public class SpawnData
{
    public float spawnTime;//��� ���⼭�� ��

    public int spriteType;//level����
    public float health;
    public float speed;

    public float damage;
}