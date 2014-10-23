using UnityEngine;
using System.Collections;

public class PlayerMovement : MonoBehaviour 
{
	public float speed = 6f;

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
	}

	void Animating (float h)
	{
		bool running = h != 0f;
		anim.SetBool ("IsRunning", running);
	}
}
