using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Particle : MonoBehaviour
{
	public int damage = 50;

    public ParticleSystem particleSystem;

    List<ParticleCollisionEvent> colEvents = new List<ParticleCollisionEvent>();

	private void Start()
	{
		particleSystem = GetComponent<ParticleSystem>();
	}
	private void Update()
	{
		if(Input.GetKeyDown(KeyCode.Mouse0))
		{
			particleSystem.Play();
		}
	}

	private void OnParticleCollision(GameObject other)
	{
		int events = particleSystem.GetCollisionEvents(other, colEvents);

		Debug.Log("Hit");

		for(int i = 0; i < events; i++)
		{

		}

		if(other.TryGetComponent(out Enemy enemy))
		{
			enemy.TakeDamage(damage);
		}
	}
}
