using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MGBulletScript : MonoBehaviour {

	float bulletSpeed = 50f;
	float lifetime = 2f;
	public float damage;
	public bool friendly;

	SpriteRenderer rend;
	float colorFlashTimer;
	float colorFlashDuration = 0.2f;

	// Use this for initialization
	void Start () {
		rend = GetComponent<SpriteRenderer>();

		colorFlashTimer = colorFlashDuration;
	}
	
	// Update is called once per frame
	void Update () 
	{
		if(PauseManagerScript.instance.isPaused)
		{
			return;
		}

		colorFlashTimer -= Time.deltaTime;

		if(!friendly)
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
				rend.color = Color.white;
			}
		}
		else
		{
			if(colorFlashTimer <= colorFlashDuration / 2)
			{
				rend.color = Color.green;

				if(colorFlashTimer <= 0f)
				{
					colorFlashTimer = colorFlashDuration;
				}
			}
			else if(colorFlashTimer <= colorFlashDuration)
			{
				rend.color = Color.white;
			}
		}

		transform.Translate (Vector3.up * Time.deltaTime * bulletSpeed );

		lifetime -= Time.deltaTime;

		if(lifetime <= 0f)
		{
			Destroy(this.gameObject);
		}
	}

	void OnTriggerEnter2D(Collider2D other)
	{
		if(friendly)
		{
			Debug.Log(other.name);

			if(other.CompareTag("Enemy"))
			{
				EnemyScript enemyscript = other.GetComponent<EnemyScript>();

				enemyscript.TakeDamage(damage);

				Destroy(gameObject);
			}
			else if(other.CompareTag("Wall"))
			{
				Destroy(gameObject);
			}
		}
		else
		{
			if(other.CompareTag("Player"))
			{
				PlayerScript player = other.GetComponent<PlayerScript>();

				player.TakeDamage(damage);

				Destroy(gameObject);
			}
			else if(other.CompareTag("Wall"))
			{
				Destroy(gameObject);
			}
		}
	}
}
