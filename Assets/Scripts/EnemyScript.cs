using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum EnemyType
{
	TYPE00,
	TYPE01,
	TYPE02,
	TYPE03,
	TYPE04,
	TYPE05,
	TYPE06,
	TYPE07,
	TYPE08,
	TYPE09,
	TOTAL,
};

public class EnemyScript : MonoBehaviour {

	public PlayerScript player;
	SpriteRenderer rend;
	public float health;
	bool damageFlash;
	float flashTime;

	public EnemyType type;
	public float moveSpeed;
	public float fireRate;
	public float fireCooldown;
	public bool canMove;
	bool canShoot = true;

	public Transform cannonBody;
	public GameObject bulletPrefab;
	public float rotateSpeed;
	public GameObject[] gunTips;
	public float firingRange;
	public float bulletVolley;
	float bulletBurstCount;

	public float bulletDamage;
	public float boomChildDMGRate = 0.5f;
	public bool boomChildDMG = true;

	public GameObject boomParticle;

	// Use this for initialization
	void Start () 
	{
		rend = GetComponent<SpriteRenderer>();
		fireCooldown = fireRate;
	}
	
	// Update is called once per frame
	void Update () 
	{
		if(PauseManagerScript.instance.isPaused)
		{
			return;
		}

		if(PlayerManagerScript.instance.gameWait)
		{
			return;
		}

		if(damageFlash)
		{
			flashTime -= Time.deltaTime;

			if(rend.color != Color.cyan)
			{
				rend.color = Color.cyan;
			}

			if(flashTime <= 0f)
			{
				rend.color = Color.white;
				damageFlash = false;
			}
		}

		if(!boomChildDMG)
		{
			boomChildDMGRate -= Time.deltaTime;

			if(boomChildDMGRate <= 0f)
			{
				boomChildDMG = true;
			}
		}

		if(canMove)
		{
			transform.Rotate(0, 0, 1f);
		}

		fireCooldown -= 1f/ fireRate;

		CheckPlayerPos();
	}

	void CheckPlayerPos()
	{
		if(Vector2.Distance(player.transform.position, cannonBody.position) <= firingRange)
		{
			CannonAimTowards();
		}
	}

	void CannonAimTowards()
	{
		Vector2 targetDir = player.transform.position - cannonBody.position;
		float angle = Mathf.Atan2(targetDir.y, targetDir.x) * Mathf.Rad2Deg;
		Quaternion rotation = Quaternion.AngleAxis(angle, Vector3.forward);
		cannonBody.rotation = Quaternion.Slerp(cannonBody.rotation, rotation, rotateSpeed * Time.deltaTime);

		if(canShoot)
		{
			if(fireCooldown <= 0)
			{
				ShootGun();
			}
		}
	}

	void ShootGun()
	{
		canShoot = false;

		SoundManagerScript.Instance.PlaySFX(AudioClipID.SFX_CANNON01);

		if((int)type >= (int)EnemyType.TYPE00 && (int)type <= (int)EnemyType.TYPE05)// || ((int)type >= (int)EnemyType.TYPE07 && (int)type <= (int)EnemyType.TYPE09))
		{
			
			for(int i = 0; i < gunTips.Length; i++)
			{
				GameObject go = Instantiate(bulletPrefab, gunTips[i].transform.position, gunTips[i].transform.rotation);

				MGBulletScript bulletScript = go.GetComponent<MGBulletScript>();

				bulletScript.friendly = false;
				bulletScript.damage = bulletDamage;
			}

			bulletBurstCount++;

			if(bulletBurstCount < bulletVolley)
			{
				fireCooldown = fireRate * 0.1f; 
			}
			else
			{
				bulletBurstCount = 0f;
				fireCooldown = fireRate;
			}

			canShoot = true;

		}
		else if((int)type >= (int)EnemyType.TYPE06 && (int)type <= (int)EnemyType.TYPE09)
		{
			
			for(int i = 0; i < gunTips.Length; i++)
			{
				GameObject go = Instantiate(bulletPrefab, gunTips[i].transform.position, gunTips[i].transform.rotation);

				MGBulletScript bulletScript = go.GetComponent<MGBulletScript>();

				bulletScript.friendly = false;
			}

			bulletBurstCount++;

			cannonBody.Rotate(0f, 0f, (90f / bulletVolley));

			if(bulletBurstCount < bulletVolley)
			{
				fireCooldown = fireRate * 0.2f; 
			}
			else
			{
				bulletBurstCount = 0f;
				fireCooldown = fireRate;
			}

			canShoot = true;


		}
	}

	public void TakeDamage(float damage)
	{
		flashTime = 0.25f;

		if(!damageFlash)
		{
			damageFlash = true;
		}

		if((health - damage) < 0f)
		{
			float temp = health - damage;

			health = 0f;
			WaveManagerScript.instance.liveEnemyHealth -= (damage - Mathf.Abs(temp));
		}
		else
		{
			WaveManagerScript.instance.liveEnemyHealth -= damage;
			health -= damage;
		}

		if(health <= 0f)
		{
			WaveManagerScript.instance.liveEnemyList.Remove(this);

			GameObject go = Instantiate(boomParticle, transform.position, Quaternion.identity);

			BoomParticleScript goScript = go.GetComponent<BoomParticleScript>();
			goScript.GetComponent<SpriteRenderer>().color = Color.red;
			goScript.ExpandBoom(0.75f, 0f, 0f, 2.5f);
			goScript.coll.enabled = false;

			Destroy(gameObject);

			SoundManagerScript.Instance.PlaySFX(AudioClipID.SFX_DEATH);
		}
	}

	public void KinematicRB()
	{
		Rigidbody2D rb = this.GetComponent<Rigidbody2D>();

		if(rb != null)
		{
			rb.bodyType = RigidbodyType2D.Kinematic;
			rb.velocity = Vector3.zero;
			rb.angularVelocity = 0f;
		}

		Debug.Log("KINRB");
	}

	public IEnumerator HitEMP()
	{
		for(int i = 0; i < 5; i++)
		{
			yield return new WaitForSeconds(0.5f * i);

			float randX = Random.Range(transform.position.x - rend.bounds.extents.x, transform.position.x + rend.bounds.extents.x);
			float randY = Random.Range(transform.position.y - rend.bounds.extents.y, transform.position.y + rend.bounds.extents.y);

			Vector3 vect = new Vector3(randX, randY, 0f);

			GameObject go = Instantiate(boomParticle, vect, Quaternion.identity);

			go.transform.parent = transform;

			BoomParticleScript goScript = go.GetComponent<BoomParticleScript>();
			goScript.GetComponent<SpriteRenderer>().color = Color.cyan;
			goScript.ExpandBoom(0.2f, 0f, 0f, 2.5f);
			goScript.coll.enabled = false;
		}
	}
}
