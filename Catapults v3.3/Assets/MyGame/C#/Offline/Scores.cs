using UnityEngine;
using System.Collections;

public class Scores : MonoBehaviour {
	public GUISkin practiceskin;
	Rect window=new Rect(Screen.width/2-200,Screen.height/2-200,400,400);
	HSController hs;
	
	string[] names=new string[10];
	int[] highscores=new int[10];
	
	// Use this for initialization
	void Start () {
		hs = gameObject.GetComponent<HSController>();
		StartCoroutine(hs.GetScores());
	}
	
	// Update is called once per frame
	void Update () {
		if (Input.GetKeyDown(KeyCode.Escape))
			Application.LoadLevel(0);
	}
	
	void OnGUI(){
		GUI.skin=practiceskin;
		getscores();
		window=GUI.Window(0,window,DoMyScores,"HIGHSCORES");
		
	}
	void DoMyScores(int windowID)
	{	
		string namelist="";
		string scorelist="";
		for(int i=0;i<5;i++){
			namelist+=names[i]+"\n";
			scorelist+=highscores[i]+"\n";
		}		
		GUI.Label(new Rect (50, 120, 250, 300), namelist);	
		GUI.Label(new Rect (320, 120, 150, 300), scorelist);
		
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
