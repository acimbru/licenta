using UnityEngine;
using System.Collections;

public class QuitGame : MonoBehaviour {

	public Texture2D normal;
	public Texture2D onAction;
	private Rect windowRect;
	
	private GUITexture guiBtn;
	private bool showMenu;
	
	// Use this for initialization
	void Start () {
		windowRect = new Rect(Screen.width/2-200, Screen.height/2-100, 400, 200);
		guiBtn=this.GetComponent<GUITexture>();
	}
	
	// Update is called once per frame
	void Update () {
		if(Input.GetMouseButtonDown(0) && guiBtn.HitTest(Input.mousePosition) || Input.GetMouseButton(0) && guiBtn.HitTest(Input.mousePosition)){
			guiBtn.texture = onAction;
			showMenu=true;
								
		}else{
			guiBtn.texture = normal;
		}
	}
	
	public void OnGUI()
    {
        if(showMenu){
			 windowRect = GUI.Window(0, windowRect, DoMyWindow, "Options");
		}
    }
	
	void DoMyWindow(int windowID) {
        
		GUI.Label(new Rect(120,30,300,30), "Do you really want to quit?");
		
		// we will load the menu level when we successfully left the room
		if(GUI.Button(new Rect(90,120,100,30),"YES")){
	            PhotonNetwork.LeaveRoom();        
	    }else if(GUI.Button(new Rect(230,120,100,30),"NO")){
				showMenu=false;
		}
		GUI.Label(new Rect(150,170,300,20),"Ping to server: " + PhotonNetwork.GetPing());    
    }
}
