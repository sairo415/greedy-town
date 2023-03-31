using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossPlayerSkill : MonoBehaviour
{
	// �������� ���� �� ���� ����
    public int minDamage;
    public int maxDamage;

	// ��ų ������
	public int damage;

	// ��ų ������
	private int viewId;

	public float damageTimer = 0.0f;	// ��Ʈ �������� ���� �ð� ����
	public float damageInterval;        // ��Ʈ ������ �ֱ�

	public bool isInBoss;				// ������ �ش� ���� ������ ����?

	[SerializeField]
	public bool isVampirism;

	private void Awake()
	{
		damage = Random.Range(minDamage, maxDamage);
	}

	// PVP
	public void SetID(int viewId)
	{
		this.viewId = viewId;
	}

	public int GetID()
	{
		return viewId;
	}
}
