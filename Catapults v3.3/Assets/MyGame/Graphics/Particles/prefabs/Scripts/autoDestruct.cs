﻿using UnityEngine;
using System.Collections;

public class autoDestruct : MonoBehaviour {
	public int destroyTime=3;
	// Use this for initialization
	void Start () {
		Destroy(gameObject,destroyTime);
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}