using UnityEngine;
using System.Collections;
public class PlayerMovement : MonoBehaviour
{

//presets for character
	float maxSpeed = 6f;
	float airSpeed = (18/4);
	float dropSpeed = -20f;
	float jumpPower = 22000f;
	int landFrames = 7;
	int maxJump = 2;

//general info about state of character
	public bool running = false;
	public int facing;
	public bool isGrounded = true;
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
	BoxCollider bottom;//horrible name imo but base is taken

//misc and variables
	Vector3 movement;
	float speed;//speed modifier
	int floorMask;//for use with raycast if needed
	Vector3 cur;//temp variable to access velocity
	public int cooldown;//for lag frames

//functions

	void Awake(){
		cooldown = 0;//initialize cooldown
		//speed = max_speed;
		jumps = maxJump;
		pressed = false;//init
		floorMask = LayerMask.GetMask ("Floor");
		bottom = GetComponent<BoxCollider>();
		anim = GetComponent <Animator> ();
		playerRigidbody = GetComponent <Rigidbody> ();

		if(transform.rotation.eulerAngles.y > 90)
			facing = -1;
		else
			facing = 1;
	}//Awake



	void Update(){
		if(cooldown == 0)
		{
			h = Input.GetAxisRaw ("Horizontal");//raw x axis
			v = Input.GetAxisRaw ("Vertical");//raw y axis

			if ((Input.GetButtonDown("Jump") || Input.GetKey("space")) 
				&& (isGrounded ||  jumps > 0) && !pressed){
				pressed = true;//button is being held down
				Jump();//what is says on the label -_-
			}//if

		
			if((Input.GetButtonUp("Jump") || Input.GetKeyUp("space"))){
				pressed = false;//you don't say? D:
			} //button is no longer down

			if(v < 0){
				if(canDrop){
					bottom.isTrigger = true;
					canDrop = false;
				}//drop through platform, near instantaneous

				if(!isGrounded){
					playerRigidbody.AddForce(new Vector3(0, dropSpeed, 0), ForceMode.Impulse);
				}//if not on ground, enables midair speed drop
			}//if down is pressed
		}
		//Debug.DrawRay(playerRigidbody.centerOfMass, Vector3.down, Color.red);
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



	void OnCollisionExit(Collision collisionInfo){
		if(collisionInfo.other.name == "Platform" 
		|| collisionInfo.other.name == "Floor")
		isGrounded = false;
	}//OnCollisionExit


	//only really matters when the collider is disabled
	void OnTriggerExit(Collider other){
		bottom.isTrigger = false;
	}//OnTriggerExit

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
			speed = airSpeed;

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
	


	void Jump()
	{
		cur = rigidbody.velocity;//gets current velocity
		cur.y = 0f;//sets the y vel to 0
		playerRigidbody.velocity = cur;//set current vertical vel to 0
		playerRigidbody.AddForce(new Vector3(0, jumpPower, 0), ForceMode.Force);
		isGrounded = false;
		--jumps;
	}//Jump
	
	

	void Animating (float h)
	{
		anim.SetBool ("IsRunning", h != 0 && isGrounded);
	}//Animating
}