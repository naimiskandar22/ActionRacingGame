using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WaveManagerScript : MonoBehaviour {

	public static WaveManagerScript instance;

	public PlatformScript[] platforms;

	public PlayerScript player;
	public EnemyType enemyType;
	public GameObject[] enemyList;

	public int currWave;
	int endWave = 0;

	public List<EnemyScript> liveEnemyList = new List<EnemyScript>();
	public float liveEnemyHealth;
	public float enemyMaxHealth;

	public Image liveEnemyBar;
	public Text liveEnemyNum;

	public GameObject waveRingGO;
	public GameObject needlePrefab;
	List<GameObject> needleList = new List<GameObject>();

	void Awake()
	{
		if(instance == null) instance = this;
	}

	// Use this for initialization
	void Start () 
	{
		for(int i = 0; i < 360; i++)
		{
			GameObject go = Instantiate(needlePrefab, Vector3.zero, Quaternion.identity) as GameObject;
			waveRingGO.transform.Rotate(0f, 0f, 1f);
			go.transform.parent = waveRingGO.transform;
			go.transform.Rotate(0f, 0f, -90f);
			go.transform.localPosition = new Vector3(Mathf.Cos(Mathf.Deg2Rad * -i) * 25f, Mathf.Sin(Mathf.Deg2Rad * -i) * 25f, 0f);
			//go.transform.localPosition = new Vector3(Mathf.Cos(Mathf.Deg2Rad * -i + 90f * Mathf.Deg2Rad) * 25f, Mathf.Sin(Mathf.Deg2Rad * -i + 90f * Mathf.Deg2Rad) * 25f, 0f);
			//go.transform.localPosition = Vector3.zero;

			go.GetComponent<Image>().fillAmount = 0f;

//			Transform rot = needlePrefab.transform;
//			rot.Rotate(0f, 0f, i * 1.0f);
//
//			go.transform.localRotation = rot.rotation;

			needleList.Add(go);

			if(!go.activeSelf)
			{
				go.SetActive(true);
			}
		}
	}
	
	// Update is called once per frame
	void Update () 
	{
		if(PlayerManagerScript.instance.gameWait)
		{
			return;
		}

		EnemyCompass();

		if(currWave > endWave && liveEnemyList.Count <= 0)
		{
			PlayerManagerScript.instance.GameWait();
			PlayerManagerScript.instance.StartCoroutine(PlayerManagerScript.instance.FadeIn(PlayerManagerScript.instance.blackScreen, 4f, 1f));
			PlayerManagerScript.instance.winLose.text = "YOU WIN";

			PlayerManagerScript.instance.Invoke("ActiveExit", 4f);

			Cursor.visible = true;
		}
		else if(liveEnemyHealth <= 0f)
		{
			SpawnEnemies();
		}


		if(liveEnemyBar.fillAmount != liveEnemyHealth / enemyMaxHealth)
		{
			liveEnemyBar.fillAmount = liveEnemyHealth / enemyMaxHealth;
		}

		if(liveEnemyNum.text != liveEnemyList.Count.ToString())
		{
			liveEnemyNum.text = liveEnemyList.Count.ToString();
		}
	}

	void SpawnEnemies()
	{
		liveEnemyHealth = 0f;
		enemyMaxHealth = 0f;

		for(int i = 0; i < platforms.Length; i++)
		{
			platforms[i].SpawnEnemies(currWave * 3 + 4);
		}

		currWave++;
	}

	void EnemyCompass()
	{
		for(int i = 0; i < needleList.Count; i++)
		{
			needleList[i].GetComponent<Image>().fillAmount = 0f;
		}

		for(int i = 0; i < liveEnemyList.Count; i++)
		{
			float dis = Vector2.Distance(player.transform.position, liveEnemyList[i].transform.position);
			Vector3 pos = liveEnemyList[i].transform.position - player.transform.position;

			int angle = (int)Mathf.Round((Mathf.Atan2(pos.y, pos.x) * Mathf.Rad2Deg));

			if(angle > 0)
			{
				angle = 360 - angle;

				if(needleList[angle].GetComponent<Image>().fillAmount < (dis / 20f))
				{
					needleList[angle].GetComponent<Image>().fillAmount = dis / 20f;
				}

				for(int j = 1; j < 10; j++)
				{
					if(angle + j < needleList.Count)
					{
						needleList[angle + j].GetComponent<Image>().fillAmount = (needleList[angle + j - 1].GetComponent<Image>().fillAmount + needleList[angle + j].GetComponent<Image>().fillAmount) / 2f;
					}
					else
					{
						int temp = angle + j - needleList.Count;

						if(temp - 1 < 0)
						{
							needleList[temp].GetComponent<Image>().fillAmount = (needleList[needleList.Count - 1].GetComponent<Image>().fillAmount + needleList[temp].GetComponent<Image>().fillAmount) / 2f;
						}
						else
						{
							needleList[temp].GetComponent<Image>().fillAmount = (needleList[temp - 1].GetComponent<Image>().fillAmount + needleList[temp].GetComponent<Image>().fillAmount) / 2f;
						}
					}
				}

				for(int j = -1; j > -10; j--)
				{
					if(angle + j > 0)
					{
						needleList[angle + j].GetComponent<Image>().fillAmount = (needleList[angle + j + 1].GetComponent<Image>().fillAmount + needleList[angle + j].GetComponent<Image>().fillAmount) / 2f;
					}
					else
					{
						int temp = needleList.Count + j;

						if(temp + 1 == needleList.Count)
						{
							needleList[temp].GetComponent<Image>().fillAmount = (needleList[0].GetComponent<Image>().fillAmount + needleList[temp].GetComponent<Image>().fillAmount) / 2f;
						}
						else
						{
							needleList[temp].GetComponent<Image>().fillAmount = (needleList[temp - 1].GetComponent<Image>().fillAmount + needleList[temp].GetComponent<Image>().fillAmount) / 2f;
						}
					}
				}
			}
			else
			{
				if(needleList[angle * -1].GetComponent<Image>().fillAmount < (dis / 20f))
				{
					needleList[angle * -1].GetComponent<Image>().fillAmount = dis / 20f;
				}

				for(int j = 1; j < 10; j++)
				{
					if(angle * -1 + j < needleList.Count)
					{
						needleList[angle * -1 + j].GetComponent<Image>().fillAmount = (needleList[angle * -1 + j - 1].GetComponent<Image>().fillAmount + needleList[angle * -1 + j].GetComponent<Image>().fillAmount) / 2f;
					}
					else
					{
						int temp = angle * -1 + j - needleList.Count;

						if(temp - 1 < 0)
						{
							needleList[temp].GetComponent<Image>().fillAmount = (needleList[needleList.Count - 1].GetComponent<Image>().fillAmount + needleList[temp].GetComponent<Image>().fillAmount) / 2f;
						}
						else
						{
							needleList[temp].GetComponent<Image>().fillAmount = (needleList[temp - 1].GetComponent<Image>().fillAmount + needleList[temp].GetComponent<Image>().fillAmount) / 2f;
						}
					}
				}

				for(int j = -1; j > -10; j--)
				{
					if(angle * -1 + j > 0)
					{
						needleList[angle * -1 - j].GetComponent<Image>().fillAmount = (needleList[angle * -1 - j + 1].GetComponent<Image>().fillAmount + needleList[angle * -1 - j].GetComponent<Image>().fillAmount) / 2f;
					}
					else
					{
						int temp = needleList.Count + j;

						if(temp + 1 == needleList.Count)
						{
							needleList[temp].GetComponent<Image>().fillAmount = (needleList[0].GetComponent<Image>().fillAmount + needleList[temp].GetComponent<Image>().fillAmount) / 2f;
						}
						else
						{
							needleList[temp].GetComponent<Image>().fillAmount = (needleList[temp - 1].GetComponent<Image>().fillAmount + needleList[temp].GetComponent<Image>().fillAmount) / 2f;
						}
					}
				}
			}

			Debug.Log(angle);


		}
	}
}
