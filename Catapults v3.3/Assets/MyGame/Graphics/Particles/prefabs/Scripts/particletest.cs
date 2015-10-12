using UnityEngine;
using System.Collections;

public class particletest : MonoBehaviour {
	public Transform firehit;
	public Transform icehit;
	public Transform hitBonus;
	public Transform hitPlayer;
	public Transform hitGround;
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
	void OnGUI(){
		if(GUI.Button(new Rect(20,20,80,20),"fire")){
			var clone = Instantiate(firehit ,this.transform.position, this.transform.rotation) as Transform;	
		}
		
		if(GUI.Button(new Rect(20,40,80,20),"ice")){
			var clone = Instantiate(icehit ,this.transform.position, Quaternion.Euler(90,0,0)) as Transform;	
		}
		if(GUI.Button(new Rect(20,60,80,20),"bonus")){
			var clone = Instantiate(hitBonus ,this.transform.position, Quaternion.Euler(0,0,0)) as Transform;	
		}
		if(GUI.Button(new Rect(20,80,80,20),"hitplayer")){
			var clone = Instantiate(hitPlayer ,this.transform.position, Quaternion.Euler(90,0,0)) as Transform;	
		}
		if(GUI.Button(new Rect(20,100,80,20),"hitground")){
			var clone = Instantiate(hitGround ,this.transform.position, Quaternion.Euler(90,0,0)) as Transform;	
		}
	}	
}
