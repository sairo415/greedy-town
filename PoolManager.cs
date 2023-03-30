using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PoolManager : MonoBehaviour
{
    //프리펩들을 보관할 변수
    public GameObject[] monsterPrefabs;
    public GameObject[] weaponPrefabs;
    public GameObject[] bossPrefabs;

    //풀 담당을 하는 리스트들
    List<GameObject>[] monsterPools;
    List<GameObject>[] weaponPools;

    void Awake()
    {
        monsterPools = new List<GameObject>[monsterPrefabs.Length];
        weaponPools = new List<GameObject>[weaponPrefabs.Length];

        for(int i=0; i< monsterPrefabs.Length; i++)
            monsterPools[i] = new List<GameObject>();

        for (int i = 0; i < weaponPrefabs.Length; i++)
            weaponPools[i] = new List<GameObject>();

    }

    public GameObject Get(int index, bool isMonster)
    {
        List<GameObject>[] pools;
        GameObject[] prefabs;
        if (isMonster)
        {
            pools = monsterPools;
            prefabs = monsterPrefabs;
        }
        else
        {
            pools = weaponPools;
            prefabs = weaponPrefabs;
        }

        //선택한 풀의 비활성화 된 게임 오브젝트 접근        
        foreach(GameObject item in pools[index])
        {
            if (!item.activeSelf)//비활성화된 놈들
            {
                //발견하면 select변수에 할당
                item.SetActive(true);
                return item;
            }
        }

        // 못 찾았으면?
        //새롭게 생성하고 할당
        GameObject select = Instantiate(prefabs[index], transform);//풀 매니저 아래서 관리하겠다.
        pools[index].Add(select);
        
        return select;
    }

    public GameObject GetBoss(int index)
    {
        GameObject boss = Instantiate(bossPrefabs[index], transform);
        return boss;
    }

    //임시 근접 무기 할당 -> 근접은 활성화 비활성화를 주기적으로 하는 것 들이 있어서 위의 것 처럼 하기엔 애매해짐
    // 이거 망치 밸런스 문제상 망치를 성서처럼 떨어뜨리고 속도를 늦추던가 할까
    public GameObject GetMelee(int index)
    {
        List<GameObject>[] pools = weaponPools;
        GameObject[] prefabs = weaponPrefabs;

        // 못 찾았으면?
        //새롭게 생성하고 할당 -> 이전 거는 자식에서 가져다 알아서 쓰고 있음
        GameObject select = Instantiate(prefabs[index], transform);//풀 매니저 아래서 관리하겠다.
        select.SetActive(false);
        pools[index].Add(select);

        return select;
    }
}
