using System;
using UnityEngine;
using System.Collections;

public class Player : MonoBehaviour {
/*player class: controls movement of a player character using CharacterController
-Currently using a constant position change vector (moveVect) to control movement
*/
	//constants
	const int RIGHT = 1;//right
	const int LEFT = -1;//left
	const int PLATFORMS = 8;//platform layer
	public float THRESHOLD = 0.85f;//controller sensitivity essentially
	public int CHANGE_DIR = 20;
	const float GRAVITY_DEF = -0.5f;
	 int[] playerLayers = {9, 10, 11, 12};



	//presets for character
	public float speed = 8F;//ground movement speed
	public float walkSpeed = 2F;
	public float jumpSpeed = 10f;//force of jump
	public float fallSpeed = 10f;//modified fall speed
	public float gravity = 21f;//225F;//20 * 9.8;
	//public float maxFallSpeed = 40f;
	public float maxSpeed = 20f;
	public float airMod = 3/5f;//modify speed if in air
	public int maxJumps = 2;//maximum number of jumps
	public int landFrames = 7;//when you land from air freeze

	//general info about state of character
	public int animState = 0;
	public bool running = false;
	public int facing;//right = 1, left = -1
	public bool isGrounded = false;//is on ground
	public bool isCrouching = false;//is crouching
	public int jumps = 0;//jumps left (enables mid air jumps)
	public bool canDrop = false;//can go through platform
	public bool isJumping = false;

	/*controller inputs and states
    Might map all buttons here instead of in update, tbd
   */
	public bool jump_button;
	public bool jump_button_up;
	public bool drop_button = false;
	public bool pressed = false;//is a button being held down?
	public float h;//x axis
	public float v;//y axis
	
	//components
	Animator anim;
	CharacterController controller;//get the charactercontroller
	//BoxCollider body_base;
	public int layer = 9;//do not allow active characters to have the same layer
	
	//variables
	public Transform my_transform;
	public int delay;//stop recieving controls for x frames
	public int jumpFrame = 0;
	public int maxJumpFrames = 5;
	public float currSpeed;//current speed w/ modifier
	public Vector3 moveVect = Vector3.zero;



	void Awake()
	{
		jumps = 0;//initializes jump
		//body_base = GetComponent<BoxCollider>();
		anim = GetComponent <Animator> ();
		moveVect = new Vector3(0, 0, 0);//initialize the move vector
		controller = GetComponent<CharacterController>();
		my_transform = transform;

		delay = 8;

		if(my_transform.rotation.eulerAngles.y > 90)
			facing = RIGHT;
		else
			facing = LEFT;
	}

	void Update(){
		/*controls character movement*/
		isGrounded = controller.isGrounded;

		if (isGrounded)
			moveVect.y = GRAVITY_DEF;

		if(my_transform.position.z != 0){
			my_transform.position = new Vector3(my_transform.position.x,my_transform.position.y, 0); 
		}//STAY ON THE X AXIS DANGIT

		//get controls
		h = Input.GetAxisRaw ("Horizontal");//raw x axis
		v = Input.GetAxisRaw ("Vertical");//raw y axis
		jump_button = (Input.GetButton("Jump") || Input.GetKey("space"));
		jump_button_up = (Input.GetButtonUp("Jump") || Input.GetKeyUp("space"));

		if(delay == 0){

			if(!isGrounded){
				Move(airMod);
			}//movement in air
			else{
				if(!isCrouching)
				  Move();
			}//regular horizontal movement

			if(jump_button && (isGrounded ||  jumps > 0) && !pressed && !isCrouching){
				pressed = true;//need to modify this soon for short jump
				isJumping = true;
				Jump();
			}
			else{
		  		Gravity(!isGrounded);
			}//disable gravity while jumping

			if(jump_button_up){
				--jumps;
				pressed = false;//you don't say? D:
			} //button is no longer down

		}//is not delayed
		else{
			Gravity(!isGrounded);

			if(h == 0){
				moveVect.x = 0;
			}//the only thing keeping us off the ice
		}//run these regardless

		crouch();//might need to change the order
		if (isCrouching) {
			//dropDown();
			moveVect.x = 0;
		}
		dropDown ();

		//everything that runs regardless
		if(delay > 0){
			--delay;
		}//cooldown of

		if(isGrounded){
			jumps = maxJumps;
		}//on landing add lag

		Animating();
		isJumping = false;

		if(moveVect.x > walkSpeed || moveVect.x < -walkSpeed)
			foreach(int other in playerLayers){
			Physics.IgnoreLayerCollision(layer, other);
			}
		else
		    foreach(int other in playerLayers){
			Physics.IgnoreLayerCollision(layer, other, false);
		    }

		controller.Move(moveVect * Time.deltaTime);//move
	}//update


	void OnTriggerEnter(Collider other){
		if(other.GetComponent<Collider>().name == "Floor" 
			|| other.GetComponent<Collider>().name == "Platform"){
			delay = landFrames;

		}//if landing, delay movement

		if(other.GetComponent<Collider>().name == "Edge")
		{
			moveVect = Vector3.zero;
			controller.transform.position = new Vector3(2, 6, 0);
		}

		//Debug.Log(other.name);
	}//OnTriggerEnter

	void OnTriggerStay(Collider other){
		if (other.GetComponentInParent<Entity>()) {
			other.GetComponentInParent<Entity>().SendMessage("Pushed", moveVect);
		}
	}

	/*General Action Functions*/


	void Jump(float modifier = 1f){
		moveVect.y = (jumpSpeed * modifier); //* Time.deltaTime;
	}//apply jump



	void Gravity(bool on = true, float modifier = 1f){
		if (on)
			moveVect.y -= gravity * Time.deltaTime * modifier;
	}//apply gravity



	void Move(float speed_mod = 1f){

		if(h != 0){
			if(h > 0){
				facing = RIGHT;
				currSpeed = 1;
				my_transform.rotation = Quaternion.LookRotation(Vector3.right);
			}//tilted to right
		
			if(h < 0){
				facing = LEFT;
				currSpeed = -1;
				my_transform.rotation = Quaternion.LookRotation(Vector3.left);
			}//tilted to left

			if(h >= THRESHOLD || h <= -THRESHOLD){
					currSpeed *= speed * speed_mod;
			}//run speed
			else{
				currSpeed *= walkSpeed * speed_mod;	
			}//walk speed

			moveVect.x = currSpeed;
		}//joystick is tilted
		else{
			moveVect.x = 0;
		}//no tilt

	}//horizontal movement, note rotations are only ascetic



	void dropDown()
	{
		if(v <= -THRESHOLD /*&& canDrop*/){
			Physics.IgnoreLayerCollision(layer, PLATFORMS, true);
			//delay = 0;
		}//note that this shuts down all collision detection for anything in the platform layer
		else{
			Physics.IgnoreLayerCollision(layer, PLATFORMS, false);
		}//dropDown, drop off a platform
	}//dropDown




	void crouch(){
		if(isGrounded && v < 0 || Input.GetButton("Crouch")){
			isCrouching = true;
		}
		else{
			isCrouching = false;
		}
	}//what it says on the name


	public virtual void Pushed(Vector3 forces){
		moveVect.x = forces.x/10;//10 is a placeholder
	}



	void Animating(){
		anim.SetBool ("IsWalking", h != 0 && (h < THRESHOLD || h > -THRESHOLD) && !isCrouching);
		anim.SetBool ("IsRunning", h != 0 && h >= THRESHOLD || h <= -THRESHOLD && !isCrouching);
		anim.SetBool ("IsGrounded", isGrounded);
		anim.SetBool ("IsJumping", isJumping);
		anim.SetBool ("IsCrouching", isCrouching && isGrounded);
		anim.SetBool ("IsFalling", !isGrounded);
		anim.SetBool ("IsJumping2", jumps <= 1);
	}//animations
}