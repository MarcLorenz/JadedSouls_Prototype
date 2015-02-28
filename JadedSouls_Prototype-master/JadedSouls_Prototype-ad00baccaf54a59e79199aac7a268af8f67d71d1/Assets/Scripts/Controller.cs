using UnityEngine;
using System.Collections;

public class Controller : MonoBehaviour{
	//general settings
	  public float THRESHOLD = 0.85f;//controller sensitivity essentially
	/*controller inputs and states
	 */
	public bool pressed;//is a button being held down?
	
	public float h;//x axis
	public float v;//y axis 

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		h = Input.GetAxisRaw ("Horizontal");//raw x axis
		v = Input.GetAxisRaw ("Vertical");//raw y axis
	}
}
