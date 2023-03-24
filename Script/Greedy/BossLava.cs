using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossLava : MonoBehaviour
{
    public int damageAmount = 10;
    public float damageInterval = 1.0f;
	bool inLava = false;

    float damageTimer = 0.0f;

	void OnTriggerEnter(Collider other)
	{
		if(other.CompareTag("Player"))
		{
			inLava = true;
		}
	}

	void OnTriggerExit(Collider other)
	{
		if(other.CompareTag("Player"))
		{
			inLava = false;
			damageTimer = 0.0f;
		}
	}

	void OnTriggerStay(Collider other)
	{
		if(other.CompareTag("Player") && inLava)
		{
			damageTimer += Time.deltaTime;

			if(damageTimer >= damageInterval)
			{
				other.GetComponent<BossPlayer>().curHealth -= damageAmount;
				damageTimer = 0.0f;
			}
		}	
	}
}
