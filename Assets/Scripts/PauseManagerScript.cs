using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PauseManagerScript : MonoBehaviour {

	public static PauseManagerScript instance;

	public bool isPaused = false;

	public GameObject pauseMenu;

	void Awake()
	{
		if(instance == null)
		{
			instance = this;
		}
	}

	// Use this for initialization
	void Start () 
	{
		
	}
	
	// Update is called once per frame
	void Update () 
	{
		if(Input.GetKeyDown(KeyCode.Escape))
		{
			SoundManagerScript.Instance.PlaySFX(AudioClipID.SFX_UI_BUTTON);

			isPaused = !isPaused;

			if(isPaused)
			{
				Cursor.visible = true;
				PlayerManagerScript.instance.ActiveExit();
				pauseMenu.SetActive(true);
				Time.timeScale = 0.0f;
			}
			else
			{
				Cursor.visible = false;
				PlayerManagerScript.instance.DeactiveExit();
				pauseMenu.SetActive(false);
				Time.timeScale = 1.0f;
			}
		}
	}
}
