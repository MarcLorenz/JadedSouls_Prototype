using UnityEngine;
using System.Collections;

public class Entity : MonoBehaviour {


	const int RIGHT = 1;//right
	const int LEFT = -1;//left
	const int PLATFORMS = 8;//platform layer
	public float THRESHOLD = 0.85f;//controller sensitivity essentially
	public int CHANGE_DIR = 20;

	public float fallSpeed = 10f;//modified fall speed
	public float gravity = 20f;//225F;//20 * 9.8;
	public bool isGrounded = false;//is on ground

	public int facing;//right = 1, left = -1

	//components
	Animator anim;
	CharacterController controller;//get the charactercontroller
	BoxCollider body_base;
	public int layer = 0;//do not allow active characters to have the same layer

	//variables
	public int delay;//stop recieving controls for x frames

	public float currSpeed;//current speed w/ modifier
	public Vector3 moveVect = Vector3.zero;




	// Use this for initialization
	
	void Awake()
	{
		body_base = GetComponent<BoxCollider>();
		anim = GetComponent <Animator> ();
		moveVect = new Vector3(0, 0, 0);//initialize the move vector
		controller = GetComponent<CharacterController>();
		
		delay = 8;
		
		if(transform.rotation.eulerAngles.y > 90)
			facing = RIGHT;
		else
			facing = LEFT;
	}

	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {

		/*controls character movement*/
		isGrounded = controller.isGrounded;

		if(transform.position.z != 0){
			transform.position = new Vector3(transform.position.x,transform.position.y, 0); 
		}//STAY ON THE X AXIS DANGIT

		Animating();
		controller.Move(moveVect * Time.deltaTime);//move
	}

	void Animating(){
	}//animations
}
