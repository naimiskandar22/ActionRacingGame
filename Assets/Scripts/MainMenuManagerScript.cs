using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MainMenuManagerScript : MonoBehaviour {

	public GameObject tutorialGO00;
	public GameObject tutorialGO01;

	public GameObject soundManagerPrefab;

	public Image blackScreen;

	void Awake()
	{
		GameObject[] soundManager = GameObject.FindGameObjectsWithTag("SoundManager");

		if(soundManager.Length == 0)
		{
			Instantiate(soundManagerPrefab, transform.position, Quaternion.identity);
		}

//		if(soundManager.Length > 1)
//		{
//			for(int i = 0; i < soundManager.Length; i++)
//			{
//				if(soundManager[i] != null)
//				{
//					SoundManagerScript script = soundManager[i].GetComponent<SoundManagerScript>();
//
//					if(!script.first)
//					{
//						Destroy(script.gameObject);
//					}
//					else
//					{
//						soundManagerScript = script;
//					}
//				}
//			}	
//		}
//		else
//		{
//			soundManagerScript = soundManager[0].GetComponent<SoundManagerScript>();
//		}

		FadeMainMenu();
	}

	// Use this for initialization
	void Start () 
	{
//		if(tutorialGO00.activeSelf)
//		{
//			tutorialGO00.SetActive(false);
//		}
//		if(tutorialGO01.activeSelf)
//		{
//			tutorialGO01.SetActive(false);
//		}
	}
	
	// Update is called once per frame
	void Update () 
	{
		if(!blackScreen.gameObject.activeSelf)
		{
			if(Input.GetKeyDown(KeyCode.Escape))
			{
				if(tutorialGO01.activeSelf)
				{
					SoundManagerScript.Instance.PlaySFX(AudioClipID.SFX_UI_BUTTON);

					tutorialGO01.SetActive(false);
				}
				else if(tutorialGO00.activeSelf)
				{
					SoundManagerScript.Instance.PlaySFX(AudioClipID.SFX_UI_BUTTON);

					tutorialGO00.SetActive(false);
				}

			}
			else if(Input.GetKeyDown(KeyCode.Space))
			{
				if(tutorialGO01.activeSelf)
				{
					SoundManagerScript.Instance.PlaySFX(AudioClipID.SFX_UI_BUTTON);

					SoundManagerScript.Instance.StartCoroutine(SoundManagerScript.Instance.BGMFadeVolume(0f, 2f, 0f));
					SoundManagerScript.Instance.bgmAudioSource.loop = false;
					Invoke("LoadNextScene", 2f);
					blackScreen.gameObject.SetActive(true);
					StartCoroutine(FadeIn(4f, 1f));
				}
				else if(tutorialGO00.activeSelf)
				{
					SoundManagerScript.Instance.PlaySFX(AudioClipID.SFX_UI_BUTTON);

					tutorialGO01.SetActive(true);
				}

			}
		}
	}

	void FadeMainMenu()
	{
		StartCoroutine(FadeIn(2f, 0f));
		SoundManagerScript.Instance.bgmVolume = 0f;
		SoundManagerScript.Instance.StartCoroutine(SoundManagerScript.Instance.BGMFadeVolume(0.1f, 8f, 0f));
		SoundManagerScript.Instance.PlayBGM(AudioClipID.BGM_MAINMENU);
	}

	void LoadNextScene()
	{
		SceneManager.LoadScene("Scene01");
		SoundManagerScript.Instance.StartCoroutine(SoundManagerScript.Instance.BGMFadeVolume(0.1f, 5f, 0f));
		SoundManagerScript.Instance.PlayBGM(AudioClipID.BGM_LEVELINTRO);
	}

	public void ExitGame()
	{
		Debug.Log("Exit");
		Application.Quit();
	}

	public IEnumerator FadeIn(float time, float end)
	{
		float a = blackScreen.color.a;

		for(float t = 0f; t < 1f; t+= Time.deltaTime / time)
		{
			Color newColor = new Color(0f, 0f, 0f, Mathf.Lerp(a, end, t));
			blackScreen.color = newColor;
			yield return null;
		}

		if(blackScreen.color.a <= 0.1f)
		{
			blackScreen.gameObject.SetActive(false);
		}
	}
}
