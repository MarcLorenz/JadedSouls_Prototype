using UnityEngine;
using System.Collections;

public class Colliders : MonoBehaviour {

	public CapsuleCollider main;//deals with physics and collisions
	//rigidbody base;//whatever this collider is attached to
	public bool toggle = false;
	public bool toggle2 = false;
	public int count = 0;//slight delay on landing, prevents a hopping glitch

	// Use this for initialization
	void Start () {
		main = GetComponent<CapsuleCollider>();//gets the main capsule collider
		//base = GetComponent<Rigidbody>();
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	void FixedUpdate(){
		//Debug.Log();
		if(count > 0)
		  --count;
		if((rigidbody.velocity.y > 0 || toggle) && toggle2 && count == 0){
			main.isTrigger = true;
		}
		else {
			main.isTrigger = false;
		}
	}

	void OnCollisionEnter(Collision collision){
		foreach(ContactPoint contact in collision.contacts){
			if(contact.otherCollider.name == "Floor"){
				toggle2 = false;
				count = 3;
			}
		}
	}

	void OnCollisionExit(Collision collision){
		foreach(ContactPoint contact in collision.contacts){
			if(contact.otherCollider.name == "Floor"){
				toggle2 = true;
			}
		}
	}

	void OnTriggerEnter(Collider other)
	{
		if(other.name == "Floor")
		{
			count = 3;
			toggle2 = false;
		}
		
	}
}