using UnityEngine;
using System.Collections;

public class DestroyOnCollision : MonoBehaviour {
	
	public GameObject impactObject;
	public Transform GameWorld;
	
	void Start(){
		if(Application.loadedLevelName!="about_scene")
		{
		GameWorld = GameObject.FindGameObjectWithTag("GameWorld").transform;
		}
	}
	
	void OnCollisionEnter(Collision col){
		if(Application.loadedLevelName!="about_scene")
		{
			var clone = Instantiate(impactObject,this.transform.position,this.transform.rotation) as GameObject;
			clone.transform.parent = GameWorld;
		
			Destroy(this.gameObject);
		}
	}
}
