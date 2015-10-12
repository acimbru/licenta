using UnityEngine;
using System.Collections;

public class ImpactRenderer : MonoBehaviour {

	public enum HitSurface{player,bonus,fire,ground,ice}
	public HitSurface type = HitSurface.ground;
	
	public string names;
	public Transform GameWorld;
	private PhotonView myView; 
	// Use this for initialization
	void Start () {
		myView = this.GetComponent<PhotonView>();
		GameWorld = GameObject.FindGameObjectWithTag("GameWorld").transform;
		SelectParticle();
	}
	
	[RPC]
	void Impact(string resourceName){		 
		    var clone = Instantiate(Resources.Load(resourceName),this.transform.position,this.transform.rotation) as GameObject;
			clone.transform.parent = GameWorld;
			
			//destroy projectile
			Destroy(this.gameObject);
	}
		
	void OnCollisionEnter(Collision col){
		
		if(!col.transform.CompareTag("Untagged")){
			switch(col.transform.tag){
				case "bonus"        : type= HitSurface.bonus;
				                      break;
				default             : break;
			}
		}
		
		//select prefab from Resources
		SelectParticle();
		
		//send RPC
		myView.RPC("Impact",PhotonTargets.All,names);
	}
	
	void SelectParticle(){
		switch(type){
			// rock particles that depend on the surface touched	
			case HitSurface.player: names= "hitplayer";
			                        break;
		    case HitSurface.bonus:  names= "hitBonus";
			                        break;
			case HitSurface.ground: names= "hitGroundWalls";
			                        break;
		    // particles that depend on type of projectile, no matter the surface	
		    case HitSurface.fire:   names= "hitFire";
			                        break;
			case HitSurface.ice:    names= "hitIce";
			                        break;
		}
	}
}
