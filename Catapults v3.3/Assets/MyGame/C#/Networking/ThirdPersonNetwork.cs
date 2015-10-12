using UnityEngine;
using System.Collections;

public class ThirdPersonNetwork : Photon.MonoBehaviour
{
	//ThirdPersonCameraNET cameraScript;
	PlayerControls player;
	Fire thisFireBtn;
	
	private Camera cam;
    
	void Awake()
    {
			player = GetComponent<PlayerControls>();
	        thisFireBtn = GetComponent<Fire>();
			cam = this.transform.parent.GetComponentInChildren<Camera>();
	        
		    if (photonView.isMine)
	        {
				cam.enabled = true;
			    player.enabled=true;
				player.isControllable=true;
				thisFireBtn.isControllable=true;
	        }
	        else
	        {           
	            cam.enabled = false;
			    player.enabled=true;
				player.isControllable=false;
				thisFireBtn.isControllable=false;
	        }
	
	        gameObject.name = gameObject.name + photonView.viewID;
    }
	
	 void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.isWriting)
        {
            //We own this player: send the others our data
            stream.SendNext((int)player._characterState);
            stream.SendNext(transform.position);
            stream.SendNext(transform.rotation); 
        }
        else
        {
            //Network player, receive data
            player._characterState = (AnimationsState)(int)stream.ReceiveNext();
			correctPlayerPos = (Vector3)stream.ReceiveNext();
            correctPlayerRot = (Quaternion)stream.ReceiveNext();
        }
    }

    private Vector3 correctPlayerPos = Vector3.zero; //We lerp towards this
    private Quaternion correctPlayerRot = Quaternion.identity; //We lerp towards this

    void Update()
    {
        if (!photonView.isMine)
        {
            //Update remote player (smooth this, this looks good, at the cost of some accuracy)
            transform.position = Vector3.Lerp(transform.position, correctPlayerPos, Time.deltaTime * 5);
            transform.rotation = Quaternion.Lerp(transform.rotation, correctPlayerRot, Time.deltaTime * 5);
        }
    }
}