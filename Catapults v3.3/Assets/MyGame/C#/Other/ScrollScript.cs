using UnityEngine;
using System.Collections;

public class ScrollScript : MonoBehaviour {

	public float lobSlider = 2; 
	public float distanceSlider = 15; 
	
	public GUISkin sliderSkin;
	private Vector3 defPosition = new Vector3(0,0,0);
	// Use this for initialization
	void Start () {
		
	}
	
	void OnGUI() {
		if(this.transform.parent.localPosition == defPosition){
			GUI.skin = sliderSkin;
			
	        GUI.BeginGroup(new Rect(Screen.width*0.9f-150,Screen.height-300,250,280));
				GUI.Box(new Rect(0,0,230,280),"");
		    	distanceSlider = GUI.VerticalSlider(new Rect(47,40,40.0f,220.0f),distanceSlider,35.0f,3.0f);
			    lobSlider = GUI.VerticalSlider(new Rect(155,40,40.0f,220.0f),lobSlider,3.0f,1.0f);
			GUI.EndGroup();
		}
    }
}
