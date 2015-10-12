using UnityEngine;
using System.Collections;

public class TapMoveControls : MonoBehaviour
{
	//navigation
	public GUISkin hitskin;
	public Vector3 target;
	NavMeshAgent agent;
	bool obstaclehit=false;
	private float guiAlpha=0.0f;
	public float fadetime=2;
	Color mytexture;
	public GameObject clickParticle;
	
	//animation		
    private Animate anim;
	//touch
	private Vector2 v2_current;
	private Vector2 v2_previous;
	private float angle;
	public int rotationspeed=5;
	//doubletap
	public float timeBetweenTouches=0.5f;
	private int tapCount=0; 
	
	//aiming object
	private ScrollScript moveSlider;
	private GameObject SlidersObject;
	public Transform Tracking;
    public float TrackingSpeed = 5;
	

	void Start ()
	{
		agent = GetComponent<NavMeshAgent> ();	
		agent.velocity=Vector3.zero;
		anim = GetComponent<Animate>();
		SlidersObject = GameObject.FindGameObjectWithTag("Sliders");
		moveSlider=SlidersObject.GetComponent<ScrollScript>();
		anim.idle ();
	}
   
	void Update ()
	{
		mytexture=new Color(1,1,1,guiAlpha);
		if(guiAlpha>0.01f){
			guiAlpha-=Time.deltaTime*(1/fadetime);
		}
		
		if(timeBetweenTouches>0){
			timeBetweenTouches-=1*Time.deltaTime;	
		}
		else{
			tapCount=0;	
		} 
		if (Input.touchCount==1 && Input.GetTouch (0).phase==TouchPhase.Ended) {
			if(timeBetweenTouches>0 && tapCount==1)
			{		
				Ray ray = Camera.main.ScreenPointToRay (Input.GetTouch(0).position);
				RaycastHit Hit;           
	           
				if (Physics.Raycast (ray, out Hit))
				if (Hit.collider != null) {
					target = Hit.point;
					agent.SetDestination (target);
					GameObject particle=Instantiate(clickParticle,target,Quaternion.identity) as GameObject;
					Debug.Log ("Raycast Hit");
				}
			}
			else{
				timeBetweenTouches=0.5f;
				tapCount+=1;
			}
		}
		
		if(Input.touchCount==2 && (Input.GetTouch (0).phase==TouchPhase.Moved || Input.GetTouch (1).phase==TouchPhase.Moved)){
			v2_current=Input.GetTouch(0).position - Input.GetTouch(1).position;
			v2_previous=(Input.GetTouch (0).position-Input.GetTouch (0).deltaPosition)-(Input.GetTouch(1).position-Input.GetTouch(1).deltaPosition);
			angle=Vector2.Angle(v2_previous,v2_current);
			if(angle>0.1){
				if(Vector3.Cross(v2_current,v2_previous).z<0){
					Debug.Log("counter clockwise");
					transform.Rotate(Vector3.up, angle*rotationspeed*-1);
				}
				else
				{
					transform.Rotate (Vector3.up, angle*rotationspeed);
					Debug.Log ("clockwise");
				}
			}
		}
		
		if(agent.velocity.sqrMagnitude>Vector3.zero.sqrMagnitude) 
		{
			anim.fwd (true);
		}
		else
		{
			if(FireSingle.FireBtn)
				anim.shoot (true);
			else 
			{
				anim.idle ();
				anim.shoot(false);	
			}
		}
		
		
		var pos = Tracking.transform.localPosition;
		pos.z = Mathf.Clamp(moveSlider.distanceSlider,3.0f,35.0f);
		Tracking.transform.localPosition=pos;
	}
	
	void OnCollisionEnter(Collision col){
		if(col.gameObject.tag=="obstacles")
		{
			target=transform.position;
			agent.SetDestination(target);
			obstaclehit=true;
		}
	}	
	
	void OnGUI()
	{
		GUI.skin=hitskin;		
		GUI.color=mytexture;
		GUI.Box (new Rect(Screen.width/2-75,Screen.height/2-200,150,60), "Obstacle Hit");
		if(obstaclehit)
		{					
			guiAlpha=1;
			obstaclehit=false;			
		}		
	}	
}
