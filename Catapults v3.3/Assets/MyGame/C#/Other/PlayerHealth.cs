using UnityEngine;
using System.Collections;

public class PlayerHealth : MonoBehaviour {

	public int health = 100;
	
	public GameObject heartsGUI;
	public EnergyBar lifeBar;
	//public HoveringHealth healthBar;
	public GameObject shield;
	public PhotonView myView;
	private int contorHit = 1;
	public bool shieldActive = false;
	public bool dead = false;
	
	private Rigidbody rigid;
	private BoxCollider coll;
	private int dmgReceive;
	private int traits;
	
	// Use this for initialization
	void Start () {
		rigid = this.GetComponent<Rigidbody>();
		coll = this.GetComponent<BoxCollider>();
		myView = this.GetComponent<PhotonView>();
		lifeBar = heartsGUI.GetComponent<EnergyBar>();
		lifeBar.valueCurrent = health;
	}
	
	// Update is called once per frame
	void Update () {
		lifeBar.valueCurrent = health;
	}
	
	void HitByRock(){
	        traits = 20;
			contorHit=0;
			myView.RPC ("TakeDamage",PhotonTargets.All,traits);
	}
	
	void HitByFire(){
	        traits = 35;
			contorHit=0;
			myView.RPC ("TakeDamage",PhotonTargets.All,traits);
	}
	
	void HitByIce(){
	        traits = 25;
		    this.SendMessage("Frozen");
			contorHit=0;
			myView.RPC ("TakeDamage",PhotonTargets.All,traits);
	}
	
	void ActivateShield(){	
		myView.RPC ("Shield",PhotonTargets.All);
	}
	
	[RPC]
	void Shield(){
		shieldActive = true; 
		shield.SetActive(true);
	}
	
	void HealthBonus(int hp){
		myView.RPC ("IncreaseHealth",PhotonTargets.All,hp);
	}
	
	[RPC]
	void IncreaseHealth(int hp){
		health+=hp;
		
		if(health>100){
			health=100;
		}
	}
	
	[RPC]
	void TakeDamage(int receiveData){
		
				dmgReceive = receiveData;		
		
				if(!shieldActive){
			    	health -= dmgReceive;
			    }
			
				if(health <= 0){	
					DisableColliders();
					this.SendMessage("OnEndGame");
					dead = true;
				}
	}
	
	void DisableColliders(){
		rigid.isKinematic=true;
		coll.enabled=false;
	}
}