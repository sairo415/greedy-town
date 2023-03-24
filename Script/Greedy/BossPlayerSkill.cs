using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossPlayerSkill : MonoBehaviour
{
    public int minDamage;
    public int maxDamage;

	public int damage;

	private void Start()
	{
		damage = Random.Range(minDamage, maxDamage);
	}
}
