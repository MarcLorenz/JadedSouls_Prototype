using UnityEngine;
using System.Collections;

public class PlayerJump : MonoBehaviour {

	public bool isGrounded = true;
	public int jumpPower = 20000;
	bool pressed = false;
	public int jumps = 2;
	Vector3 cur;

	// Use this for initialization
	void Start(){
	}
	
	// Update is called once per frame
	void Update () {

		if ((Input.GetButtonDown("Jump") || Input.GetKey("space")) && (isGrounded ||  jumps > 0) && !pressed)
		{
			--jumps;
			pressed = true;
			Jump();
		}

		if((Input.GetButtonUp("Jump") || Input.GetKeyUp("space"))){
			pressed = false;
		} 
	}

	void Jump(){
		rigidbody.useGravity = false;//turn off gravity, set v vel to 0, turn back on
		cur = rigidbody.velocity;
		cur.y = 0f;
		rigidbody.velocity = cur;//set current vertical vel to 0
		rigidbody.AddForce(new Vector3(0, jumpPower, 0), ForceMode.Force);
		rigidbody.useGravity = true;
		isGrounded = false;
		}

	void FixedUpdate(){

		isGrounded = Physics.Raycast(transform.position, -Vector3.up, 0.1f);
		
		if(isGrounded && !pressed)
			jumps = 2;

		}

}