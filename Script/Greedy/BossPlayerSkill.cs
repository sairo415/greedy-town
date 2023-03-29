using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossPlayerSkill : MonoBehaviour
{
    public int minDamage;
    public int maxDamage;

	public int damage;

	private int viewId;

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
