using UnityEngine;
using System.Collections;

public class DetectHit : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
	
	void OnCollisionEnter(Collision other){
		switch(other.transform.tag){
			case "rock":this.SendMessageUpwards("HitByRock",SendMessageOptions.DontRequireReceiver);
			            break;
			
			case "fire":this.SendMessageUpwards("HitByFire",SendMessageOptions.DontRequireReceiver);
			            break;
			
			case "ice" :this.SendMessageUpwards("HitByIce",SendMessageOptions.DontRequireReceiver);
			            break;
		}
	}
}
