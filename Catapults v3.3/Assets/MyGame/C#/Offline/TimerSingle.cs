using UnityEngine;
using System.Collections;

public class TimerSingle : MonoBehaviour {
	private int roundtime=90;
	public float timeleft;
	public GUISkin timerskin;
	
	public FireSingle fire;
	public PlayerControlsSingle controls;
	// Use this for initialization
	void Start () {
		Reset();
	}
	
	// Update is called once per frame
	void Update () {
		if(targetpractice.gameover==false)
		{
			timeleft-=Time.deltaTime;
			if(timeleft < 0){        
	            targetpractice.gameover=true;  
				fire.enabled=false;
				controls.enabled=false;
			}
		}
	}
	public void Reset(){
		timeleft = roundtime;	
	}
	void OnGUI(){
		GUI.skin=timerskin;
		GUI.Label(new Rect(Screen.width/2-50, 10,100,80),(int)timeleft+"");
	}
}
