using UnityEngine;
using System.Collections;

public class SinglePlayerGUI : MonoBehaviour {
	
	// Use this for initialization
	void Start () {
		if(!MainMenuGUI.connectMultiplayer){	
			if(!MainMenuGUI.connectMultiplayer){ 
   			PhotonNetwork.offlineMode = true;
   			Application.LoadLevel(MainMenuGUI.SinglePlayerScene);
  			}
		}
	}
	
	// Update is called once per frame
	void Update () {
	
	}
	
	public void OnCreatedRoom()
    {
		if(!MainMenuGUI.connectMultiplayer){
			PhotonNetwork.LoadLevel(MainMenuGUI.SinglePlayerScene);
		}
    }
}
