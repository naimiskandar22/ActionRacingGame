using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CrosshairScript : MonoBehaviour {

	public Vector3 rawMousePos;
	public Vector3 worldMousePos;

	void Start()
	{
		Cursor.visible = false;
	}

	// Update is called once per frame
	void Update () {

		rawMousePos = Input.mousePosition;
		worldMousePos = Camera.main.ScreenToWorldPoint(rawMousePos);
		worldMousePos.z = 0f;

		this.transform.position = worldMousePos;
	}
}
