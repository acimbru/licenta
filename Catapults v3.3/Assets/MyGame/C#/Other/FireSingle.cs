using UnityEngine;
using System.Collections;

public class FireSingle : MonoBehaviour {
	//public vars
    public static bool FireBtn = false;
	public Transform LaunchCupPoint;
	public Transform LineRenderStart;
	public Transform LineRenderEnd;
	public GameObject[] Projectile ;
	public GameObject ProjectileHolder;
	
	public bool enablePrediction = true;
	private float lob=2;
	public Texture2D[] LaunchButtons;
	
	//private vars
	private LineRenderer predictionLine;
	private Vector3 startingVelocity;
	private GameObject ProjectileClone ;
	private GUITexture LaunchButton;
	private GameObject SliderObject;
	public bool Desktop;
	private ScrollScript slider;
	
	//Catapult State 
	// 1 - can shoot
	// 0 - reloading
	private int shootCounter;
	
	void Awake(){
		Desktop=true;
		#if UNITY_ANDROID
    		Desktop=false;
		#endif
	}
	
	void Start(){
		SliderObject = GameObject.FindGameObjectWithTag("Sliders");
		slider = SliderObject.GetComponent<ScrollScript>();
		LaunchButton = GameObject.FindGameObjectWithTag("firebutton").GetComponent<GUITexture>();
		
		predictionLine= GetComponent<LineRenderer>();
		predictionLine.enabled=true;
		predictionLine.SetWidth(0.2f,0.2f);
		NormalProjectile(1);
	}
	
	// Update is called once per frame
	void Update () {
		startingVelocity = GetTrajectoryVelocity(LineRenderStart.position, LineRenderEnd.position, lob, Physics.gravity);		
		
		lob=slider.lobSlider;
		
		if(enablePrediction)
			UpdatePredictionLine();
		if(shootCounter>0){
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
		}else{
			LaunchButton.texture=LaunchButtons[2];
		}
	}
	
	IEnumerator Shoot(){
		shootCounter-=1;
		yield return new WaitForSeconds(0.1f);
		LaunchButton.texture=LaunchButtons[2];
		FireBtn=true;
		yield return new WaitForSeconds(1.75f);
		FireBtn=false;
		ProjectileClone.transform.parent = this.transform.parent;
		ProjectileClone.collider.enabled = true;
		ProjectileClone.rigidbody.isKinematic = false;
		ProjectileClone.rigidbody.AddForce(startingVelocity,ForceMode.Impulse);
		ProjectileClone = null;
		yield return new WaitForSeconds(1.5f);
		NormalProjectile(1);
	}
	
	void NormalProjectile(int numberOfShots){
		LaunchButton.texture = LaunchButtons[0];
		ProjectileClone = Instantiate(Projectile[0],LaunchCupPoint.position,LaunchCupPoint.rotation) as GameObject;
		ProjectileClone.transform.parent = ProjectileHolder.transform;
		shootCounter = numberOfShots; 
	}
	
	void FireProjectile(){
		LaunchButton.texture = LaunchButtons[0];
		Destroy(ProjectileClone);
		ProjectileClone = Instantiate(Projectile[1],LaunchCupPoint.position,LaunchCupPoint.rotation) as GameObject;
		ProjectileClone.transform.parent = ProjectileHolder.transform;
		shootCounter = 1; 
	}
	
	void IceProjectile(){
		LaunchButton.texture = LaunchButtons[0];
		Destroy(ProjectileClone);
		ProjectileClone = Instantiate(Projectile[2],LaunchCupPoint.position,LaunchCupPoint.rotation) as GameObject;
		ProjectileClone.transform.parent = ProjectileHolder.transform;
		shootCounter = 1; 
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
}
