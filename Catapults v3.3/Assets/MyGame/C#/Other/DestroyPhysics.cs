using UnityEngine;
using System.Collections;

public class DestroyPhysics : MonoBehaviour {

	public float DestroyInSeconds = 6;
	
	// Use this for initialization
	void Start () {
	 	StartCoroutine("DestroyObject");
	}
	
	// Update is called once per frame
	void Update () {
	
	}
	
	IEnumerator DestroyObject() {
        yield return new WaitForSeconds(DestroyInSeconds);
		Destroy(gameObject);
    }
}
