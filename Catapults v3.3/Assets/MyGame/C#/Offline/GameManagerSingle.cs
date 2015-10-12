using UnityEngine;
using System.Collections;

public class GameManagerSingle : MonoBehaviour {
	
	public GameObject Player;
	public GameObject Bullseye;
	
	public Transform SpawnPlayerPoint;
	public Transform SpawnBullseyePoint;
	
	// Use this for initialization
	void Start () {
		var player   = Instantiate(Player, new Vector3(0,0,0),Quaternion.identity) as GameObject;
		player.transform.parent = this.transform;
		player.transform.localPosition = SpawnPlayerPoint.localPosition;
		
		var bullseye = Instantiate(Bullseye, new Vector3(0,0,0),Quaternion.identity) as GameObject;
		bullseye.transform.parent = this.transform;
		bullseye.transform.localPosition = SpawnBullseyePoint.localPosition;
		bullseye.transform.localEulerAngles = new Vector3(-90,-90,0);
	}
	
	// Update is called once per frame
	void Update () {
	
	}
		 
}
