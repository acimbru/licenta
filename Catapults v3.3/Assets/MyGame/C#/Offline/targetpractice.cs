using UnityEngine;
using System.Collections;

public class targetpractice : MonoBehaviour {
	public static bool gameover;
	public static int scor;
	public GUISkin practiceskin;
	Rect window=new Rect(Screen.width/2-150,Screen.height/2-150,300,300);
	Rect windownewscor=new Rect(Screen.width/2-150,Screen.height/2-125,300,250);
	string namePlayer="";
	string[] names=new string[10];
	int[] highscores=new int[10];	
	public static bool userHasHitReturn=false;
	public bool posted;
	public bool read;
	bool writing=false;
	HSController hs;
	
	// Use this for initialization
	void Start () {
	gameover=false;
	posted=false;
	read=false;
	scor=0;
	namePlayer=PlayerPrefs.GetString("name");
	hs = gameObject.GetComponent<HSController>();
	}
	
	// Update is called once per frame
	void Update () {

	}
	
	void OnGUI(){
		GUI.skin=practiceskin;
		GUI.Box(new Rect(Screen.width-100,0,60,60),scor.ToString());
		string msg="FINAL SCORE: "+scor;	
		
		if(gameover){
			if(checkifhighscore(scor))	
			{				
				if(userHasHitReturn==false){
				string newscore="NEW HIGHSCORE: "+scor+"\nENTER YOUR NAME:";
				windownewscor=GUI.Window(0,windownewscor,DoMyNewScore,newscore);				
				}
				else{
				if(posted==false)
					{
					StartCoroutine(hs.PostScores(namePlayer,scor));			
					posted=true;	
					}
				window=GUI.Window(0,window,DoMyScores,"HIGHSCORES");
				}
			}else{
			if(writing==false){			
				if(read == false){
					StartCoroutine(hs.GetScores());
					read=true;
				}					
				getscores();
				window=GUI.Window(0,window,DoMyScores,"HIGHSCORES");				
			}
			}
		}				   
	}
	void DoMyScores(int windowID)
	{	
		string namelist="";
		string scorelist="";
		for(int i=0;i<5;i++){
			namelist+=names[i]+"\n";
			scorelist+=highscores[i]+"\n";
		}		
		GUI.Label(new Rect (70, 60, 130, 200), namelist);	
		GUI.Label(new Rect (210, 60, 100, 200), scorelist);
		//buttons
		if(GUI.Button(new Rect(10,240,130,50),"Try again")){
			Application.LoadLevel(Application.loadedLevel);
			/*scor=0;
			gameover=false;
			userHasHitReturn=false;
			posted=false;
			read=false;
			TimerSingle ts = gameObject.GetComponent<TimerSingle>();
			ts.Reset();	*/		
		}
		if(GUI.Button(new Rect(160,240,130,50),"Main Menu")){
			Application.LoadLevel(0);
		}
	}
	void DoMyNewScore(int windowID)
	{
		Event e = Event.current; 
		if (e.keyCode == KeyCode.Return  || GUI.Button(new Rect(125,170,50,40), "OK")) {
			userHasHitReturn = true;
		}else if (false == userHasHitReturn) {
			namePlayer = GUI.TextArea(new Rect(70,120,160,40), namePlayer, 30);		
			PlayerPrefs.SetString("name",namePlayer);
		}
	}	
	bool checkifhighscore(int scornou){
		bool state=false;
		getscores();
		string tempname;
		int tempscore;
		for(int i=0; i<5;i++)
			if(scornou>highscores[i]){			
			tempname=names[i];
			tempscore=highscores[i];
			names[i]=namePlayer;
			highscores[i]=scor;
			state=true;
			for(int j=6; j>i+2;j--){
			names[j]=names[j-1];
			highscores[j]=highscores[j-1];	
			}
			names[i+1]=tempname;
			highscores[i+1]=tempscore;
			return state;
		}
		return state;
	}
	
	void getscores(){		    			
		    string[] words=hs.scores.Split();
			
			for(int i=0; i<words.Length-1; i+=2){
					names[i/2]=words[i];
					//Debug.Log (words[i]);
					int.TryParse(words[i+1],out highscores[i/2]);
			}
	}	
}