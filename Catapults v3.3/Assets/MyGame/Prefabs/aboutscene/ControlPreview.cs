using UnityEngine;
using System.Collections;

public class ControlPreview : MonoBehaviour {
	public float zoomspeed=0.5f;
	public float rotatespeed=0.05f;
	public GameObject target;
	public Transform pivot;	
	public GUISkin myskin;
	public GameObject catapult;
	public GameObject[] projectiles;
	public Transform spawn;
	private GameObject myprojectile;
	private bool spawned=false;
	private Animate anim;
	
	//private variables
	private string direction="";	
	private Vector3 initialAngles;

	void Start () {		
		anim = catapult.GetComponent<Animate>();
	}
	
	void OnGUI()
	{
		//info
		GUI.skin=myskin;
		string mytext="Created by:\n\nCimbru Andrei\nAnita Silviu";
		string usv="USV 2014";
		GUI.Label (new Rect(Screen.width-200,10,200,200),mytext);
		GUI.Label(new Rect(10,Screen.height-40,120,40),usv);
		
		//animations
		if(GUI.Button(new Rect(5,10,150,60),"Projectiles"))
		{
			if(spawned==false)
			{
				myprojectile=Instantiate(projectiles[Random.Range(0,projectiles.Length)],spawn.position,Quaternion.identity) as  GameObject;
				myprojectile.transform.parent=spawn;
				spawned=true;
			}
			else{
				Destroy(myprojectile);
				myprojectile=Instantiate(projectiles[Random.Range(0,projectiles.Length)],spawn.position,Quaternion.identity) as  GameObject;
				myprojectile.transform.parent=spawn;
			}
		}
		
		if(GUI.Button(new Rect(5,90,150,60),"Shoot"))
		{
			if(spawned==true)
			{
				Destroy(myprojectile);
				spawned=false;	
			}
			anim.shoot(true);
			StartCoroutine(ResetIdleRoutine());
		}
		
		if(GUI.Button(new Rect(5,170,150,60),"Death"))
		{
			if(spawned==true)
			{
				Destroy(myprojectile);
				spawned=false;			
			}
			anim.dead ();
			StartCoroutine(ResetIdleRoutine());	
		}
	}
	
	IEnumerator ResetIdleRoutine(){
			yield return new WaitForSeconds(0.1f);
			anim.resetidle ();
			anim.shoot (false);
	}

	void Update () {
		if (Input.GetKeyDown(KeyCode.Escape)){
			Application.LoadLevel(0);
		}
		//set the camera to look at the the catapult all the time
		transform.LookAt(target.transform.position);
		//camera zoom		
		
		if(Input.touchCount==2)
			{			
				Touch touch0=Input.GetTouch(0);
				Touch touch1=Input.GetTouch(1);
				
				Vector2 touch0prev=touch0.position - touch0.deltaPosition;
				Vector2 touch1prev=touch1.position - touch1.deltaPosition;
				
				float prevdistance=(touch0prev-touch1prev).magnitude;
				float curentdistance=(touch0.position-touch1.position).magnitude;
				
				float delta=(prevdistance - curentdistance)*zoomspeed;			
				camera.fieldOfView += delta;
	            camera.fieldOfView = Mathf.Clamp(camera.fieldOfView, 10f, 90f);
	
			}
		
		//rotate object
		else if(Input.touchCount==1)
		{
			Touch touch=Input.GetTouch (0);
			Vector2 touchDeltaPosition = touch.deltaPosition;
			if(Mathf.Abs(touchDeltaPosition.y)>Mathf.Abs(touchDeltaPosition.x)) direction="vertical";
				else direction="horizontal";
		if(direction=="vertical")
			{			
			target.transform.RotateAround(pivot.right ,-touchDeltaPosition.y * rotatespeed);
			
//			initialAngles = target.transform.eulerAngles;
//			initialAngles.x = Mathf.Clamp(initialAngles.x, -30, 30);
//			initialAngles.y = Mathf.Clamp(initialAngles.y, -30, 30);
//			target.transform.eulerAngles = initialAngles;
			}
		else if(direction=="horizontal"){
			target.transform.RotateAround(target.transform.up,-touchDeltaPosition.x*rotatespeed);
			}
		}
	}
}
