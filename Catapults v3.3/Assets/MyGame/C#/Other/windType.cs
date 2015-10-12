using UnityEngine;
using System.Collections;

public class windType : MonoBehaviour {

	public Texture2D[] windtextures;
	public enum WindType {none,left,right};
	public WindType wind = WindType.none;
	
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
    
	void WindSetup(int type){
		print ("WindBarReceived" + type);
		switch(type){
			case 1: wind = WindType.none;
						         break;
			case 2: wind = WindType.left;
						         break;
			case 3: wind = WindType.right;
						         break;
		}
		
		switch(wind){
			case WindType.none:  EnergyBarRenderer.windBar=windtextures[0];
								 EnergyBarRenderer.direction=0;
						         break;
			case WindType.left:  EnergyBarRenderer.windBar=windtextures[1];
							 	 EnergyBarRenderer.direction=1;
						         break;
			case WindType.right: EnergyBarRenderer.windBar=windtextures[2];
								 EnergyBarRenderer.direction=2;
						         break;
		}
    }
}
