using UnityEngine;
using System.Collections;

public class MainMenuGUI : MonoBehaviour {

	public GUISkin MenuSkin;
	
	private int menuSelector;
	private  float btnHeight ;
	private  float btnWidth  ;
	private float space;
	
	private bool thisShow  = true;
    private string playerNameInput = "";
	
	private SinglePlayerGUI SinglePlayerScript;
    private MultiplayerGUI MultiPlayerScript;
	
	public static bool connectMultiplayer = false;
	public static readonly string MainMenuScene = "mainMenu";
	public static readonly string SinglePlayerScene = "practice_scene";
	public static readonly string MultiPlayerScene = "fight_scene";
	public static readonly string HighScoreScene = "highscore_scene";
	public static readonly string AboutScene = "about_scene";
	

	
	void Awake(){
		
		SinglePlayerScript=GetComponent<SinglePlayerGUI>();
		MultiPlayerScript=GetComponent<MultiplayerGUI>();
		
		menuSelector = 0;
		btnHeight = 80;
		btnWidth  = 300;
		space=btnHeight+20;
		playerNameInput = PlayerPrefs.GetString("playerName" + Application.platform, "");
		
	}
	
	// Use this for initialization
	void Start () {
		 Screen.SetResolution(1280, 720, true);
	}
	
	// Update is called once per frame
	void Update () {
		if (Input.GetKeyDown(KeyCode.Escape))
			Application.LoadLevel(0);
	}
	
	void OnGUI(){
		if(thisShow){
			switch(menuSelector){
				case 0  : MainMenu ();
					      break;
				case 1  : SinglePlayer ();
					      break;
				case 2  : Multiplayer ();
					      break;
				case 3  : HighScores ();
					      break;
				case 4  : About ();
					      break;
			}
		}
	}
	
	void MainMenu(){
		GUI.skin = MenuSkin;
		
		GUI.BeginGroup( new Rect( Screen.width/2 -150,Screen.height/2 - 200,btnWidth,space*6));
			if(GUI.Button(new Rect(0,0,btnWidth,btnHeight),"Practice")){
				menuSelector=1;
			}
			if(GUI.Button(new Rect(0,space,btnWidth,btnHeight),"Multiplayer")){
				menuSelector=2;
			}
			if(GUI.Button(new Rect(0,space*2,btnWidth,btnHeight),"HighScores")){
				menuSelector=3;
			}
			if(GUI.Button(new Rect(0,space*3,btnWidth,btnHeight),"About")){
				menuSelector=4;
			}
			if(GUI.Button(new Rect(0,space*4,btnWidth,btnHeight),"Exit")){
				Application.Quit();
			}
		GUI.EndGroup();	
	}
	
	void MainActive (){
		thisShow=true;
	}
	
	void SinglePlayer (){
		menuSelector=0;
		thisShow=false;
		connectMultiplayer=false;
		SinglePlayerScript.enabled=true;
	}
	
	void Multiplayer (){
		menuSelector=0;
		connectMultiplayer=true;
		MultiPlayerScript.enabled=true;
		thisShow=false;	
	}
	
	void HighScores (){
		menuSelector=0;
		Application.LoadLevel(HighScoreScene);
	}
	
	void About(){
		menuSelector=0;
		Application.LoadLevel(AboutScene);
	}
}
