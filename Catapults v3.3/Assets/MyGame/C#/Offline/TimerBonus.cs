using UnityEngine;
using System.Collections;

public class TimerBonus : MonoBehaviour {

	private float BonusTime = 30;
	private TimerSingle timer;
	private Vector3 position = new Vector3(0,0,0);
	
	// Use this for initialization
	void Start () {
		GameObject scor = GameObject.FindGameObjectWithTag("scorSingle");
		timer = scor.GetComponent<TimerSingle>();
		RandomPos();
	}
	
	// Update is called once per frame
	void Update () {
	
	}
	
	void OnTriggerEnter(Collider other){		
		if(BonusTime>=0){
			timer.timeleft += BonusTime;
			BonusTime -=2;
			RandomPos();
		}
	}
	
	void RandomPos(){
		this.transform.localPosition = new Vector3(Random.Range (-15.0f,15.0f),Random.Range (4.0f,10.0f),Random.Range(-6.0f,6.0f));	
	}
}