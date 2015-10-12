using UnityEngine;
using System.Collections;

public class center : MonoBehaviour {
  	public GameObject parinte;
	public Vector3 spawnpoint=new Vector3(0,0,12);
	// Use this for initialization
	void Start () {
		RandomPositions();
	}
	
	// Update is called once per frame
	void Update () {
	
	}
	void OnCollisionEnter(Collision col){
		if(col.gameObject.tag=="rock")	
		{			
			targetpractice.scor+=3;	
			RandomPositions();
			//parinte.transform.localScale=new Vector3(2,2,2);
		}
	}
	
	void RandomPositions(){
		float newx=spawnpoint.x + Random.Range(-16.0f,16.0f);
		float newz=spawnpoint.z + Random.Range(-9.0f,10.0f);
		float angle=Random.Range(10,90);
		Vector3 newpos=new Vector3(newx,parinte.transform.localPosition.y,newz);			
		parinte.transform.localPosition=newpos;
		transform.parent.localEulerAngles=new Vector3(90,-angle,0);
	}
}
