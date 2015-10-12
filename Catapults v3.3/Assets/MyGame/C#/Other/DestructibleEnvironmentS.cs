using UnityEngine;
using System.Collections;

public class DestructibleEnvironmentS : MonoBehaviour {

	public enum ObstacleType{none,Small,Medium,Big}
	public ObstacleType type = ObstacleType.none;	
	
	private int hitsTaken = 0;
	
	// Use this for initialization
	void Start () {
		
	}
	
	void OnCollisionEnter(Collision other){		
		if(other.transform.CompareTag("rock")){
			hitsTaken+=1;
			
			switch(type){
				case ObstacleType.Small  : if(hitsTaken==1){
											 Destroy(this.gameObject);
										   }
				                           break;
				
				case ObstacleType.Medium : if(hitsTaken==2){
											 Destroy(this.gameObject);
										   }
				                           break;
				
				case ObstacleType.Big    : if(hitsTaken==3){
											 Destroy(this.gameObject);
										   }
				                           break;
				
				default                  : print("No type selected!");
										   break;
			}
		}
	}
}
