
using UnityEngine;

/// <summary>
/// A custom handler that implements the ITrackableEventHandler interface.
/// </summary>
public class DefaultTrackableEventHandler : MonoBehaviour,
                                            ITrackableEventHandler
{
	public GUISkin practiceskin;
    #region PRIVATE_MEMBER_VARIABLES
 
    private TrackableBehaviour mTrackableBehaviour;
    
    #endregion // PRIVATE_MEMBER_VARIABLES



    #region UNTIY_MONOBEHAVIOUR_METHODS
    
    void Start()
    {
        mTrackableBehaviour = GetComponent<TrackableBehaviour>();
        if (mTrackableBehaviour)
        {
            mTrackableBehaviour.RegisterTrackableEventHandler(this);
        }
		PlayerPrefs.SetInt("instantiat",0);
		PlayerPrefs.SetInt("found",0);
		PlayerPrefs.SetInt("buttonshow",1);
		Screen.SetResolution(1280,720,true);
    }

    #endregion // UNTIY_MONOBEHAVIOUR_METHODS


	public Object world1;
	public Object world2;
	public Object world3;
	private bool found = false;	
	public Texture green;
	public Texture red;
    #region PUBLIC_METHODS

    /// <summary>
    /// Implementation of the ITrackableEventHandler function called when the
    /// tracking state changes.
    /// </summary>
    public void OnTrackableStateChanged(
                                    TrackableBehaviour.Status previousStatus,
                                    TrackableBehaviour.Status newStatus)
    {
        if (newStatus == TrackableBehaviour.Status.DETECTED ||
            newStatus == TrackableBehaviour.Status.TRACKED ||
            newStatus == TrackableBehaviour.Status.EXTENDED_TRACKED)
        {
            OnTrackingFound();
			found=true;
			PlayerPrefs.SetInt("found",1);
        }
        else
        {
            OnTrackingLost();
			found=false;
			PlayerPrefs.SetInt("found",0);
        }
    }
	
	public void InstantiateWorld(){	
		GameObject gameworld;
		string marker=PlayerPrefs.GetString("markername");
		int instanced=PlayerPrefs.GetInt("instantiat");
		if(instanced==0)
			switch(marker){
		case "map1c":
			gameworld=Instantiate(world1, new Vector3(0,0,0), Quaternion.identity) as GameObject;
			gameworld.transform.parent = this.transform;
			PlayerPrefs.SetInt("instantiat",1);
			break;
		
		case "map2c":
			gameworld=Instantiate(world2, new Vector3(0,0,0), Quaternion.identity) as GameObject;
			gameworld.transform.parent = this.transform;
			PlayerPrefs.SetInt("instantiat",1);
			break;
		case "map3c":			
			gameworld=Instantiate(world3, new Vector3(0,0,0), Quaternion.identity) as GameObject;
			gameworld.transform.parent = this.transform;
			PlayerPrefs.SetInt("instantiat",1);
			break;
		}					
		
		
	}
	public void Update(){
		if (Input.GetKeyDown(KeyCode.Escape))
			Application.LoadLevel(0);
	}
	public void OnGUI(){
		GUI.skin=practiceskin;
		int detect=PlayerPrefs.GetInt("found");
		int buttonshow=PlayerPrefs.GetInt("buttonshow");
		if(detect==1)
		{
			GUI.DrawTexture(new Rect(Screen.width-30,10, 20,20),green);
			if(buttonshow==1){
				if(GUI.Button(new Rect(Screen.width/2-75,Screen.height/2-25, 150,50),"Start game")){
					InstantiateWorld();
					PlayerPrefs.SetInt("buttonshow",0);
				}
		    }	
		}
		else
		{
			GUI.DrawTexture(new Rect(Screen.width-30,10, 20,20),red);			
		}
					
	}
    #endregion // PUBLIC_METHODS



    #region PRIVATE_METHODS


    private void OnTrackingFound()
    {		
        Renderer[] rendererComponents = GetComponentsInChildren<Renderer>(true);
        Collider[] colliderComponents = GetComponentsInChildren<Collider>(true);

        // Enable rendering:
        foreach (Renderer component in rendererComponents)
        {
            component.enabled = true;
        }

        // Enable colliders:
        /*foreach (Collider component in colliderComponents)
        {
          //  component.enabled = true;
        }
		 */
        Debug.Log("Trackable " + mTrackableBehaviour.TrackableName + " found");
		PlayerPrefs.SetString("markername",mTrackableBehaviour.TrackableName);
		
		
    }


    private void OnTrackingLost()
    {
        Renderer[] rendererComponents = GetComponentsInChildren<Renderer>(true);
        Collider[] colliderComponents = GetComponentsInChildren<Collider>(true);

        // Disable rendering:
        foreach (Renderer component in rendererComponents)
        {
            component.enabled = false;
        }

        /* Disable colliders:
        foreach (Collider component in colliderComponents)
        {
           // component.enabled = false;
        }
		*/
        Debug.Log("Trackable " + mTrackableBehaviour.TrackableName + " lost");
    }

    #endregion // PRIVATE_METHODS
}
