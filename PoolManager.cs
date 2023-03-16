using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PoolManager : MonoBehaviour
{
    //��������� ������ ����
    public GameObject[] monsterPrefabs;
    public GameObject[] weaponPrefabs;

    //Ǯ ����� �ϴ� ����Ʈ��
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

        //������ Ǯ�� ��Ȱ��ȭ �� ���� ������Ʈ ����        
        foreach(GameObject item in pools[index])
        {
            if (!item.activeSelf)//��Ȱ��ȭ�� ���
            {
                //�߰��ϸ� select������ �Ҵ�
                item.SetActive(true);
                return item;
            }
        }

        // �� ã������?
        //���Ӱ� �����ϰ� �Ҵ�
        GameObject select = Instantiate(prefabs[index], transform);//Ǯ �Ŵ��� �Ʒ��� �����ϰڴ�.
        pools[index].Add(select);
        
        return select;
    }

    //�ӽ� ���� ���� �Ҵ� -> ������ Ȱ��ȭ ��Ȱ��ȭ�� �ֱ������� �ϴ� �� ���� �־ ���� �� ó�� �ϱ⿣ �ָ�����
    public GameObject GetMelee(int index)
    {
        List<GameObject>[] pools = weaponPools;
        GameObject[] prefabs = weaponPrefabs;

        // �� ã������?
        //���Ӱ� �����ϰ� �Ҵ� -> ���� �Ŵ� �ڽĿ��� ������ �˾Ƽ� ���� ����
        GameObject select = Instantiate(prefabs[index], transform);//Ǯ �Ŵ��� �Ʒ��� �����ϰڴ�.
        select.SetActive(false);
        pools[index].Add(select);

        return select;
    }
}
