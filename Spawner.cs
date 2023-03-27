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
        spawnPoint = GetComponentsInChildren<Transform>();//이거 자기 자신도 포함함

    }

    void Update()
    {
        if (!GameManager.instance.isLive)
            return;

        timer += Time.deltaTime;

        //현재는 10초당 몬스터가 강해지는 구조 -> 나중에 킬 수랑 타이머 둘 다 써도 괜찮을 듯?
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

        //자기자신 빼려고 1부터
        enemy.transform.position = spawnPoint[Random.Range(1,spawnPoint.Length)].position + new Vector3(0,-0.98f,0);
        enemy.GetComponent<Enemy>().Init(spawnData[level]);
    }
}

[System.Serializable]
public class SpawnData
{
    public float spawnTime;//얘는 여기서만 씀

    public int spriteType;//level관련
    public int health;
    public float speed;

    public int damage;
}