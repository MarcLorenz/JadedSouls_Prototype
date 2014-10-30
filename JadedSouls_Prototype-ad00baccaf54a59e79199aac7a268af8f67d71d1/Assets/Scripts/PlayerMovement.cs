using UnityEngine;
using System.Collections;

public class PlayerMovement : MonoBehaviour 
{
	public Vector3 Right;
	public Vector3 Left;
	public float speed = 6f;
	public Vector3 newPos;

	Vector3 movement;
	Animator anim;
	Rigidbody playerRigidbody;
	int floorMask;

	void Awake()
	{
		floorMask = LayerMask.GetMask ("Floor");
		anim = GetComponent <Animator> ();
		playerRigidbody = GetComponent <Rigidbody> ();
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
		movement.Set (h, 0f, 0f);

		movement = movement * speed * Time.deltaTime;

		playerRigidbody.MovePosition (transform.position + movement);

		Right = new Vector3 (100000 , 0, 0);
		Left = new Vector3 (-100000, 0, 0);

		if ((transform.position != Right) && Input.GetKey("d")){
			transform.LookAt(Right);
		}
		if ((transform.position != Left) && Input.GetKey("a")){
			transform.LookAt(Left);
		}
	}
	
	void Animating (float h)
	{
		bool running = h != 0f;
		anim.SetBool ("IsRunning", running);
	}
}
