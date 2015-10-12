using UnityEngine;

public class GameCenter : Photon.MonoBehaviour
{
    public Transform playerHolder;
	public GameObject gameWorld;
	public Transform SpawnPoint1;
	public Transform SpawnPoint2;
	
    public void Awake()
    {
        // in case we started this demo with the wrong scene being active, simply load the menu scene
		if (!PhotonNetwork.connected)
	    {
	            Application.LoadLevel(MainMenuGUI.MainMenuScene);
	            return;
	    }
       	SpawnPlayer();
    }
	
	void Start(){
		 
	}
	
	void SpawnPlayer(){	
	     PhotonNetwork.Instantiate(this.playerHolder.name,new Vector3(0,0,0), Quaternion.identity,0);
	}

    public void OnLeftRoom()
    {
        Debug.Log("OnLeftRoom (local)");
        // back to main menu        
        Application.LoadLevel(MainMenuGUI.MainMenuScene);
    }

    public void OnDisconnectedFromPhoton()
    {
        Debug.Log("OnDisconnectedFromPhoton");

        // back to main menu        
        Application.LoadLevel(MainMenuGUI.MainMenuScene);
    }

    public void OnPhotonInstantiate(PhotonMessageInfo info)
    {
        Debug.Log("OnPhotonInstantiate " + info.sender);    // you could use this info to store this or react
    }

    public void OnPhotonPlayerConnected(PhotonPlayer player)
    {
        Debug.Log("OnPhotonPlayerConnected: " + player);
    }

    public void OnPhotonPlayerDisconnected(PhotonPlayer player)
    {
        Debug.Log("OnPlayerDisconneced: " + player);
		PhotonNetwork.Disconnect();
    }

    public void OnFailedToConnectToPhoton()
    {
        Debug.Log("OnFailedToConnectToPhoton");

        // back to main menu        
        Application.LoadLevel(MainMenuGUI.MultiPlayerScene);
    }
	
	public void OnLevelWasLoaded(int level)
	{
		PhotonNetwork.isMessageQueueRunning = true;	
	}
}
