using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

[System.Serializable]
public struct AmmoHandler
{
	public CannonType gunType;
	public int currAmmo;
	public Image rendRing;
	public Image rendIcon;
	public Image ammoBar;
	public Image ammoBarBorder;
}

public class PlayerManagerScript : MonoBehaviour {

	public static PlayerManagerScript instance;

	public AmmoHandler[] ammoList;
	public int maxAmmo;

	public GameObject ammoGO;

	public Image playerHealthBar;
	public float playerHealth;
	public float playerMaxHealth;
	public Text playerLives;

	public Text EMPCount;

	public bool gameWait = false;
	public Image blackScreen;
	public Image blackScreen01;
	public Text winLose;
	public GameObject exitButton;

	void Awake()
	{
		if(instance == null) instance = this;
	}

	// Use this for initialization
	void Start () 
	{
		gameWait = true;
		StartCoroutine(FadeIn(blackScreen, 8f, 0f));
		Invoke("GameWait", 4f);

		maxAmmo = 10;
	}
	
	// Update is called once per frame
	void Update () 
	{
		if(SoundManagerScript.Instance.bgmAudioSource.clip == SoundManagerScript.Instance.FindAudioClip(AudioClipID.BGM_LEVELINTRO))
		{
			if(!SoundManagerScript.Instance.bgmAudioSource.isPlaying)
			{
				SoundManagerScript.Instance.bgmAudioSource.loop = true;
				SoundManagerScript.Instance.PlayBGM(AudioClipID.BGM_LEVEL);
			}
		}

		if(PauseManagerScript.instance.isPaused)
		{
			return;
		}

		for(int i = 0; i < ammoList.Length; i++)
		{
			if(ammoList[i].ammoBar.fillAmount != ((float)ammoList[i].currAmmo / (float)maxAmmo))
			{
				ammoList[i].ammoBar.fillAmount = ((float)ammoList[i].currAmmo / (float)maxAmmo);
			}

			if(ammoList[i].currAmmo <= 0)
			{
				if(ammoList[i].ammoBarBorder.color != Color.gray)
				{
					ammoList[i].ammoBarBorder.color = Color.gray;
				}

				if(ammoList[i].rendIcon.color != Color.gray)
				{
					ammoList[i].rendIcon.color = Color.gray;
				}
			}
			else
			{
				if(ammoList[i].ammoBarBorder.color != Color.white)
				{
					ammoList[i].ammoBarBorder.color = Color.white;
				}

				if(ammoList[i].rendIcon.color != Color.white)
				{
					ammoList[i].rendIcon.color = Color.white;
				}
			}
		}

		if(playerHealthBar.fillAmount != ((float)playerHealth / (float)playerMaxHealth))
		{
			playerHealthBar.fillAmount = ((float)playerHealth / (float)playerMaxHealth);
		}
	}

	public IEnumerator FadeIn(Image screen, float time, float end)
	{
		float a = screen.color.a;

		for(float t = 0f; t < 1f; t+= Time.deltaTime / time)
		{
			Color newColor = new Color(0f, 0f, 0f, Mathf.Lerp(a, end, t));
			screen.color = newColor;
			yield return null;
		}
	}

	public void GameWait()
	{
		if(gameWait)
		{
			gameWait = !gameWait;
		}
		else
		{
			gameWait = !gameWait;
		}
	}

	public void ActiveExit()
	{
		exitButton.SetActive(true);
	}

	public void DeactiveExit()
	{
		exitButton.SetActive(false);
	}

	public void FadeToMainMenu()
	{
		SoundManagerScript.Instance.PlaySFX(AudioClipID.SFX_UI_BUTTON);

		//PauseManagerScript.instance.isPaused = false;
		Time.timeScale = 1.0f;
		Debug.Log("MainMenu00");
		SoundManagerScript.Instance.StartCoroutine(SoundManagerScript.Instance.BGMFadeVolume(0f, 1.5f, 0f));
		Debug.Log("MainMenu01");
		if(!SoundManagerScript.Instance.first)
		{
			Debug.Log("MainMenu02");
			SoundManagerScript.Instance.first = true;
		}
		Debug.Log("MainMenu03");
		blackScreen01.gameObject.SetActive(true);
		Debug.Log("MainMenu04");
		StartCoroutine(FadeIn(blackScreen01, 2f, 1f));
		Debug.Log("MainMenu05");
		Invoke("GoToMainMenu", 2.5f);
	}

	public void GoToMainMenu()
	{

		SceneManager.LoadScene("MainMenu");
	}
}
