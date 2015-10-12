using UnityEngine;
using System.Collections;

public class Fire : MonoBehaviour {
	//public vars
    public bool FireBtn = false;
	public Transform LaunchCupPoint;
	public Transform LineRenderStart;
	public Transform LineRenderEnd;
	public GameObject[] Projectile ;
	public Transform ProjectileHolder;
	public GameObject windGen;
	public bool enablePrediction = true;

	public Texture2D[] LaunchButtons;
	public LineRenderer predictionLine;
	public GameObject ProjectileClone ;
	public GameObject SoundManager;
	public int shootCounter;
	public bool isControllable;
	public bool Desktop;
	//private vars
    private float lob=2;
	private Vector3 startingVelocity;
	private GUITexture LaunchButton;
	private GameObject SliderObject;
	private ScrollScript slider;
	private PhotonView myView; 
	private PhotonView mySoundView; 
	//Catapult State 
	// 1 - can shoot
	// 0 - reloading
	private float _doubleTapTimeD;
	private bool CanShootbutton;
	private Transform[] winds;
    
	
	void Awake(){
		Desktop=true;
		#if UNITY_ANDROID
    		Desktop=false;
		#endif
	}
	
	void Start(){
		myView = this.GetComponent<PhotonView>();
		SliderObject = GameObject.FindGameObjectWithTag("Sliders");
		slider = SliderObject.GetComponent<ScrollScript>();
		LaunchButton = GameObject.FindGameObjectWithTag("firebutton").GetComponent<GUITexture>();
		CanShootbutton=true;
		predictionLine= GetComponent<LineRenderer>();
		predictionLine.enabled=true;
		predictionLine.SetWidth(0.2f,0.2f);
		LaunchButton.texture = LaunchButtons[0];
		
		Transform[] children = this.GetComponentsInChildren<Transform>();
		foreach(Transform child in children){
		    if(child.gameObject.tag == "Holder"){
		        ProjectileHolder=child;
    	    }
        }
		
		winds  = this.transform.parent.GetComponentsInChildren<Transform>();
		
		foreach( Transform child in winds){
			if(child.CompareTag("windGenerator")){		
				  windGen = child.gameObject;
			}		
		}
	}
	
	/*
	void OnGUI(){
		if(Desktop){
			if(isControllable){
				if(GUI.Button(new Rect(50,50,100,100),"Shoot")){
					StartCoroutine("Shoot");
				}
			}
		}
	}
	*/
	
	// Update is called once per frame
	void Update () {
		if(isControllable){
					
			startingVelocity = GetTrajectoryVelocity(LineRenderStart.position, LineRenderEnd.position, lob, Physics.gravity);				
			lob=slider.lobSlider;

			if(enablePrediction)
				UpdatePredictionLine();
			
			if(shootCounter>=0){
				bool doubleTapD = false;
				
				predictionLine.enabled=true;
				
				if (Input.GetMouseButtonDown(0))
		        {
		            if (Time.time < _doubleTapTimeD + .3f)
		            {
		                doubleTapD = true;
		            }
		            _doubleTapTimeD = Time.time;
		        }
				
		        if (doubleTapD)
		        {
		            Debug.Log("DoubleTapD");
					int id1 = PhotonNetwork.AllocateViewID();
				    myView.RPC("ProjectileType",PhotonTargets.AllBuffered,"projectile_rock",id1,0);
					LaunchButton.texture = LaunchButtons[0];
		        }
				
				if(!Desktop){
					foreach (Touch touch in Input.touches){
						if(Input.touchCount > 0){
							if(LaunchButton.HitTest(touch.position)){
								LaunchButton.texture=LaunchButtons[1];
								StartCoroutine("Shoot");
							}
						}
					}
				}else{
					if(Input.GetMouseButtonDown(0) && LaunchButton.HitTest(Input.mousePosition) || Input.GetMouseButton(0) && LaunchButton.HitTest(Input.mousePosition)){
								LaunchButton.texture=LaunchButtons[1];
								StartCoroutine("Shoot");
					}
				}
			}
		}else{
			predictionLine.enabled=false;
		}
		
		//enable/disable button
		if(CanShootbutton){
			LaunchButton.enabled = true;
		}else{
			LaunchButton.enabled = false;
		}
	}
	 
	IEnumerator Shoot(){
		if(ProjectileClone!=null){
			shootCounter-=1;
			predictionLine.enabled=false;
			FireBtn=true;
			yield return new WaitForSeconds(1.75f);
			FireBtn=false;
			myView.RPC("Firing",PhotonTargets.All);	
			CanShootbutton=false;
		}
	}
	
	[RPC]
	void Firing(){
		if(ProjectileClone!=null){	
		    ProjectileClone.transform.parent = this.transform.parent;
			ProjectileClone.collider.enabled = true;
			ProjectileClone.rigidbody.isKinematic = false;
			ProjectileClone.rigidbody.AddForce(startingVelocity,ForceMode.Impulse);
			ProjectileClone = null;
		}
	}
	
	[RPC]
	void ProjectileType(string resourceName,int id1, int shootNumber){		 
			if(ProjectileClone!=null){
				Destroy(ProjectileClone);
		    }
		    
		    ProjectileClone = Instantiate(Resources.Load(resourceName),LaunchCupPoint.position,LaunchCupPoint.rotation) as GameObject;
			ProjectileClone.transform.parent = ProjectileHolder;
		    
		    PhotonView[] nViews = ProjectileClone.GetComponentsInChildren<PhotonView>();
            nViews[0].viewID = id1;	
		    windGen.GetPhotonView().RPC ("ApplyWind",PhotonTargets.All);
			shootCounter = shootNumber;
			CanShootbutton=true;
	}
	
	void ChangeProjectile(object[] received){
		int id2 = PhotonNetwork.AllocateViewID();
		object[] data = received;
		myView.RPC("ProjectileType",PhotonTargets.AllBuffered,(string)data[0],id2,(int)data[1]);
	}
	
	void UpdatePredictionLine() 
	{
		predictionLine.SetVertexCount(180);
		Vector3 previousPosition = LineRenderStart.position;
		for(int i = 0; i < 180; i++)
		{
			Vector3 currentPosition = GetTrajectoryPoint(LineRenderStart.position, startingVelocity, i, 1, Physics.gravity);
			Vector3 direction = currentPosition - previousPosition;
			direction.Normalize();
			
			float distance = Vector3.Distance(currentPosition, previousPosition);
			
			RaycastHit hitInfo = new RaycastHit();
			if(Physics.Raycast(previousPosition, direction, out hitInfo, distance))
			{
				predictionLine.SetPosition(i,hitInfo.point);
				predictionLine.SetVertexCount(i);
				break;
			}
			
			previousPosition = currentPosition;
			predictionLine.SetPosition(i,currentPosition);
		}
	}
	
	Vector3 GetTrajectoryPoint(Vector3 startingPosition, Vector3 initialVelocity, float timestep, float lob, Vector3 gravity)
	{
    	float physicsTimestep = Time.fixedDeltaTime;
    	Vector3 stepVelocity = initialVelocity * physicsTimestep;
		
		//Gravity is already in meters per second, so we need meters per second per second
		Vector3 stepGravity = gravity * physicsTimestep * physicsTimestep;
		
		return startingPosition + (timestep * stepVelocity) + ((( timestep * timestep + timestep) * stepGravity ) / 2.0f);
	}
	
	public static Vector3 GetTrajectoryVelocity(Vector3 startingPosition, Vector3 LineRenderEndPosition, float lob, Vector3 gravity)
	{
		float physicsTimestep = Time.fixedDeltaTime;
	    float timestepsPerSecond = Mathf.Ceil(1f/physicsTimestep);
		
		//By default we set n so our projectile will reach our LineRenderEnd point in 1 second
	    float n = lob * timestepsPerSecond;
		
	    Vector3 a = physicsTimestep * physicsTimestep * gravity;
		Vector3 p = LineRenderEndPosition;
		Vector3 s = startingPosition;
	    
		Vector3 velocity = (s + (((n * n + n) * a) / 2f) - p) * -1 / n;
		
		//This will give us velocity per timestep. The physics engine expects velocity in terms of meters per second
		velocity /= physicsTimestep;
		return velocity;
	}
	
	void OnEndGame(){
        isControllable = false;
    }
}
