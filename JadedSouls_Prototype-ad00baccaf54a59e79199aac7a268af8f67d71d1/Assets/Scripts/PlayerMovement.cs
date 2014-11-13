using UnityEngine;
using System.Collections;
public class PlayerMovement : MonoBehaviour
{
	public float speed = 6f;
	public Vector3 newPos;
	PlayerJump jump;
	public float air_factor = 3f/4f;
	public bool running = false;
	public int facing;
	public float h;
	public float v;
	public Colliders shell;
	

	Vector3 movement;
	Animator anim;
	Rigidbody playerRigidbody;
	int floorMask;
	Vector3 down, drop;

	void Awake()
	{
		h = v = 0;
		floorMask = LayerMask.GetMask ("Floor");
		anim = GetComponent <Animator> ();
		playerRigidbody = GetComponent <Rigidbody> ();
		jump = playerRigidbody.GetComponent <PlayerJump>();
		shell = playerRigidbody.GetComponent<Colliders>();
		down = new Vector3(0, -60f, 0);
		drop = new Vector3(0, -25f, 0);
		if(transform.rotation.eulerAngles.y > 90){
			facing = -1;
		}
		else{
			facing = 1;
		}
	}

	void Update(){
		h = Input.GetAxisRaw ("Horizontal");
		v = Input.GetAxisRaw ("Vertical");
	}//this might help with press vs lag?


	void FixedUpdate()
	{
		Move (h);
		Animating (h);
	}

	void Move (float h)
	{
		if(jump.isGrounded){
			movement.Set (h, 0f, 0f);
		}
		else {
			movement.Set (h * air_factor, 0f, 0f);
		}
		movement = movement * speed * Time.deltaTime;
		playerRigidbody.MovePosition(transform.position + movement);

		if (facing < 0 && h > 0 ){
			playerRigidbody.transform.Rotate(Vector3.up, 180);
			facing = 1;
		}
		if (facing > 0 && h < 0){
			playerRigidbody.transform.Rotate(Vector3.up, -180);
			facing = -1;
		}

		if(v < 0)
		{
			shell.toggle = true;
			//if(jump.isGrounded)
			  //playerRigidbody.AddForce(drop, ForceMode.VelocityChange);
			//else {
				playerRigidbody.AddForce(down, ForceMode.Impulse);
			//}
		}
		else {
			shell.toggle = false;
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