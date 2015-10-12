using UnityEngine;
using System.Collections;

[RequireComponent (typeof (AudioSource))]
public class SoundScripts : MonoBehaviour {

	
	
	public AudioClip shoot; //done
	public AudioClip dead;  //done
	public AudioClip hit_normal;
	public AudioClip hit_fire;
	public AudioClip hit_ice;
	public AudioClip bonus_on;
	public AudioClip bonus_picked;
	public AudioClip shield_on;
	public AudioClip shield_off;
	
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
	
	[RPC]
    void Shoot()
    {
        if (!this.enabled)
    	{
        	return;
    	}
		
		Debug.Log("Shoot");
 
        this.audio.clip = shoot;
        this.audio.Play();
    }
	
	[RPC]
    void Dead()
    {
        if (!this.enabled)
    	{
        	return;
    	}
		
		Debug.Log("Dead");
 
        this.audio.clip = dead;
        this.audio.Play();
    }
	
	[RPC]
    void Hit_N()
    {
        if (!this.enabled)
    	{
        	return;
    	}
		
		Debug.Log("Hit_N");
 
        this.audio.clip = hit_normal;
        this.audio.Play();
    }
	
	[RPC]
    void Hit_F()
    {
        if (!this.enabled)
    	{
        	return;
    	}
		
		Debug.Log("Hit_F");
 
        this.audio.clip = hit_fire;
        this.audio.Play();
    }
	
	[RPC]
    void Hit_I()
    {
        if (!this.enabled)
    	{
        	return;
    	}
		
		Debug.Log("Hit_I");
 
        this.audio.clip = hit_ice;
        this.audio.Play();
    }
	
	[RPC]
    void Bonus_On()
    {
        if (!this.enabled)
    	{
        	return;
    	}
		
		Debug.Log("Bonus_On");
 
        this.audio.clip = bonus_on;
        this.audio.Play();
    }
	
	[RPC]
    void Bonus_Picked()
    {
        if (!this.enabled)
    	{
        	return;
    	}
		
		Debug.Log("Bonus_Picked");
 
        this.audio.clip = bonus_picked;
        this.audio.Play();
    }
	
	[RPC]
    void Shield_On()
    {
        if (!this.enabled)
    	{
        	return;
    	}
		
		Debug.Log("Shield_On");
 
        this.audio.clip = shield_on;
        this.audio.Play();
    }
	
	[RPC]
    void Shield_Off()
    {
        if (!this.enabled)
    	{
        	return;
    	}
		
		Debug.Log("Shield_Off");
 
        this.audio.clip = shield_off;
        this.audio.Play();
    }
}
