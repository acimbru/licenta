using UnityEngine;
using System.Collections;

public class WindGenerator : MonoBehaviour {

	// Use this for initialization
	public int windForce;
	public Vector3 windDirection;
	
	public Vector3 direction;
	public int force;
	
	private int dir=0;
	private int frc=0;
	
	public GameObject windGUI;
	public EnergyBar windFor;
	public windType windDir;
	
	public GameObject projectileHolder;
	public GameObject projectile;
	public object[] data = new object[2];
	
	public bool isControlable=false;
	
	void Start () {
		if(this.GetComponent<PhotonView>().isMine){
			isControlable=true;
		}else{
			isControlable=false;
		}
		
		if(isControlable){
			var windGUIS = this.transform.parent.GetComponentsInChildren<Transform>();
			
			foreach( Transform gui in windGUIS){
				if(gui.CompareTag("windBar")){
					windGUI = gui.gameObject;
				}
			}
			
			GameObject[] holders = GameObject.FindGameObjectsWithTag("Holder");
			
			foreach(GameObject holds in holders){
				if(holds.GetPhotonView().isMine){
					projectileHolder = holds.gameObject;
				}
			}
	
			windFor=windGUI.GetComponent<EnergyBar>();
			windDir=windGUI.GetComponent<windType>();
			
			this.GetComponent<PhotonView>().RPC("RandomWind",PhotonTargets.All);
		}
	}
	
	// Update is called once per frame
	void Update () {
		if(isControlable){
			windFor.valueCurrent=force;
			windForce=force;
		}
	}
	
	[RPC]
	void RandomWind(){
		if(isControlable){
			print ("wind generated");
			//wind direction
			dir=Random.Range(1,3);
			//wind force
			frc=Random.Range(60,100);
			
	
			switch(dir){
				/*case 0: windGUI.BroadcastMessage("WindSetup",1);
				        print ("BarMSGSENT");
				        windDirection=Vector3.zero;
				        force=0;
					    break;*/
				case 1: windGUI.BroadcastMessage("WindSetup",2);
						print ("BarMSGSENT");
						if(PhotonNetwork.isMasterClient){
				        	windDirection=Vector3.right;
						}else{
							windDirection=Vector3.left;
						}
						force=frc;
						break;
				case 2: windGUI.BroadcastMessage("WindSetup",3);
						print ("BarMSGSENT");
						if(PhotonNetwork.isMasterClient){
				        	windDirection=Vector3.left;
						}else{
							windDirection=Vector3.right;
						}
				        force=frc;
						break;
			}
		}
	}
	
	[RPC]
	void ApplyWind(){
		if(isControlable){
			print ("wind sent");
			data[0] = windForce;
			data[1] = windDirection;
			
			Transform[] projectiles = projectileHolder.GetComponentsInChildren<Transform>();
			
			foreach(Transform proj in projectiles){
				if(proj.gameObject.GetPhotonView()!=null){	
					projectile=proj.gameObject;
					projectile.GetPhotonView().RPC("WindData",PhotonTargets.All,data);
				}
			}
		}
	}
	
	[RPC]
	void WindData(object[] param){

	}
}
