using UnityEngine;
using System.Collections;

public class PlayerJump : MonoBehaviour {

	public bool isGrounded = true;
	public int jumpPower = 1;

	// Use this for initialization
	void Start(){
	}
	
	// Update is called once per frame
	void Update () {

		if ((Input.GetButtonDown("Jump") || Input.GetKey("space")) && isGrounded)
		{
			Jump();
		}
	}

	void Jump(){
		Debug.Log(isGrounded+"1");
			rigidbody.AddForce(new Vector3(0, jumpPower, 0), ForceMode.Force);
		isGrounded = false;
		}

	void FixedUpdate(){

		isGrounded = Physics.Raycast(transform.position, -Vector3.up, 0.5f);

		Debug.Log (isGrounded);
		}

}