using UnityEngine;
using System.Collections;

public class Freeze : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
	
	[RPC]
	void ShowIceTexture(bool state){
		this.gameObject.renderer.enabled = state;
	}
}
