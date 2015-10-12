using UnityEngine;
using System.Collections;

public class DestructibleEnvironmentM : MonoBehaviour {

	public enum ObstacleType{none,Small,Medium,Big}
	public ObstacleType type = ObstacleType.none;	
	
	private int hitsTaken = 0;
	private PhotonView myView;
	
	// Use this for initialization
	void Start () {
		myView = this.GetComponent<PhotonView>();
	}
	
	void OnCollisionEnter(Collision other){		
		if(other.transform.CompareTag("fire") || other.transform.CompareTag("ice") || other.transform.CompareTag("rock")){
			myView.RPC("AddHits",PhotonTargets.All);
			
			switch(type){
				case ObstacleType.Small  : if(hitsTaken==1){
											 myView.RPC("DestroyObstacle",PhotonTargets.All);
										   }
				                           break;
				
				case ObstacleType.Medium : if(hitsTaken==2){
											 myView.RPC("DestroyObstacle",PhotonTargets.All);
										   }
				                           break;
				
				case ObstacleType.Big    : if(hitsTaken==3){
											 myView.RPC("DestroyObstacle",PhotonTargets.All);
										   }
				                           break;
				
				default                  : print("No type selected!");
										   break;
			}
		}
	}
	
	[RPC]
	void AddHits(){
		hitsTaken+=1;
	}
	
	[RPC]
	void DestroyObstacle(){
		if(myView.isSceneView){
			Destroy(this.gameObject);
		}
	}
}
