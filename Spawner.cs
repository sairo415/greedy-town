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
        timer += Time.deltaTime;

        //����� 10�ʴ� ���Ͱ� �������� ���� -> ���߿� ų ���� Ÿ�̸� �� �� �ᵵ ������ ��?
        level = Mathf.FloorToInt(GameManager.instance.gameTime / 10f);

        if (timer > spawnData[level].spawnTime)
        {
            timer = 0;
            Spawn();
        }
    }

    void Spawn()
    {
        GameObject enemy = GameManager.instance.pool.Get(0, true);

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