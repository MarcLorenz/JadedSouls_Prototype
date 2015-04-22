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
			pressed = true;//button is being held down
			Jump();//what is says on the label -_-
		}

		if((Input.GetButtonUp("Jump") || Input.GetKeyUp("space"))){
			pressed = false;//you don't say? D:
		} //button is no longer down
	}

	void Jump(){
		cur = GetComponent<Rigidbody>().velocity;//gets current velocity
		cur.y = 0f;//sets the y vel to 0
		GetComponent<Rigidbody>().velocity = cur;//set current vertical vel to 0
		GetComponent<Rigidbody>().AddForce(new Vector3(0, jumpPower, 0), ForceMode.Force);
		isGrounded = false;//model is now in air
	}

	void FixedUpdate(){

		isGrounded = Physics.Raycast(GetComponent<Rigidbody>().transform.position, Vector3.down, 0.1f);
		
		if(isGrounded && GetComponent<Rigidbody>().velocity.y < -.1){
			jumps = 2;//once character lands, you get two jumps
		}//second condition prevents isGrounded activating after jump

	}

}