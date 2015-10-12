using UnityEngine;
using System.Collections;

public class DisableGUI : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		if(targetpractice.gameover)
			this.gameObject.SetActive(false);
	}
}
