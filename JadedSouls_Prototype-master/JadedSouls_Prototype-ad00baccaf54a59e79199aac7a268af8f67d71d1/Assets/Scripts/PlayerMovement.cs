using UnityEngine;
using System.Collections;
public class PlayerMovement : Entity
{
	public float THRESHOLD = 0.85f;//controller sensitivity essentially
	public int CHANGE_DIR = 20;
	int[] playerLayers = {9, 10, 11, 12};

	//presets for character
	public float speed = 8F;//ground movement speed
	public float walkSpeed = 2F;
	public float jumpSpeed = 10f;//force of jump
	public float maxSpeed = 20f;
	public float airMod = 3/5f;//modify speed if in air
	public int maxJumps = 2;//maximum number of jumps
	public int landFrames = 7;//when you land from air free

//inherits from entity
	//might or might not fill this out

//general info about state of character
	public bool isJumping = false;
	public bool running = false;
	public bool canDrop = false;
	public int jumps;//jumps left (enables mid air jumps)
	public bool isCrouching = false;//is crouching
	public bool isPushing = false;

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
	//Anim
	Rigidbody playerRigidbody;//might not actually need this

//misc and variables
	//Vector3 movement;
	//float speed;//speed modifier
	//Vector3 cur;//temp variable to access velocity
	public int cooldown;//for lag frame

//functions

	void Awake(){
		base.Awake ();
		isCrouching = false;
		cooldown = 0;//initialize cooldown
		//speed = max_speed;
		jumps = maxJumps;
		pressed = false;//init
		//floorMask = LayerMask.GetMask ("Floor");
		anim = GetComponent <Animator> ();
		playerRigidbody = GetComponent <Rigidbody> ();
	}//Awake



	void Update(){

		h = Input.GetAxisRaw ("Horizontal2");//raw x axis
		v = Input.GetAxisRaw ("Vertical2");//raw y axis
		jump_button = (Input.GetButton("Jump2"));// || Input.GetKey("space"));
		jump_button_up = (Input.GetButtonUp("Jump2"));// || Input.GetKeyUp("space"));

		isGravity = true;//gravity is on by default

		if(!isGrounded){
			Move(airMod);
		}//movement in air
		else{
			if(!isCrouching)
				Move();
		}//regular horizontal movement

		if ((Input.GetButtonDown ("Jump2"))// || Input.GetKey ("space")) 
			&& (isGrounded || jumps > 0) && !pressed) {
			pressed = true;//button is being held down
			isGravity = false;
			Jump ();//what is says on the label -_-
		}//if

		if (Input.GetButtonUp ("Jump2")){// || Input.GetKeyUp ("space"))) {
			--jumps;
			pressed = false;//you don't say? D:
		} //button is no longer down
		
		crouch();//might need to change the order
		if (isCrouching) {
			//dropDown();
			moveVect.x = 0;
		}

		dropDown ();

		if(moveVect.x > walkSpeed || moveVect.x < -walkSpeed)
		foreach(int other in playerLayers){
			Physics.IgnoreLayerCollision(layer, other);
		}
		else
		foreach(int other in playerLayers){
			Physics.IgnoreLayerCollision(layer, other, false);
		}

		base.Update();
	}//Update


	void FixedUpdate()
	{
		if(cooldown > 0)
			--cooldown;//count down if neededd

		//Move (h);
		Animating (h);
		
		if(isGrounded && playerRigidbody.velocity.y <= 0.001){
			jumps = 2;//once character lands, you get two jumps

		}//if
		
		//Debug.Log(playerRigidbody.rotation.eulerAngles.y);
	}//FixedUpdate

/*collision detection
Pretty easy to understand, but I might add more comments soon

*/

	/*void OnCollisionEnter(Collision collision) {
		base.OnCollisionEnter(collision);
		foreach (ContactPoint contact in collision.contacts) {
			if(contact.otherCollider.name == "Platform"){
				canDrop = true;//are you on a platform?
			}
			else {
				canDrop = false;
			//Debug.DrawRay(contact.point, contact.normal, Color.white);
		    }

		}//foreach contact point
		cooldown = landFrames;//landing lag NOTE: ANY collision will cause this...
		isGrounded = true;
	}//OnCollisionEnter
*/
	void OnTriggerStay(Collider other){
		if (other.GetComponentInParent<Entity>()) {
			other.GetComponentInParent<Entity>().SendMessage("Pushed", moveVect);
		}
		isPushing = true;
	}


/*general movement//
Move: Horizontal Movement
Jump: Self Explanatory...must I say more?
Animating: BIG SUPRISE!

*/
	void Move (float speed_mod = 1f)
{
		if(h != 0){
			if(h > 0){
				facing = RIGHT;
				currSpeed = 1;
				transform.rotation = Quaternion.LookRotation(Vector3.right);
			}//tilted to right
		
			if(h < 0){
				facing = LEFT;
				currSpeed = -1;
				transform.rotation = Quaternion.LookRotation(Vector3.left);
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
			isPushing = false;
		}//no tilt

	}//horizontal movement, note rotations are only ascetic


	void Jump(float modifier = 1f){
		moveVect.y = (jumpSpeed * modifier); //* Time.deltaTime;
	}//apply jump

	void crouch(){
		if(isGrounded && (v < 0 || Input.GetButton("Crouch2"))){
			isCrouching = true;
		}
		else{
			isCrouching = false;
		}
	}//what it says on the name

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

	public virtual void Pushed(Vector3 forces){
		if (!isPushing)
						base.Pushed (forces);
	}
	
	

	void Animating (float h){
		base.Animating ();
		anim.SetBool ("IsWalking", h != 0 && (h < THRESHOLD || h > -THRESHOLD));// && !isCrouching);
		anim.SetBool ("IsRunning", h != 0 && h >= THRESHOLD || h <= -THRESHOLD);// && !isCrouching);
		anim.SetBool ("IsGrounded", isGrounded);
		anim.SetBool ("IsJumping", isJumping);
		anim.SetBool ("IsCrouching", isCrouching && isGrounded);
		anim.SetBool ("IsFalling", !isGrounded);
		anim.SetBool ("IsJumping2", jumps <= 1);
	}//Animating
}