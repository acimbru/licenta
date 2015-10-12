using UnityEngine;
using System.Collections;

public class WorldPicker : MonoBehaviour {

	public GameObject Brick_Fortress;
	public GameObject Forest;
	public GameObject Wood_Fortress;
	
	private int leveltype;
	
	// Use this for initialization
	void Awake () {
		leveltype = PlayerPrefs.GetInt("map");
	     
		switch(leveltype){
			case 0: Brick_Fortress.SetActive(true);
					Forest.SetActive(false);
					Wood_Fortress.SetActive(false);
			        break;
			case 1:	Brick_Fortress.SetActive(false);
					Forest.SetActive(true);
					Wood_Fortress.SetActive(false);
			        break;
			case 2: Brick_Fortress.SetActive(false);
					Forest.SetActive(false);
					Wood_Fortress.SetActive(true);
			        break;
		}
	}
}
