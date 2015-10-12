using UnityEngine;
using System.Collections;

public class FollowTarget : MonoBehaviour {

	public Transform Targeting;
	
	// Update is called once per frame
	void Update () {
		this.transform.LookAt(Targeting);	
	}
}
