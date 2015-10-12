using UnityEngine;
using System.Collections;

public class TimerScript : MonoBehaviour {
 
	//public vars
	public Texture2D message;
	public bool timerStart;
	public int values;
	public PhotonView myView;
	//private vars
	private EnergyBar timerValue;
	private double startTime;
	private GameObject[] players;
	private RotatableGuiItem rotateScript;
	private int contor = 0;
	private bool isControlable;
	public bool showMsg;
	public bool endGame = false;
	// Use this for initialization
	void Start () {
		myView = GetComponent<PhotonView>();
		rotateScript = GetComponent<RotatableGuiItem>();
		startTime= PhotonNetwork.time;
	}
	
	// Update is called once per frame
	void Update () {
		//print(PhotonNetwork.countOfPlayers);
		
		players=GameObject.FindGameObjectsWithTag("childcatapult");
		
		if(players.Length==2 && contor==0){
			myView.RPC("SetTimer",PhotonTargets.All,PhotonNetwork.time);
			timerStart= true;
			contor+=1;
		}
		
		if(timerStart){
			StartTimer();
		}
		
		if(endGame){
			rotateScript.enabled=false;
		}
	}
	
	void OnGUI() {
		if(showMsg && !endGame){
			GUI.DrawTexture(new Rect(Screen.width/2-200,Screen.height/2-100,400,200), message);
		}
    }
	
	bool ok = true;
	
	void StartTimer(){
		if(Mathf.Abs(values) == 0){
			if(ok){	
				SwitchPlayer();
				ok=false;
			}
		}
		
		double timeStep = PhotonNetwork.time - startTime;
		double seconds = timeStep % 60;
		values = (int)seconds;
		rotateScript.angle = values*12;
		
		//Debug.Log ("timp este: " + values);
		if(Mathf.Abs(values) >= 30){
			showMsg=true;		
		}else{
			showMsg=false;
		}
		
		if(Mathf.Abs(values) == 31){
			ResetTimer();		
		}
	}
	
	void ResetTimer(){
		ok=true;
		startTime= PhotonNetwork.time;
		values=0;
		rotateScript.angle = values*12;
		timerStart=true;
	}	
	
	[RPC]
	void SetTimer(double type){
		startTime = type;
	}
	
	void SwitchPlayer(){				
		players[0].GetPhotonView().RPC("Switch",PhotonTargets.Others);
		players[1].GetPhotonView().RPC("Switch",PhotonTargets.Others);
	}
}
