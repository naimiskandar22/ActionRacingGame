using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum CannonType
{
	NORMAL,
	SNIPER,
	SHOTGUN,
	CLUSTERNADE,
	BOOMERANG,
	ZIGZAG,
	CLUSTERCHILD,
	BOOMERANGCHILD,
	TOTAL,
}

public class PlayerScript : MonoBehaviour {

	Rigidbody2D rb;
	SpriteRenderer rend;
	Vector3 camPosition;

	public float health;
	public float maxHealth;
	public int lives;
	public int empCount;
	public GameObject empGO;

	float moveSpeed = 50f;
	float torqueForce = -200f;
	float driftFactorSlippy = 1f;
	float driftFactorSticky = 0.9f;
	float maxStickyVelocity = 2.5f;
	float minSlippyVelocity = 1.5f;

	//MG variables
	public GameObject[] mgGuns;
	public GameObject bulletPrefab;
	float mgFireRate = 3.5f;
	float mgCooldown;

	//Cannon variables
	CannonType cannonType;
	public GameObject cannonGO;
	public bool tankMode;
	public CrosshairScript crosshair;
	public Transform cannonBody;
	public Transform cannonTip;
	float rotateSpeed = 10f;
	float moveSpeedLoss = 5f;
	float mgBulletDamage = 10f;
	int[] ammoList = new int[5];

	public GameObject heavyBulletPrefab;
	float cannonFireRate = 10f;
	float cannonBulletDamage = 20f;
	float cannonCooldown;

	//Damage colour flash
	bool damageFlash = false;
	float flashTime = 0.5f;


	// Use this for initialization
	void Start () 
	{
		rend = GetComponent<SpriteRenderer>();
		rb = GetComponent<Rigidbody2D>();

		health = 100f;
		maxHealth = health;

		PlayerManagerScript.instance.playerHealth = health;
		PlayerManagerScript.instance.playerMaxHealth = maxHealth;
		PlayerManagerScript.instance.playerLives.text = "X : " + lives.ToString();
		PlayerManagerScript.instance.EMPCount.text = "X : " + empCount.ToString();

		tankMode = true;
		cannonGO.SetActive(true);
		PlayerManagerScript.instance.ammoGO.SetActive(true);

		moveSpeed /= moveSpeedLoss;
		torqueForce /= moveSpeedLoss / 2;


		for(int i = 0; i < ammoList.Length; i++)
		{
			ammoList[i] = 5;

			PlayerManagerScript.instance.ammoList[i].currAmmo = ammoList[i];
			PlayerManagerScript.instance.ammoList[i].rendRing.color = Color.clear;
		}

		cannonType = CannonType.NORMAL;
		PlayerManagerScript.instance.ammoList[(int)cannonType].rendRing.color = Color.yellow;
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

		if(Input.GetAxis("Mouse ScrollWheel") != 0)
		{
			float d = Input.GetAxis("Mouse ScrollWheel");

			if(d < 0)
			{
				PlayerManagerScript.instance.ammoList[(int)cannonType].rendRing.color = Color.clear;

				cannonType = (CannonType)((int)cannonType + 1);

				if((int)cannonType > (int)CannonType.BOOMERANG)
				{
					cannonType = CannonType.NORMAL;
				}

				PlayerManagerScript.instance.ammoList[(int)cannonType].rendRing.color = Color.yellow;

				if(ammoList[(int)cannonType] <= 0)
				{
					if(PlayerManagerScript.instance.ammoList[(int)cannonType].rendRing.color != Color.gray)
					{
						PlayerManagerScript.instance.ammoList[(int)cannonType].rendRing.color = Color.gray;
					}
				}
			}
			else if(d > 0)
			{
				PlayerManagerScript.instance.ammoList[(int)cannonType].rendRing.color = Color.clear;

				cannonType = (CannonType)((int)cannonType - 1);

				if((int)cannonType < (int)CannonType.NORMAL)
				{
					cannonType = CannonType.BOOMERANG;
				}

				PlayerManagerScript.instance.ammoList[(int)cannonType].rendRing.color = Color.yellow;

				if(ammoList[(int)cannonType] <= 0)
				{
					if(PlayerManagerScript.instance.ammoList[(int)cannonType].rendRing.color != Color.gray)
					{
						PlayerManagerScript.instance.ammoList[(int)cannonType].rendRing.color = Color.gray;
					}
				}
			}
		}

//		if(Input.GetKeyDown(KeyCode.Alpha1))
//		{
//			PlayerManagerScript.instance.ammoList[(int)cannonType].rendRing.color = Color.clear;
//
//			cannonType = (CannonType)((int)cannonType + 1);
//
//			if((int)cannonType > (int)CannonType.BOOMERANG)
//			{
//				cannonType = CannonType.NORMAL;
//			}
//
//			PlayerManagerScript.instance.ammoList[(int)cannonType].rendRing.color = Color.yellow;
//
//			if(ammoList[(int)cannonType] <= 0)
//			{
//				if(PlayerManagerScript.instance.ammoList[(int)cannonType].rendRing.color != Color.gray)
//				{
//					PlayerManagerScript.instance.ammoList[(int)cannonType].rendRing.color = Color.gray;
//				}
//			}
//
//		}
//
//		if(Input.GetKeyDown(KeyCode.Alpha2))
//		{
//			PlayerManagerScript.instance.ammoList[(int)cannonType].rendRing.color = Color.clear;
//
//			cannonType = (CannonType)((int)cannonType - 1);
//
//			if((int)cannonType < (int)CannonType.NORMAL)
//			{
//				cannonType = CannonType.BOOMERANG;
//			}
//
//			PlayerManagerScript.instance.ammoList[(int)cannonType].rendRing.color = Color.yellow;
//
//			if(ammoList[(int)cannonType] <= 0)
//			{
//				if(PlayerManagerScript.instance.ammoList[(int)cannonType].rendRing.color != Color.gray)
//				{
//					PlayerManagerScript.instance.ammoList[(int)cannonType].rendRing.color = Color.gray;
//				}
//			}
//		}

		if(Input.GetKeyDown(KeyCode.E))
		{
			FireEMP();
		}

		if(Input.GetMouseButtonDown(1))
		{

			cannonGO.SetActive(!cannonGO.activeSelf);

			PlayerManagerScript.instance.ammoGO.SetActive(!PlayerManagerScript.instance.ammoGO.activeSelf);

			tankMode = !tankMode;
			if(tankMode)
			{
				moveSpeed /= moveSpeedLoss;
				torqueForce /= moveSpeedLoss / 2;
			}
			else
			{
				moveSpeed *= moveSpeedLoss;
				torqueForce *= moveSpeedLoss / 2;
			}
		}

		if(tankMode)
		{
			CannonAimTowards();

			if(Input.GetMouseButtonDown(0))
			{
				if(cannonCooldown <= 0f)
				{
					ShootCannon();
					cannonCooldown = cannonFireRate;
				}
			}
		}

		if(Input.GetKey(KeyCode.Space))
		{
			if(mgCooldown <= 0)
			{
				ShootMG();;
				mgCooldown = mgFireRate;
			}

		}

		if(damageFlash)
		{
			flashTime -= Time.deltaTime;

			if(rend.color != Color.red)
			{
				rend.color = Color.red;
			}

			if(flashTime <= 0f)
			{
				rend.color = Color.white;
				damageFlash = false;
			}
		}

		cannonCooldown -= 1f/ cannonFireRate;
		mgCooldown -= 1f/ mgFireRate;

		camPosition = transform.position;
		camPosition.z = -5f;

		Camera.main.transform.position = camPosition;

	}

	void FixedUpdate()
	{
		if(PauseManagerScript.instance.isPaused)
		{
			return;
		}

		if(PlayerManagerScript.instance.gameWait)
		{
			return;
		}

		float tf;

		float driftFactor = driftFactorSticky;

		if(RightVelocity().magnitude > maxStickyVelocity)
		{
			driftFactor = driftFactorSlippy;
		}

		rb.velocity = ForwardVelocity() + RightVelocity() * driftFactorSlippy;

		if(Input.GetButton("Vertical"))
		{
			rb.AddForce(transform.up * moveSpeed * (int)Input.GetAxis("Vertical"));
		}

		if(!tankMode)
		{
			tf = Mathf.Lerp(0, torqueForce, rb.velocity.magnitude / 10f);
		}
		else
		{
			tf = torqueForce / 2f;
		}


		rb.angularVelocity = Input.GetAxis("Horizontal") * tf;
	}

	Vector2 ForwardVelocity()
	{
		return transform.up * Vector2.Dot(rb.velocity * 1.005f, transform.up);
	}

	Vector2 RightVelocity()
	{
		return transform.right * Vector2.Dot(rb.velocity, transform.right);
	}

	void CannonAimTowards()
	{
		Vector2 targetDir = crosshair.transform.position - cannonBody.position;
		float angle = Mathf.Atan2(targetDir.y, targetDir.x) * Mathf.Rad2Deg;
		Quaternion rotation = Quaternion.AngleAxis(angle, Vector3.forward);
		cannonBody.rotation = Quaternion.Slerp(cannonBody.rotation, rotation, rotateSpeed * Time.deltaTime);
	}

	void ShootMG()
	{
		for(int i = 0; i < mgGuns.Length; i++)
		{
			SoundManagerScript.Instance.PlaySFX(AudioClipID.SFX_MG);

			GameObject newBullet = Instantiate(bulletPrefab, mgGuns[i].transform.position, this.transform.rotation);
			MGBulletScript bulletscript = newBullet.GetComponent<MGBulletScript>();

			bulletscript.damage = mgBulletDamage;
			bulletscript.friendly = true;
		}
	}

	void ShootCannon()
	{
		if(ammoList[(int)cannonType] > 0)
		{
			GameObject newBullet01 = Instantiate(heavyBulletPrefab, cannonTip.position, cannonTip.rotation);

			CannonBulletScript bulletscript01 = newBullet01.GetComponent<CannonBulletScript>();

			bulletscript01.bulletType = cannonType;
			bulletscript01.damage = cannonBulletDamage;

			rb.AddForce(cannonTip.up * (-1f) * 5f, ForceMode2D.Impulse);

			ammoList[(int)cannonType] -= 1;
			PlayerManagerScript.instance.ammoList[(int)cannonType].currAmmo--;

			if(ammoList[(int)cannonType] <= 0)
			{
				if(PlayerManagerScript.instance.ammoList[(int)cannonType].rendRing.color != Color.gray)
				{
					PlayerManagerScript.instance.ammoList[(int)cannonType].rendRing.color = Color.gray;
				}
			}

			if(cannonType == CannonType.SNIPER)
			{
				SoundManagerScript.Instance.PlaySFX(AudioClipID.SFX_SNIPER);

				bulletscript01.bulletSpeed = bulletscript01.bulletSpeed * 1.5f;
				bulletscript01.damage = bulletscript01.damage * 1.5f;
			}

			if(cannonType == CannonType.SHOTGUN)
			{
				SoundManagerScript.Instance.PlaySFX(AudioClipID.SFX_SHOTGUN);

				bulletscript01.bulletType = CannonType.NORMAL;

				GameObject newBullet02 = Instantiate(heavyBulletPrefab, cannonTip.position, cannonTip.rotation);

				CannonBulletScript bulletscript02 = newBullet02.GetComponent<CannonBulletScript>();
				bulletscript02.bulletType = CannonType.NORMAL;
				bulletscript02.damage = cannonBulletDamage;
				newBullet02.transform.Rotate(0f, 0f, -30f);

				GameObject newBullet03 = Instantiate(heavyBulletPrefab, cannonTip.position, cannonTip.rotation);

				CannonBulletScript bulletscript03 = newBullet03.GetComponent<CannonBulletScript>();
				bulletscript03.bulletType = CannonType.NORMAL;
				bulletscript03.damage = cannonBulletDamage;
				newBullet03.transform.Rotate(0f, 0f, 30f);
			}
			else
			{
				SoundManagerScript.Instance.PlaySFX(AudioClipID.SFX_CANNON00);
			}
		}

	}

	public void TakeDamage(float damage)
	{
		health -= damage;

		if(health <= 0f)
		{
			if(lives > 0)
			{
				lives--;

				PlayerManagerScript.instance.GameWait();
				PlayerManagerScript.instance.Invoke("GameWait", 2f);
				PlayerManagerScript.instance.playerLives.text = "X : " + lives.ToString();

				health = maxHealth;
			}
			else
			{
				PlayerManagerScript.instance.GameWait();
				PlayerManagerScript.instance.StartCoroutine(PlayerManagerScript.instance.FadeIn(PlayerManagerScript.instance.blackScreen, 4f, 1f));
				PlayerManagerScript.instance.winLose.text = "YOU LOSE";

				Cursor.visible = true;

				PlayerManagerScript.instance.Invoke("ActiveExit", 4f);
			}
		}

		PlayerManagerScript.instance.playerHealth = health;

		flashTime = 0.25f;

		if(!damageFlash)
		{
			damageFlash = true;
		}
	}

	void FireEMP()
	{
		if(empCount > 0)
		{
			empCount--;

			SoundManagerScript.Instance.StopAllCoroutines();

			SoundManagerScript.Instance.StartCoroutine(SoundManagerScript.Instance.BGMFadeVolume(0f, 0.1f, 0f));

			SoundManagerScript.Instance.PlaySFX(AudioClipID.SFX_EMP);

			PlayerManagerScript.instance.EMPCount.text = "X : " + empCount.ToString();

			for(int i = 0; i < 3; i++)
			{
				GameObject newemp = Instantiate(empGO, transform.position, Quaternion.identity) as GameObject;
				BoomParticleScript empScript = newemp.GetComponent<BoomParticleScript>();

				if(empScript != null)
				{
					empScript.emp = true;
					empScript.ExpandBoom(4f - i - 0.5f, 0f, 0f, 4f / (i+1));
					empScript.GetComponent<SpriteRenderer>().color = Color.cyan;
				}
			}

			SoundManagerScript.Instance.StartCoroutine(SoundManagerScript.Instance.BGMFadeVolume(0.1f, 1f, 2f));

		}
	}

//	void DamageFlash()
//	{
//		Debug.Log("DMG Flash");
//
//		float t = 100f;
//
//		bool exit = false;
//
//		while(t > 0f)
//		{
//			t -= Time.deltaTime;
//
//			if(rend.color != Color.red)
//			{
//				rend.color = Color.red;
//			}
//		}
//
//
//		if(t <= 0f)
//		{
//			exit = true;
//			rend.color = Color.white;
//		}
//
//	}
}
