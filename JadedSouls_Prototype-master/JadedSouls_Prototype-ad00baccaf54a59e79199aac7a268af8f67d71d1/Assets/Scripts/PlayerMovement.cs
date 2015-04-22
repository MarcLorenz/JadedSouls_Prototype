using UnityEngine;
using System.Collections;


public class PlayerMovement : Entity
{
	public float THRESHOLD = 0.85f;//controller sensitivity essentially
	public int CHANGE_DIR = 20;
	//int num_controls = 4;
	//string[] controls = new string[] {"Horizontal", "Vertical", "Jump", "Crouch"};
	int num_controls = 6;
	string[] controls = new string[] {"Horizontal", "Vertical", "Jump", "Crouch", "basic_atk", "second_atk"};
	public int player = 1;//this is appended to controls
	int[] playerLayers = {9, 10, 11, 12};
	string hitbox = "Player";

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
	public bool crouch_button;

//components
	//Anim
	Rigidbody playerRigidbody;//might not actually need this

//misc and variables
	//Vector3 movement;
	//float speed;//speed modifier
	//Vector3 cur;//temp variable to access velocity
	public int cooldown;//for lag frame

//TEMP FOR DEMO///
	bool atk1;
	bool atk2;
	bool isHit;
	bool weak_atk = false;
	bool heavy_atk = false;
//END

//functions

	void Awake(){
		for(int x = 0; x < num_controls; x++)
			controls[x] += player;
		base.Awake ();
		FRICTION = 1.2f;
		//isStatic = false;
		isCrouching = false;
		cooldown = 0;//initialize cooldown
		//speed = max_speed;
		jumps = maxJumps;
		pressed = false;//init
		//floorMask = LayerMask.GetMask ("Floor");
		anim = GetComponent <Animator> ();
		playerRigidbody = GetComponent <Rigidbody> ();
		//IgnoreCollision (bottom, floor);
		hitbox += player;
	}//Awake



	void Update(){

		weak_atk = false;
		heavy_atk = false;
		h = Input.GetAxisRaw (controls[0]);//raw x axis
		v = Input.GetAxisRaw (controls[1]);//raw y axis
		jump_button = (Input.GetButton(controls[2]));// || Input.GetKey("space"));
		jump_button_up = (Input.GetButtonUp(controls[2]));// || Input.GetKeyUp("space"));
		crouch_button = Input.GetButton(controls[3]);
		//TEMP FOR DEMO
		atk1 = Input.GetButtonDown(controls[4]);
		atk2 = Input.GetButtonDown(controls[5]);
		//TEMP FOR DEMO
		
		isGravity = true;//gravity is on by default
		isJumping = false;

			if(!isGrounded){
				Move(airMod);
			}//movement in air
			else{
				if(!isCrouching)
					Move();
			}//regular horizontal movement

			if (jump_button && (isGrounded || jumps > 0) && !pressed && delay < 1) {
				delay = 8;
				isJumping = true;
				pressed = true;//button is being held down
				isGravity = false;
				Jump ();//what is says on the label -_-
			}//if

		    anim.SetBool ("IsJumping", isJumping);

			if (jump_button_up){
				--jumps;
				pressed = false;//you don't say? D:
			} //button is no longer down
		
			crouch();//might need to change the order
			dropDown();

			if (isCrouching) {
				moveVect.x = 0;
				
			}

			attack();


		/*if (moveVect.x > walkSpeed || moveVect.x < -walkSpeed) {
			foreach (int other in playerLayers) {
				Physics.IgnoreLayerCollision (layer, other);
			}
		} else {
			isPushing = false;
			foreach (int other in playerLayers) {
				Physics.IgnoreLayerCollision (layer, other, false);
			}
		}*/
		
		if(h != 0)
			isStatic = false;
		else
			isStatic = true;

		if(moveVect.y > 0)
			Physics.IgnoreLayerCollision(layer, PLATFORMS, true);
		base.Update();

	}//Update


	void FixedUpdate()
	{
		if(delay > 0)
			--delay;//count down if neededd

		//Move (h);
		Animating (h);
		
		if(isGrounded && playerRigidbody.velocity.y <= 0.001){
			jumps = 2;//once character lands, you get two jumps
		}
	}//FixedUpdate

/*collision detection
Pretty easy to understand, but I might add more comments soon

*/
	public void OnTriggerEnter(Collider other){
		if(other.GetComponent<Collider>().name == "Floor" 
		   || other.GetComponent<Collider>().name == "Platform"){
			//Physics.IgnoreLayerCollision(layer, PLATFORMS, false);
			//delay = 5;
		}
//TEMP
		if(other.name != hitbox)
			if(atk1 || atk2)
				other.GetComponentInParent<Entity>().SendMessage("knockback", facing);
				 
//
		base.OnTriggerEnter(other);
	}

	public void OnTriggerStay(Collider other){
		if (other.GetComponentInParent<Entity>()) {
			other.GetComponentInParent<Entity>().SendMessage("Pushed", moveVect);
			isPushing = true;
		}

		if(other.name != hitbox)
			if(atk1 || atk2)
				other.GetComponentInParent<Entity>().SendMessage("knockback", facing);
		
		if(other.GetComponent<Collider>().name == "Floor"){
		  //|| other.GetComponent<Collider>().name == "Platform"){
			Physics.IgnoreLayerCollision(layer, PLATFORMS, false);
		}

	}

	public void OnTriggerExit(Collider other){
		if(other.GetComponent<Collider>().name == "Floor" 
		   || other.GetComponent<Collider>().name == "Platform"){
			//Physics.IgnoreLayerCollision(layer, PLATFORMS, false);
		}
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
			//moveVect.x = 0;
			isPushing = false;
		}//no tilt

	}//horizontal movement, note rotations are only ascetic


	void Jump(float modifier = 1f){
		moveVect.y = (jumpSpeed * modifier); //* Time.deltaTime;
	}//apply jump

	void crouch(){
		if(isGrounded && (v < 0 || crouch_button)){
			isCrouching = true;

		}
		else{
			isCrouching = false;
		}
	}//what it says on the name


	void dropDown()
	{
		if(v <= -THRESHOLD){
			Physics.IgnoreLayerCollision(layer, PLATFORMS);
			//delay = 5;
		}//note that this shuts down all collision detection for anything in the platform layer
	}//dropDown

	public virtual void Pushed(Vector3 forces){
		base.Pushed (forces);
	}

	void attack(){
		if(atk2 && isGrounded){
			heavy_atk = true;
			anim.Play("Kick");
		}
		else
		  if(atk1 && isGrounded){
			 weak_atk = true;
			 anim.Play("Punch");
		}
		else {
			if(atk1 || atk2){
				heavy_atk = true;
				anim.Play ("Air_Kick");
			}
		}
	}

	public void knockback(int f){
		    moveVect.x = 120 * f;
			
	}
	
	

	void Animating (float h){
		base.Animating ();
		anim.SetBool ("IsWalking", h != 0 && (h < THRESHOLD || h > -THRESHOLD));// && !isCrouching);
		anim.SetBool ("IsRunning", h != 0 && h >= THRESHOLD || h <= -THRESHOLD);// && !isCrouching);
		anim.SetBool ("IsGrounded", isGrounded);
		anim.SetBool ("IsCrouching", isCrouching && isGrounded);
		anim.SetBool ("IsFalling", !isGrounded);
		anim.SetBool ("IsJumping2", jumps <= 1);
	}//Animating
}