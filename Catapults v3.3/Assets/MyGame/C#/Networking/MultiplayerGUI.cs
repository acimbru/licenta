using System;
using UnityEngine;
using Random = UnityEngine.Random;

public class MultiplayerGUI : MonoBehaviour
{
    public GUISkin mySkin;
	
	private string roomName = "myRoom";
	
	private int selGridInt = 0;
    
	private string[] selStrings = new string[] {"Map1", "Map2", "Map3"};
	
    private Vector2 scrollPos = Vector2.zero;

    private bool connectFailed = false;
	
	private MainMenuGUI menu;
	
    public void Start()
    {
			menu = this.GetComponent<MainMenuGUI>();
		
			if(MainMenuGUI.connectMultiplayer){
			    // this makes sure we can use PhotonNetwork.LoadLevel() on the master client and all clients in the same room sync their level automatically
		        PhotonNetwork.automaticallySyncScene = true;
		
		        // the following line checks if this client was just created (and not yet online). if so, we connect
		        if (PhotonNetwork.connectionStateDetailed == PeerState.PeerCreated)
		        {
		            // Connect to the photon master-server. We use the settings saved in PhotonServerSettings (a .asset file in this project)
		            PhotonNetwork.ConnectUsingSettings("1.0");
		        }
		
		        // generate a name for this player, if none is assigned yet
		        if (String.IsNullOrEmpty(PhotonNetwork.playerName))
		        {
		            PhotonNetwork.playerName = "Guest" + Random.Range(1, 9999);
		        }
		
		        // if you wanted more debug out, turn this on:
		        // PhotonNetwork.logLevel = NetworkLogLevel.Full;
			}
	}

    public void OnGUI()
    {
        GUI.skin = mySkin;
		
		if (!PhotonNetwork.connected)
        {
            if (PhotonNetwork.connectionState == ConnectionState.Connecting)
            {
                GUILayout.Label("Connecting " + PhotonNetwork.ServerAddress);
                GUILayout.Label(Time.time.ToString());
            }
            else
            {
                GUILayout.Label("Not connected. Check console output.");
            }

            if (this.connectFailed)
            {
                GUILayout.Label("Connection failed. Check setup and use Setup Wizard to fix configuration.");
                GUILayout.Label(String.Format("Server: {0}:{1}", new object[] {PhotonNetwork.ServerAddress, PhotonNetwork.PhotonServerSettings.ServerPort}));
                GUILayout.Label("AppId: " + PhotonNetwork.PhotonServerSettings.AppID);
                
                if (GUILayout.Button("Try Again", GUILayout.Width(100)))
                {
                    this.connectFailed = false;
                    PhotonNetwork.ConnectUsingSettings("1.0");
                }
            }

            return;
        }

		/*if(GUI.Button(new Rect(50,50,100,100),"Back")){
			PhotonNetwork.Disconnect();
			menu.SendMessage("MainActive");
		}*/
		
        
		
		GUI.Box(new Rect((Screen.width - 800) / 2, (Screen.height - 600) / 2, 800, 600), "Join or Create a Room");
        	GUILayout.BeginArea(new Rect((Screen.width - 600) / 2, (Screen.height - 450) / 2, 660, 500));

       		GUILayout.Space(25);

        // Player name
        GUILayout.BeginHorizontal();
        GUILayout.Label("Player name:", GUILayout.Width(80),GUILayout.Height(80));
        PhotonNetwork.playerName = GUILayout.TextField(PhotonNetwork.playerName,GUILayout.Width(400),GUILayout.Height(80));
        GUILayout.Space(105);
        
		if (GUI.changed)
        {
            // Save name
            PlayerPrefs.SetString("playerName", PhotonNetwork.playerName);
        }
        GUILayout.EndHorizontal();

        GUILayout.Space(15);

        // Join room by title
        GUILayout.BeginHorizontal();
       		GUILayout.Label("Room name:", GUILayout.Width(80));
        	this.roomName = GUILayout.TextField(this.roomName,GUILayout.Width(400),GUILayout.Height(80));
		GUILayout.EndHorizontal();
		
		GUILayout.Space(10);
		
		GUILayout.BeginVertical();
			GUILayout.BeginHorizontal();
				GUILayout.Space(88);
				selGridInt = GUILayout.SelectionGrid(selGridInt, selStrings, 3,GUILayout.Width(400));
				GUILayout.Space(30);
		
				if (GUILayout.Button("Create", GUILayout.Width(130),GUILayout.Height(50)))
	      		{
	            	switch(selGridInt){
						case 0: PlayerPrefs.SetInt("map",0);
						        break;
						case 1:	PlayerPrefs.SetInt("map",1);
						        break;
						case 2: PlayerPrefs.SetInt("map",2);
						        break;
					}
					PlayerPrefs.Save();
					PhotonNetwork.CreateRoom(this.roomName, true, true, 2);
	        	}
			GUILayout.EndHorizontal();
		GUILayout.EndVertical();			

        GUILayout.Space(15);
        if (PhotonNetwork.GetRoomList().Length == 0)
        {
            GUILayout.Label("Currently no games are available.");
            GUILayout.Label("Rooms will be listed here, when they become available.");
        }
        else
        {
            PhotonNetwork.GetRoomList();
			GUILayout.Label("There are rooms currently available. Join either:");

            // Room listing: simply call GetRoomList: no need to fetch/poll whatever!
            this.scrollPos = GUILayout.BeginScrollView(this.scrollPos);
            foreach (RoomInfo roomInfo in PhotonNetwork.GetRoomList())
            {
                GUILayout.BeginVertical();
				GUILayout.Space(10);
				GUILayout.EndVertical();
				GUILayout.BeginHorizontal();
                GUILayout.Label(roomInfo.name + " " + roomInfo.playerCount + "/" + roomInfo.maxPlayers);
				GUILayout.Space(10);
                if (GUILayout.Button("Join",GUILayout.Width(100),GUILayout.Height(50)))
                {
                    switch(selGridInt){
						case 0: PlayerPrefs.SetInt("map",0);
						        break;
						case 1:	PlayerPrefs.SetInt("map",1);
						        break;
						case 2: PlayerPrefs.SetInt("map",2);
						        break;
					}
					PlayerPrefs.Save();
					PhotonNetwork.JoinRoom(roomInfo.name);
                }

                GUILayout.EndHorizontal();
            }

            GUILayout.EndScrollView();
        }

        GUILayout.EndArea();
    }

    // We have two options here: we either joined(by title, list or random) or created a room.
    public void OnJoinedRoom()
    {
        Debug.Log("OnJoinedRoom");
    }

    public void OnCreatedRoom()
    {
        if(MainMenuGUI.connectMultiplayer){
			Debug.Log("OnCreatedRoom");
	        PhotonNetwork.LoadLevel(MainMenuGUI.MultiPlayerScene);
		}
    }


    public void OnDisconnectedFromPhoton()
    {
        Debug.Log("Disconnected from Photon.");
    }

    public void OnFailedToConnectToPhoton(object parameters)
    {
        this.connectFailed = true;
        Debug.Log("OnFailedToConnectToPhoton. StatusCode: " + parameters);
    }
}
