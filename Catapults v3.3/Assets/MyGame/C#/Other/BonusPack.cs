using UnityEngine;
using System.Collections;

public class BonusPack : MonoBehaviour {
	
	public enum packType{none,fire,ice,health,shield}
	
	public packType pack = packType.none;	
	
	private AbilityScript ability;
	private Transform[] children;
	
	private Vector3 position = new Vector3(0,0,0);
	private PhotonView myView;
	
	// Use this for initialization
	void Start () {
		myView = this.GetComponent<PhotonView>();
		
		if(PhotonNetwork.isMasterClient){
			RandomPos();
			myView.RPC ("SetPosition",PhotonTargets.AllBuffered,position);
		}
	}
	
	// Update is called once per frame
	void Update (){
		
	}
	
	void RandomPos(){
		position = new Vector3(Random.Range (-21.0f,12.0f),Random.Range (4.5f,12.0f),Random.Range(-7.0f,7.0f));	
	}
	
	[RPC]
	void SetPosition(Vector3 pos){
		this.transform.localPosition = pos;
	}
	
	void OnCollisionEnter(Collision other){		
		children = other.transform.parent.GetComponentsInChildren<Transform>();
		
		foreach ( Transform child in children){
			if(child.CompareTag("ability")){
				
				ability=child.GetComponent<AbilityScript>();
				
				switch(pack){
					case packType.fire   : ability.GetComponent<PhotonView>().RPC("PowerType",PhotonTargets.All,1);
										   break;
					case packType.ice    : ability.GetComponent<PhotonView>().RPC("PowerType",PhotonTargets.All,2);
					                       break;
					case packType.health : ability.GetComponent<PhotonView>().RPC("PowerType",PhotonTargets.All,3);
					                       break;
					case packType.shield : ability.GetComponent<PhotonView>().RPC("PowerType",PhotonTargets.All,4);
					                       break;
				}
			}
		}		
		myView.RPC("SelfDestroy",PhotonTargets.All);
	}
	
    [RPC]
	void SelfDestroy(){
		if(myView.isSceneView){
			Destroy(this.gameObject);
		}
	}
}
