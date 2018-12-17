using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CannonBulletScript : MonoBehaviour {

	public SpriteRenderer rend;
	public float damage;
	public float bulletSpeed = 2f;
	public bool clusternade = false;
	public CannonType bulletType;

	public BoxCollider2D coll;

	float colorFlashTimer;
	float colorFlashDuration = 0.2f;

	public GameObject boomParticle;

	// Use this for initialization
	void Start () 
	{
		rend = GetComponent<SpriteRenderer>();

		coll = GetComponent<BoxCollider2D>();
	}
	
	// Update is called once per frame
	void Update() 
	{
		if(PauseManagerScript.instance.isPaused)
		{
			return;
		}

		colorFlashTimer -= Time.deltaTime;

		if(bulletType == CannonType.CLUSTERNADE || bulletType == CannonType.CLUSTERCHILD)
		{
			if(colorFlashTimer <= colorFlashDuration / 2)
			{
				rend.color = Color.red;

				if(colorFlashTimer <= 0f)
				{
					colorFlashTimer = colorFlashDuration;
				}
			}
			else if(colorFlashTimer <= colorFlashDuration)
			{
				rend.color = Color.yellow;
			}
		}
		else if(bulletType == CannonType.BOOMERANG || bulletType == CannonType.BOOMERANGCHILD)
		{
			if(colorFlashTimer <= colorFlashDuration / 2)
			{
				rend.color = Color.cyan;

				if(colorFlashTimer <= 0f)
				{
					colorFlashTimer = colorFlashDuration;
				}
			}
			else if(colorFlashTimer <= colorFlashDuration)
			{
				rend.color = Color.yellow;
			}
		}

		if(bulletType == CannonType.CLUSTERCHILD)
		{

			if(bulletSpeed < 0f)
			{
				bulletSpeed = 0f;

				GameObject newpart = Instantiate(boomParticle, this.transform.position, Quaternion.identity) as GameObject;
				BoomParticleScript partscript = newpart.GetComponent<BoomParticleScript>();

				if(partscript != null)
				{
					partscript.ExpandBoom(0.5f, 0.5f, damage, 1f);
					Destroy(gameObject, 0.5f);
				}
			}
			else if(bulletSpeed > 0f)
			{
				bulletSpeed -= Time.deltaTime * 100f;
			}
		}
		else if(bulletType == CannonType.BOOMERANG)
		{
			if(!coll.enabled)
			{
				transform.Rotate(0f, 0f, 5f);
			}
		}

		transform.Translate (Vector3.right * Time.deltaTime * bulletSpeed);
	}

//	void BulletHit(EnemyScript other)
//	{
//		if(bulletType == CannonType.NORMAL)
//		{
//			
//		}
//	}

	void OnTriggerEnter2D(Collider2D other)
	{
		if(!other.CompareTag("Player") && !other.CompareTag("EnemyBullet"))
		{
			Debug.Log("Cannon Hit");

			EnemyScript enemy = other.GetComponent<EnemyScript>();

			if(bulletType == CannonType.NORMAL)
			{
				
				if(enemy != null)
				{
					enemy.TakeDamage(damage);
				}

				Destroy(gameObject);

			}
			else if(bulletType == CannonType.SNIPER)
			{
				
				if(enemy != null)
				{
					enemy.TakeDamage(damage);
				}

				Destroy(gameObject);

			}
			else if(bulletType == CannonType.SHOTGUN)
			{
				
				if(enemy != null)
				{
					enemy.TakeDamage(damage);
				}

				Destroy(gameObject);

			}
			else if(bulletType == CannonType.CLUSTERNADE)
			{
				
				bulletType = CannonType.NORMAL;

				for(int i = 0; i < 6; i++)
				{
					Transform pos = transform;

					pos.Rotate(0f, 0f, 360f / 6f * i);

					GameObject newbullet = Instantiate(this.gameObject, transform.position, pos.rotation);

					CannonBulletScript bulletscript = newbullet.GetComponent<CannonBulletScript>();
					bulletscript.bulletType = CannonType.CLUSTERCHILD;
					bulletscript.damage = damage;
				}

				if(enemy != null)
				{
					enemy.TakeDamage(damage);
				}

				Destroy(gameObject);
			}
			else if(bulletType == CannonType.BOOMERANG)
			{
				Debug.Log(other.name);

				coll.enabled = false;
				rend.enabled = false;

				GameObject newbullet = Instantiate(this.gameObject, transform.position, transform.rotation);

				CannonBulletScript bulletscript = newbullet.GetComponent<CannonBulletScript>();
				bulletscript.bulletType = CannonType.BOOMERANGCHILD;
				bulletscript.coll.enabled = true;
				bulletscript.rend.enabled = true;
				bulletscript.bulletSpeed = bulletSpeed * 0.005f;
				bulletscript.damage = 20f;

				newbullet.transform.parent = this.transform;

				if(enemy != null)
				{
					enemy.TakeDamage(damage);
				}

				Destroy(gameObject, 5f);
			}
			else if(bulletType == CannonType.BOOMERANGCHILD)
			{
				if(enemy != null)
				{
					enemy.TakeDamage(damage);
				}
			}
			else if(bulletType == CannonType.CLUSTERCHILD)
			{
				bulletSpeed = 0f;

				Debug.Log(other.name);

				if(enemy != null)
				{
					enemy.TakeDamage(damage);

					GameObject newpart = Instantiate(boomParticle, this.transform.position, Quaternion.identity) as GameObject;
					BoomParticleScript partscript = newpart.GetComponent<BoomParticleScript>();

					if(partscript != null)
					{
						partscript.ExpandBoom(1f, 0f, damage, 1f);
						Destroy(gameObject, 0.2f);
					}


					//StartCoroutine(ClusterBoom(1.2f, 10f, 12, 15, 0.2f));
				}
				else
				{
					GameObject newpart = Instantiate(boomParticle, this.transform.position, Quaternion.identity) as GameObject;
					BoomParticleScript partscript = newpart.GetComponent<BoomParticleScript>();

					if(partscript != null)
					{
						partscript.ExpandBoom(0.5f, 0.5f, damage, 1f);
						Destroy(gameObject, 0.5f);
					}

					//StartCoroutine(ClusterBoom(0.8f, 5f, 8, 10, 0.5f));
				}
			}


			//BulletHit();


		}


	}

	IEnumerator ClusterBoom(float size, float speed, int min, int max, float delay)
	{
		yield return new WaitForSeconds(delay);

		//List<GameObject> particleList = new List<GameObject>();

		Debug.Log("min : " + min);
		Debug.Log("max : " + max);


		int num = Random.Range(min, max + 1);
		int i = 0;

		GameObject newpart = Instantiate(boomParticle, this.transform.position, Quaternion.identity) as GameObject;
		BoomParticleScript partscript = newpart.GetComponent<BoomParticleScript>();

		if(partscript != null)
		{
			partscript.expandSize = size;
		}

//		while(i < num)
//		{
//			Transform pos = this.transform;
//			pos.Rotate(0f, 0f, 360f / num * i);
//
//			GameObject newpart = Instantiate(boomParticle, this.transform.position, pos.rotation) as GameObject;
//			BoomParticleScript partscript = newpart.GetComponent<BoomParticleScript>();
//
//			//particleList.Add(newpart);
//
//			newpart.transform.parent = this.transform;
//			//newpart.transform.Rotate(0f, 0f, 360f/ (float)i);
//			newpart.transform.localScale = new Vector3(size, size, 1f);
//
//			if(partscript != null)
//			{
//				partscript.moveSpeed = speed;
//
//				Debug.Log(partscript.moveSpeed);
//			}
//
//			i++;
//		}
			
	}

	void OnTriggerStay2D(Collider2D other)
	{
		if(!other.CompareTag("Player"))
		{
			Debug.Log("Cannon Hit");

			EnemyScript enemy = other.GetComponent<EnemyScript>();

			if(bulletType == CannonType.BOOMERANGCHILD)
			{
				if(enemy != null)
				{
					if(enemy.boomChildDMG)
					{
						enemy.TakeDamage(damage);

						enemy.boomChildDMG = false;
						enemy.boomChildDMGRate = 0.5f;
					}

				}
			}
		}
	}
}
