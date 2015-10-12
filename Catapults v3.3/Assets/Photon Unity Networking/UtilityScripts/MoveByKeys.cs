using UnityEngine;
using System.Collections;


public class MoveByKeys : MonoBehaviour
{
        //Inputs Movement
	    public AnimationsState _characterState;
	
       	private Animate anim;
	    //Catapult Mechanics Movement
		private float topSpeedForward = 3.0f; 				// top speed of forward
		private float topSpeedReverse = -2.0f; 				// top speed of reverse
		
		private float accelerationRate = 1.0f; 				// rate at which top movement speed is reached
		private float rotationRate     = 20.0f; 				// rate at whitch top rotation speed is reached
		
		private float topRotationSpeedRight =6.0f;			// top speed rotation right	
		private float topRotationSpeedLeft  =-6.0f;			// mtop speed rotation left	
							
		private float brakeRate          = 0.005f; 			// rate at which speed is lost when not accelerating
		  
		private float currentMoveSpeed     = 0.0f; 			// stores current move speed
		private float currentRotationSpeed = 0.0f; 			// stores current rotation speed
	    private ScrollScript moveSlider;
		private GameObject SlidersObject;
	    private Fire ShootScript;
	    //Targeting Object
		public Transform Tracking;
        public float TrackingSpeed = 5;
        
		public bool isControllable=true;
        
	    void Start ()
        {
        	ShootScript=this.GetComponent<Fire>();
		    anim =  GetComponent<Animate>();
			SlidersObject = GameObject.FindGameObjectWithTag("Sliders");
			moveSlider=SlidersObject.GetComponent<ScrollScript>();
		}

        void Update ()
        {		
			if(isControllable){ 
				//As long as the joystick has been moved in x or y
			    if(Input.GetKey(KeyCode.UpArrow)){
			       //FORWARD
				   Acceleration("forward");
				   _characterState = AnimationsState.Forward;
			          
				   if(Input.GetKey(KeyCode.LeftArrow)){
					  //FORWARD LEFT
					  Rotation("left");
				   }else if(Input.GetKey(KeyCode.RightArrow)){
					  //FORWARD RIGHT
					  Rotation("right");
				   }
			    }else if(Input.GetKey(KeyCode.DownArrow)){
					 //BACKWARD
				 	 Acceleration("reverse");
					 _characterState = AnimationsState.Forward;
				   if(Input.GetKey(KeyCode.LeftArrow)){
					 //BACKWARD LEFT
					 Rotation("left");
			       }else if(Input.GetKey(KeyCode.RightArrow)){
					 //BACKWARD RIGHT	
					 Rotation("right");
				   }
		        }else if(Input.GetKey(KeyCode.RightArrow)){
					 //RIGHT
					 _characterState = AnimationsState.Right;
					 Rotation("right");
				}else if(Input.GetKey(KeyCode.LeftArrow)){
					 //LEFT
					 _characterState = AnimationsState.Left;
					 Rotation("left");
				}else{
					 //IDLE
					 if(ShootScript.FireBtn){
						_characterState = AnimationsState.Shoot;
					 }else{
						_characterState = AnimationsState.Idle;
					 }
			         
				}
				var pos = Tracking.transform.localPosition;
				pos.z = Mathf.Clamp(moveSlider.distanceSlider,3.0f,35.0f);
				Tracking.transform.localPosition=pos;
			}
		
			switch(_characterState){
				case AnimationsState.Idle:     anim.idle();
											   anim.shoot(false);
											   break;
				case AnimationsState.Forward:  anim.fwd(true);
											   break;
				case AnimationsState.Backward: anim.bwd(true);
											   break;
				case AnimationsState.Left:     anim.left(true);
			                                   break;
				case AnimationsState.Right:    anim.right(true);
											   break;
				case AnimationsState.Shoot:    anim.shoot(true);
											   break;
			}
		
        }
	
		void Acceleration(string motion){
	
			if(motion=="forward"){	
				if( currentMoveSpeed<0){		
					currentMoveSpeed+=accelerationRate+brakeRate;
				}
				else if(currentMoveSpeed<topSpeedForward){
					currentMoveSpeed+=accelerationRate;				
				}				
			    transform.Translate(0, 0, currentMoveSpeed*Time.deltaTime);
				return;
			}
		
			if(motion=="reverse"){
				if( currentMoveSpeed>0){
					currentMoveSpeed-=accelerationRate+brakeRate;
				}
				else if(currentMoveSpeed>topSpeedReverse){
					currentMoveSpeed-=accelerationRate;			
				}
				transform.Translate(0, 0, currentMoveSpeed*Time.deltaTime);
			}
		}

		void Rotation(string rot){
		
			if(rot=="right"){
				if( currentRotationSpeed<0){
					currentRotationSpeed+=rotationRate+brakeRate;
				}
				 if(currentRotationSpeed<topRotationSpeedRight){
					currentRotationSpeed+=rotationRate;				
				}
				this.transform.Rotate(0,currentRotationSpeed*Time.deltaTime,0);
			}
			
			if(rot=="left"){
				if( currentRotationSpeed>0){
					currentRotationSpeed-=rotationRate+brakeRate;
				}
				else if(currentRotationSpeed>topRotationSpeedLeft){
					currentRotationSpeed-=rotationRate;			
				}
				this.transform.Rotate(0,currentRotationSpeed*Time.deltaTime,0);
			}
		}
}