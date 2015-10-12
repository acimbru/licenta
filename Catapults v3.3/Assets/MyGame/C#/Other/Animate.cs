using UnityEngine;
using System.Collections;

public class Animate : MonoBehaviour {
	
	public Animator animator;
	
	// Use this for initialization
	void Awake () {
		animator = GetComponent<Animator>();
	}
	
	void Start(){
	
	}
	
	public void idle () {  
			  animator.SetBool("fwd",false);
			  animator.SetBool("bwd",false);
			  animator.SetBool("left",false);
			  animator.SetBool("right",false);
    }
	
	public void resetidle () {  
			  animator.SetBool("fwd",false);
			  animator.SetBool("bwd",false);
			  animator.SetBool("left",false);
			  animator.SetBool("right",false);
			  animator.SetBool("death",false);
    }
	
	public void fwd (bool state) {  
		  animator.SetBool("fwd",state);
		  animator.SetBool("bwd",!state);
		  animator.SetBool("left",!state);
		  animator.SetBool("right",!state);
    }
	
	public void bwd (bool state) {  
		  animator.SetBool("fwd",!state);
		  animator.SetBool("bwd",state);
		  animator.SetBool("left",!state);
		  animator.SetBool("right",!state);
    } 
	
	public void left (bool state) {  
		  animator.SetBool("left",state);
		  animator.SetBool("right",!state);
		  animator.SetBool("fwd",!state);
		  animator.SetBool("bwd",!state);
    }
	
	public void right (bool state) {  
		  animator.SetBool("right",state);
		  animator.SetBool("fwd",!state);
		  animator.SetBool("bwd",!state);
		  animator.SetBool("left",!state);
    }
	
	public void shoot (bool state) {
		  animator.SetBool("shoot",state);
    }
	
	public void dead () {
		  animator.SetBool("death",true);
    }
}
