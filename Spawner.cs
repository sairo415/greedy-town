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
        GameObject enemy = GameManager.instance.pool.Get(Mathf.FloorToInt(level / 4), true);//

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
    public int health;
    public float speed;

    public int damage;
}