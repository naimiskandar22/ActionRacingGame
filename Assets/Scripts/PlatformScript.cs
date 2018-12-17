using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlatformScript : MonoBehaviour {

	public GameObject spawnerBody;
	public GameObject spawnerArm;
	public GameObject spawnerTip;

	WaveManagerScript waveManager;
	int enemySpawn;

	// Use this for initialization
	void Start () 
	{
		spawnerBody.SetActive(false);

		waveManager = WaveManagerScript.instance;
	}
	
	// Update is called once per frame
	void Update () 
	{
		if(PlayerManagerScript.instance.gameWait)
		{
			return;
		}
	}

	public void SpawnEnemies(int enemy)
	{
		SetActiveSelf();

		spawnerArm.transform.rotation = Quaternion.identity;

		spawnerArm.transform.Rotate(0f, 0f, Random.Range(0f, 360f));

		enemySpawn = enemy;

		for(int i = 0; i < enemy; i++)
		{
			Invoke("SendEnemy", 1f * i);
		}

		Invoke("SetActiveSelf", enemy * 1f);
	}

	public void SendEnemy()
	{
		spawnerArm.transform.Rotate(0f, 0f, 360f / enemySpawn);

		//EnemyType type = (EnemyType)Random.Range(waveManager.currWave * 2, waveManager.currWave * 2 + 6);
		//Temporary
		EnemyType type = (EnemyType)Random.Range(0, (int)EnemyType.TOTAL);

		GameObject newenemy = Instantiate(waveManager.enemyList[(int)type], spawnerTip.transform.position, spawnerTip.transform.rotation) as GameObject;

		EnemyScript enemyScript = newenemy.GetComponent<EnemyScript>();
		Rigidbody2D rgd = newenemy.GetComponent<Rigidbody2D>();

		if(rgd != null)
		{
			rgd.AddForce(spawnerTip.transform.up * 200f * (int)type);
		}

		if(enemyScript != null)
		{
			enemyScript.player = waveManager.player;
			enemyScript.type = type;
			enemyScript.health = ((int)type + 1) * 20;

			waveManager.liveEnemyHealth += enemyScript.health;
			waveManager.enemyMaxHealth += enemyScript.health;
			waveManager.liveEnemyList.Add(enemyScript);

			enemyScript.Invoke("KinematicRB", (waveManager.currWave + 1) * 2f);
		}
	}

	public void SetActiveSelf()
	{
		if(spawnerBody.activeSelf)
		{
			spawnerBody.SetActive(false);
		}
		else
		{
			spawnerBody.SetActive(true);
		}
	}

}
