using UnityEngine;
using System.Collections;
public class PlayerMovement : Entity
{

//presets for character
	public float walkSpeed = 2F;
	public float jumpSpeed = 10f;//force of jump
	public float maxSpeed = 20f;
	public float airMod = 3/5f;//modify speed if in air
	public int maxJumps = 2;//maximum number of jumps
	public int landFrames = 7;//when you land from air freeze

//inherits from entity
	//might or might not fill this out

//general info about state of character
	public bool running = false;
	public bool canDrop = false;
	public int jumps;//jumps left (enables mid air jumps)

/*controller inputs and states
  Might map all buttons here instead of in update, tbd
*/
	public bool pressed;//is a button being held down?

	public float h;//x axis
	public float v;//y axis 

//components
	Animator anim;
	Rigidbody playerRigidbody;//might not actually need this

//misc and variables
	Vector3 movement;
	float speed;//speed modifier
	Vector3 cur;//temp variable to access velocity
	public int cooldown;//for lag frames

//functions

	void Awake(){
		cooldown = 0;//initialize cooldown
		//speed = max_speed;
		jumps = maxJumps;
		pressed = false;//init
		//floorMask = LayerMask.GetMask ("Floor");
		anim = GetComponent <Animator> ();
		playerRigidbody = GetComponent <Rigidbody> ();

		if(transform.rotation.eulerAngles.y > 90)
			facing = -1;
		else
			facing = 1;
	}//Awake



	void Update(){
		if (cooldown == 0) {

			if ((Input.GetButtonDown ("Jump") || Input.GetKey ("space")) 
				&& (isGrounded || jumps > 0) && !pressed) {
				pressed = true;//button is being held down
				Jump ();//what is says on the label -_-
				}//if

		
				if ((Input.GetButtonUp ("Jump") || Input.GetKeyUp ("space"))) {
					pressed = false;//you don't say? D:
				} //button is no longer down

		}
	}//Update


	void FixedUpdate()
	{
		if(cooldown > 0)
			--cooldown;//count down if neededd

		Move (h);
		Animating (h);
		
		if(isGrounded && playerRigidbody.velocity.y <= 0.001){
			jumps = 2;//once character lands, you get two jumps
		}//if
		
		//Debug.Log(playerRigidbody.rotation.eulerAngles.y);
	}//FixedUpdate

/*collision detection
Pretty easy to understand, but I might add more comments soon

*/

	void OnCollisionEnter(Collision collision) {
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


/*general movement//
Move: Horizontal Movement
Jump: Self Explanatory...must I say more?
Animating: BIG SUPRISE!

*/
	void Move (float h)
{
		movement.Set (h, 0, 0);

		if(isGrounded)//max possible speed in/on air/ground
			speed = maxSpeed;
		else
			speed = maxSpeed * airMod;

		/*STUB: apply modifiers here*/
		
		movement = movement * speed * Time.deltaTime;
		playerRigidbody.MovePosition(transform.position + movement);
		
		if (facing < 0 && h > 0 ){
			//cooldown = 2;
			playerRigidbody.rotation = Quaternion.LookRotation(Vector3.right);
			facing = 1;
		}//if facing left

		if (facing > 0 && h < 0){
			//cooldown = 2;
			playerRigidbody.rotation = Quaternion.LookRotation(Vector3.left);
			facing = -1;
		}//if facing right
		
	}//Move
	


	void Jump(){
		moveVect.y = (jumpSpeed);
		//cur = rigidbody.velocity;//gets current velocity
		//cur.y = 0f;//sets the y vel to 0
		//playerRigidbody.velocity = cur;//set current vertical vel to 0
		//playerRigidbody.AddForce(new Vector3(0, jumpPower, 0), ForceMode.Force);
		isGrounded = false;
		--jumps;
	}//Jump
	
	

	void Animating (float h){
		//anim.SetBool ("IsRunning", h != 0 && isGrounded);
	}//Animating
}