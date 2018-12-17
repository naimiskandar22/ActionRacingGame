using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoomParticleScript : MonoBehaviour {

	public float expandSize;
	Vector3 expandRate;
	float delayTimer;
	float damage; 

	public bool emp;

	public CircleCollider2D coll;

	// Use this for initialization
	void Start () 
	{
		
	}
	
	// Update is called once per frame
	void Update () 
	{

		if(delayTimer <= 0f)
		{
			if(transform.localScale.x < expandSize)
			{
				transform.localScale += expandRate;
			}
			else
			{
				Destroy(gameObject, 0.8f);
			}
		}
		else
		{
			delayTimer -= Time.deltaTime;
		}


	}

	public void ExpandBoom(float size, float delay, float dmg, float expSpeed)
	{
		expandSize = size;

		expandRate.x = 0f;
		expandRate.y = 0f;
		expandRate.z = 1f;

		transform.localScale = expandRate;

		expandRate.x = Time.deltaTime * 1f * expSpeed;
		expandRate.y = Time.deltaTime * 1f * expSpeed;

		delayTimer = delay;
		damage = dmg;

//		float delayTimer = 5f;
//
//		while(transform.localScale.x < expandSize)
//		{
//			if(delayTimer <= 0f)
//			{
//				transform.localScale += exp;
//				delayTimer = 5f;
//			}
//
//			delayTimer -= Time.deltaTime / 100f;
//
//		}
	}

	void OnTriggerEnter2D(Collider2D other)
	{
		if(!emp)
		{
			if(other.CompareTag("Enemy"))
			{
				EnemyScript enemyscript = other.GetComponent<EnemyScript>();

				enemyscript.TakeDamage(damage);
			}
		}
		else
		{
			if(other.CompareTag("Enemy"))
			{
				EnemyScript enemyscript = other.GetComponent<EnemyScript>();

				enemyscript.fireCooldown = 50f;

				enemyscript.StartCoroutine(enemyscript.HitEMP());
			}
			else
			{
				MGBulletScript bulletscript = other.GetComponent<MGBulletScript>();

				if(bulletscript != null)
				{
					if(!bulletscript.friendly)
					{
						Debug.Log("EMp");
						Destroy(other.gameObject);
					}
				}
			}
		}

	}
}
