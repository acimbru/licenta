using UnityEngine;
using System.Collections;

public class ComponentsManager : MonoBehaviour {

	public GameObject GUIControls;
	private bool isControlable=true;
	public GameObject nameTag;
	public GameObject windGen;
	public GameObject windBar;
	private Transform[] controls;
	private Transform tags;
	private Transform[] winds;
	private Transform[] windDisplays;
	private PhotonView myView;
	private int contorDeStart = 0; 
	private PlayerControls movement;
	private Fire action;
	private GameObject[] players = new GameObject[2];
	// Use this for initialization
	void Start () {
		action = this.GetComponent<Fire>();
		movement = this.GetComponent<PlayerControls>();
		myView = this.GetComponent<PhotonView>();
		
		controls = this.transform.parent.GetComponentsInChildren<Transform>();
		
		foreach( Transform child in controls){
			if(child.CompareTag("touchGUI")){		
				  GUIControls = child.gameObject;
			}		
		}
		
		winds  = this.transform.parent.GetComponentsInChildren<Transform>();
		
		foreach( Transform child in winds){
			if(child.CompareTag("windGenerator")){		
				  windGen = child.gameObject;
			}		
		}
		
		windDisplays  = this.transform.parent.GetComponentsInChildren<Transform>();
		
		foreach( Transform child in windDisplays){
			if(child.CompareTag("windBar")){		
				  windBar = child.gameObject;
			}		
		}
		
		
		tags = GetComponentInChildren<Transform>();
		
		foreach( Transform child in tags){
			if(child.CompareTag("nametag")){		
				  nameTag = child.gameObject;
			}		
		}
		
		nameTag.GetComponent<TextMesh>().text = myView.owner.name;
	}
	
	// Update is called once per frame
	void Update () {
		players=GameObject.FindGameObjectsWithTag("childcatapult");
		
		if(contorDeStart==0){
			if(players.Length==2){
				if(PhotonNetwork.isMasterClient){
					isControlable=true;
				}else{
					isControlable=false;
				}
				contorDeStart+=1;
			}
		}
		
		if(isControlable){
			GUIControls.transform.localPosition = new Vector3(0,0,0);
		}else{
			GUIControls.transform.localPosition = new Vector3(10,0,0);
		}
	}
	
	[RPC]
	public void Switch(){
		isControlable=!isControlable;
		print ("switched");
				
		if(isControlable){
			windGen.GetPhotonView().RPC("RandomWind",PhotonTargets.All);
			action.enabled = true;
			action.shootCounter = 1;
			action.predictionLine.enabled=true;
			movement.enable=true;
		}else{
			action.predictionLine.enabled=false;
			action.enabled=false;
			movement.enable=false;
		}
	}
}
