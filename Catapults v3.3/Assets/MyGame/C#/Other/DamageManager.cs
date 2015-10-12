using UnityEngine;
using System.Collections;

public class DamageManager : MonoBehaviour {
	
	public Texture2D LostMsg;
	public Texture2D WonMsg;	
	private bool showMsg;
	
	private Texture2D main;
	private PhotonView myView;
	private float timeLeft;
	private TimerScript ServerTimer;
	// Use this for initialization
	void Start () {
		myView = GetComponent<PhotonView>();	
		GameObject ClockObject = GameObject.FindGameObjectWithTag("timer");
		ServerTimer = ClockObject.GetComponent<TimerScript>();
	}
	
	// Update is called once per frame
	void Update () {
		if(showMsg){
			EndTimer();
		}
	}
	/*
	void OnCollisionEnter(Collision col){
		switch(col.gameObject.tag){
			case "rock" : myView.RPC("HitByRock",PhotonTargets.All);
			              print ("hit by rock");
				          break;
			case "fire" : myView.RPC("HitByFire",PhotonTargets.All);
				          print ("hit by fire");
			              break;
			case "ice"  : myView.RPC("HitByIce",PhotonTargets.All);
				          print ("hit by ice");
			              break;
		}
	}*/
	
	
	void OnCollisionEnter(Collision col){
		switch(col.gameObject.tag){
			case "rock" : this.BroadcastMessage("HitByRock");
				          break;
			case "fire" : this.BroadcastMessage("HitByFire");
			              break;
			case "ice"  : this.BroadcastMessage("HitByIce");
			              break;
		}
	}
	
	
	void OnEndGame(){
        myView.RPC ("EndGame",PhotonTargets.All);
    }
	
	[RPC]
	void EndGame(){
		ServerTimer.timerStart=false;
		ServerTimer.endGame=true;
		timeLeft=7;
		showMsg=true;
	}
	
	void OnGUI(){
        if(showMsg){
			if(myView.isMine){
				main = LostMsg;
			}else{
				main = WonMsg;
			}
			GUI.DrawTexture(new Rect(Screen.width/2-200,Screen.height/2-300,400,200), main);
		}
    }
	
	 void EndTimer(){			
			timeLeft  -= Time.deltaTime;
			
			if(timeLeft <= 0){	
			   PhotonNetwork.Disconnect();
			}	    
	 }
}
