using UnityEngine;
using System.Collections;

public class SettingsSingle : MonoBehaviour {
	
	public GUISkin myskin;
	public Texture2D normal;
	public Texture2D onAction;
	
	private Rect windowRect;
	private GUITexture guiBtn;
	private bool showMenu;
	private bool enabletouch=false; 
	
		
	// Use this for initialization
	void Start () {
		windowRect = new Rect(Screen.width/2-170, Screen.height/2-100, 340, 180);
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
		
		touchcontrols(enabletouch);
	}
	
	void touchcontrols(bool state)
	{
		GameObject catapult;
		GameObject joystick;
		GameObject jBackground;
		PlayerControlsSingle pcsingle;
		TapMoveControls tmc;
		NavMeshAgent catapultagent;
		catapult=GameObject.FindGameObjectWithTag("childcatapult");
		joystick=GameObject.FindGameObjectWithTag("joystick");		
		GUITexture jtexture=joystick.GetComponent<GUITexture>();
		Joystick myjoystick=joystick.GetComponent<Joystick>();
		jBackground=GameObject.FindGameObjectWithTag("joystickBackground");
		GUITexture btexture=jBackground.GetComponent<GUITexture>();
		btexture.enabled=!state;
		jtexture.enabled=!state;				
		myjoystick.enabled=!state;	
		
		tmc =catapult.GetComponent<TapMoveControls>();
		pcsingle=catapult.GetComponent<PlayerControlsSingle>();
		catapultagent=catapult.GetComponent<NavMeshAgent>();
		tmc.enabled=state;
		catapultagent.enabled=state;
		pcsingle.enabled=!state;
	}
	public void OnGUI()
    {
		GUI.skin=myskin;
        if(showMenu){
			 windowRect = GUI.Window(0, windowRect, DoMyWindow, "Options");
		}
    }
	
	void DoMyWindow(int windowID) {
        
		GUI.Label(new Rect(30,55,240,60), "Enable touch controls");		
		enabletouch=GUI.Toggle(new Rect(250,40,64,64),enabletouch,"");	  
		if(GUI.Button (new Rect(150,120,60,40),"OK"))
		{
			showMenu=false;	
		}		
    }
}
