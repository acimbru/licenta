using UnityEngine;
using System.Collections;

public class ProjectileNetwork : Photon.MonoBehaviour {
	
    private bool appliedInitialUpdate;
	private bool myRigid;
	private bool hisRigid;
	
    void Start()
    {
		if(photonView.isMine){
			myRigid = false;
			hisRigid = false; 
		}else{
			myRigid = false;
			hisRigid = false; 
		}
    }

    void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.isWriting)
        {
            //We own this player: send the others our data
			stream.SendNext(myRigid);
			stream.SendNext(transform.position);
            stream.SendNext(transform.rotation);
			if(myRigid){
				stream.SendNext(rigidbody.velocity);
			}
        }
        else
        {
            if (!appliedInitialUpdate && hisRigid)
            {
                appliedInitialUpdate = true;
                transform.position = correctPlayerPos;
                transform.rotation = correctPlayerRot;
				rigidbody.velocity = Vector3.zero;
            }
			
			//Network player, receive data
			hisRigid         = (bool)stream.ReceiveNext();
            correctPlayerPos = (Vector3)stream.ReceiveNext();
            correctPlayerRot = (Quaternion)stream.ReceiveNext();
			
			if(hisRigid){
				rigidbody.velocity = (Vector3)stream.ReceiveNext();
			}
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
		
		if(!rigidbody.isKinematic){
			myRigid=true;
		}
    }
}
