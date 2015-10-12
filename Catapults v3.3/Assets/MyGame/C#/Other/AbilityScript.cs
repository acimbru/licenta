using UnityEngine;
using System.Collections;

public class AbilityScript : MonoBehaviour {
	
	public GameObject myCatapult;
	public enum Ability {none,fire,ice,shield,health,nowind}
	public Ability power = Ability.none;
	
	private GUITexture gui;
	private new GUIText name;
	public bool isControllable;
	
	public bool Desktop;
	// Use this for initialization
	void Awake(){
		Desktop=true;
		#if UNITY_ANDROID
    		Desktop=false;
		#endif
	}
	
	void Start () {
		var holder = this.transform.parent.transform.parent.transform.parent.gameObject.GetComponentInChildren<Transform>();
				
		foreach(Transform child in holder){
			if(child.CompareTag("childcatapult")){
				myCatapult = child.gameObject;
			}
		}

		gui = this.GetComponent<GUITexture>();	
		name = this.GetComponent<GUIText>();
		name.text="";
	}
	
	// Update is called once per frame
	void Update () {
			//displays ability power if hit in air by player	
			ShowPower();
			
			//if ability loaded and displayed , on touch is activated
			if(!Desktop){
			    foreach (Touch touch in Input.touches){
						if(Input.touchCount > 0){
							if(gui.HitTest(touch.position)){
			                   EnablePower();
							}
						}
				}
			}else{
				if(Input.GetMouseButtonDown(0) && gui.HitTest(Input.mousePosition) || Input.GetMouseButton(0) && gui.HitTest(Input.mousePosition)){
							EnablePower();
				}
			}
	}
	
	void ShowPower(){
		switch(power){
			case Ability.none  : gui.enabled=false;
								 name.text="";
								 break;
			case Ability.fire  : gui.enabled=true;
			                     name.text="Fire"; 
								 break;
			case Ability.ice   : gui.enabled=true;
			                     name.text="Ice";
								 break;
			case Ability.health: gui.enabled=true;
								 name.text="Health";
			                     break;	
			case Ability.shield: gui.enabled=true;
								 name.text="Shield";
			                     break;
		}
	}
	
	void EnablePower(){
		object[] data = new object[2];

		switch(power){
			case Ability.none  : break;
			case Ability.fire  : data[0] = "projectile_fire";
                                 data[1] = 0;
			                     myCatapult.SendMessage("ChangeProjectile",data);
								 power = Ability.none;
								 break;
			case Ability.ice   : data[0] = "projectile_ice";
                                 data[1] = 0;
			                     myCatapult.SendMessage("ChangeProjectile",data);
								 power = Ability.none;
								 break;
			case Ability.health: myCatapult.SendMessage("HealthBonus",30);
								 power = Ability.none;
			                     break;
			case Ability.shield: myCatapult.SendMessage("ActivateShield");
								 power = Ability.none;
			                     break;
		}
	}
	
	[RPC]
	void PowerType(int type){
		 switch(type){
			case 1:power=Ability.fire;
				   print ("fire_received");
			       break;
			case 2:power=Ability.ice;
				   print ("ice_received");
			       break;
			case 3:power=Ability.health;
				   print ("health_received");
				   break;
			case 4:power=Ability.shield;
			       print ("shield_received");
				   break;	
		}
	}
}
