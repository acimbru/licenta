using UnityEngine;
using System.Collections;

public class DisableShield : MonoBehaviour {
	
	public int values;
	private float startTime;

	// Use this for initialization
	void Start () {
		startTime=Time.time;
	}
	
	// Update is called once per frame
	void Update () {
		if(this.gameObject.activeSelf==true){
				StartTimer();
		}
	}
	
	void StartTimer(){
		float timeStep = Time.time - startTime;
		float seconds = timeStep % 60;
		values = (int)seconds;
		
		if(values >=59){
			this.gameObject.SetActive(false);
		}
	}
}
