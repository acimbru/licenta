using UnityEngine;
using System.Collections;

public class WindPhysics : MonoBehaviour {

	// Use this for initialization
	public Vector3 wind;
	
	public bool canBeShot=false;
	
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		//if(canBeShot){
		ApplyWindForce();
		//}
	}
	
	void ApplyWindForce(){		
		rigidbody.AddForce(wind * Time.deltaTime);
	}
	
	[RPC]
	void WindData(object[] param){
		wind = (int)param[0] * (Vector3)param[1];
		print ("received wind");
	}
}
