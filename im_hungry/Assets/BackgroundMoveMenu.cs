using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackgroundMoveMenu : MonoBehaviour
{
	public float speed, clamppos;
	Vector3 startpos;

	// Use this for initialization
	void Start()
	{
		startpos = transform.position;
	}

	// Update is called once per frame
	void Update()
	{
		float newpos = Mathf.Repeat(Time.time * speed, clamppos);
		transform.position = startpos + Vector3.left * newpos + Vector3.up * newpos;
	}
}