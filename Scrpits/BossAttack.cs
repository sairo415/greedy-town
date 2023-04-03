using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossAttack : MonoBehaviour
{
    // �������� ���� �� ���� ����
    public int minDamage;
    public int maxDamage;

    // ��ų ������
    public int damage;

    public float damageTimer = 0.0f;    // ��Ʈ �������� ���� �ð� ����
    public float damageInterval;        // ��Ʈ ������ �ֱ�

    public bool isInBoss;				// �÷��̾ �ش� ���� ������ ����

    private void Awake()
    {
        damage = Random.Range(minDamage, maxDamage);
    }
}
