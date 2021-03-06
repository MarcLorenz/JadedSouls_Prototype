﻿using UnityEngine;
using System.Collections;

public class Entity : MonoBehaviour {


	public const int RIGHT = 1;//right
	public const int LEFT = -1;//left
	public const int PLATFORMS = 8;//platform layer
	public const float GRAVITY_DEF = -0.5f;
	public float FRICTION = 1.2f;
	public float fallSpeed = 10f;//modified fall speed
	public float gravity = 20f;//225F;//20 * 9.8;
	public bool isGrounded = false;//is on ground
	public bool isStatic = true;//is this affected by certain props

	public int facing;//right = 1, left = -1

	//components
	public Animator anim;
	public CharacterController controller;//get the charactercontroller
	BoxCollider body_base;
	public BoxCollider bottom;//horrible name imo but base is taken
	public int layer = 0;//do not allow active entities to have the same layer
	public Rigidbody calcPhy;//REMOVE

	//variables
	public int delay;//stop recieving controls for x frames
	public bool isGravity;
	public float currSpeed;//current speed w/ modifier
	public Vector3 moveVect = Vector3.zero;



	public float abs(float arg){
		if (arg > 0)
			return arg;
		else
			return -arg;
	}

	// Use this for initialization
	
	public virtual void Awake()
	{
		//body_base = GetComponent<BoxCollider>();
		isGravity = true;
		bottom = GetComponent<BoxCollider>();
		anim = GetComponent <Animator> ();
		moveVect = new Vector3(0, 0, 0);//initialize the move vector
		controller = GetComponent<CharacterController>();
		calcPhy = GetComponent<Rigidbody>();
		delay = 8;
		if(transform.rotation.eulerAngles.y > 90)
			facing = RIGHT;
		else
			facing = LEFT;
	}

	public virtual void Start () {
	
	}
	
	// Update is called once per frame
	public virtual void Update () {
		/*controls character movement*/
		isGrounded = controller.isGrounded;

		if (isGravity && isGrounded)
			moveVect.y = GRAVITY_DEF;

		if(transform.position.z != 0){
			transform.position = new Vector3(transform.position.x,transform.position.y, 0); 
		}//STAY ON THE X AXIS DANGIT

		Gravity(isGravity && !isGrounded);
		
		if(isStatic)
			Friction();

		Animating();

		controller.Move(moveVect * Time.deltaTime);//move
	}
	
	public virtual void Friction(){
		if(isStatic)
			moveVect.x /= FRICTION;
	
		if(abs(moveVect.x) <= 0.01)
			moveVect.x = 0;
	}

	public virtual void OnCollisionEnter(Collision collisionInfo){

	}//OnCollisionExit

	public virtual void OnCollisionStay(Collision collisionInfo){
	}	

	public virtual void OnCollisionExit(Collision collisionInfo){
		//if(collisionInfo.other.name == "Platform" 
		  // || collisionInfo.other.name == "Floor")
			//isGrounded = false;
	}//OnCollisionExit

	public virtual void OnTriggerEnter(Collider other){
		if(other.GetComponent<Collider>().name == "Floor" 
		   || other.GetComponent<Collider>().name == "Platform"){
		//	delay = landFrames;
			
		}//if landing, delay movement
		
		if(other.GetComponent<Collider>().name == "Edge")
		{
			moveVect = Vector3.zero;
			controller.transform.position = new Vector3(2, 6, 0);
		}
		
		//Debug.Log(other.name);
		
		
	}//OnTriggerEnter



	//public virtual void OnTriggerStay(Collider other){
		/*if (other.name == "Floor" 
		    || other.name == "Platform"){
			
				if(moveVect.x > 0)
					moveVect.x -= FRICTION;
				else
			  	  if(moveVect.x < 0)
				 	moveVect.x += FRICTION;

			    if(abs(moveVect.x) <= FRICTION)
				  moveVect.x = 0;
			}*/
	//}	
	
	//only really matters when the collider is disabled
	public virtual void OnTriggerExit(Collider other){
		//bottom.isTrigger = false;
		//if (other.GetComponentsInParent<Entity> () == null)
		//				moveVect.x = 0;
	}//OnTriggerExit

	public virtual void Pushed(Vector3 forces){
		moveVect.x += forces.x/50;//10 is a placeholder
	}

	public virtual void Gravity(bool on = true, float modifier = 1f){
		if (on)
			moveVect.y -= gravity * Time.deltaTime * modifier;
	}//apply gravity

	public virtual void Animating(){
	}//animations


}
