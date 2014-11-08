using UnityEngine;
using System.Collections;
public class PlayerMovement : MonoBehaviour
{
	public Vector3 Right;
	public Vector3 Left;
	public float speed = 6f;
	public Vector3 newPos;
	PlayerJump jump;
	public float jump_factor = 3f/4f;
	public bool running = false;

	Vector3 movement;
	Animator anim;
	Rigidbody playerRigidbody;
	int floorMask;
	void Awake()
	{
		floorMask = LayerMask.GetMask ("Floor");
		anim = GetComponent <Animator> ();
		playerRigidbody = GetComponent <Rigidbody> ();
		jump = playerRigidbody.GetComponent <PlayerJump>();		

		Right = new Vector3 (100000 , 0, 0);
		Left = new Vector3 (-100000, 0, 0);
	}
	void FixedUpdate()
	{
		float h = Input.GetAxisRaw ("Horizontal");
		float v = Input.GetAxisRaw ("Vertical");
		Move (h);
		Animating (h);
	}
	void Move (float h)
	{
		if(jump.isGrounded){
			movement.Set (h, 0f, 0f);
		}
		else {
			movement.Set (h * jump_factor, 0f, 0f);
		}
		movement = movement * speed * Time.deltaTime;
		playerRigidbody.MovePosition (transform.position + movement);

		if ((transform.position != Right) && h > 0 ){
			transform.LookAt(Right);
		}
		if ((transform.position != Left) && h < 0){
			transform.LookAt(Left);
		}
	}
	void Animating (float h)
	{
		if(jump.isGrounded){
			running = h != 0f;
		}
		else {
			running = false;
		}
		anim.SetBool ("IsRunning", running);
	}
}