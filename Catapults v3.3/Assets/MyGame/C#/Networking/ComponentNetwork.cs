using UnityEngine;
using System.Collections;

public class ComponentNetwork : Photon.MonoBehaviour {
	
	public Transform GameWorld;
	public Transform SpawnPoint1;
	public Transform SpawnPoint2;
	
	public bool componentGUI;
	// Use this for initialization
	void Start () {
		GameWorld = GameObject.FindGameObjectWithTag("GameWorld").transform;
		
		var spawns = GameObject.FindGameObjectsWithTag("Spawnpoint");
		
		SpawnPoint1=spawns[0].transform;
		SpawnPoint2=spawns[1].transform;
		
		if(componentGUI){
			if (!photonView.isMine)
	        {
	            this.gameObject.transform.localPosition = new Vector3(100,100,0);
	        }
		}else{
			if(PhotonNetwork.isMasterClient){
				if(this.CompareTag("Player")){
					this.transform.parent = GameWorld;
				}
				
				if(this.CompareTag("childcatapult")){
					this.transform.position = SpawnPoint1.position;
					this.transform.rotation = SpawnPoint1.rotation;
				}
			}else{
				if(this.CompareTag("Player")){
					this.transform.parent = GameWorld;
				}
				
				if(this.CompareTag("childcatapult")){
					this.transform.position = SpawnPoint2.position;
					this.transform.rotation = SpawnPoint2.rotation;
				}
			}
		}
	}
}
