using UnityEngine;
using System.Collections;

public class Combat : MonoBehaviour {


	public bool atkButton;
	public Rigidbody hitbox;
	Rigidbody player;
	// Use this for initialization

	void Awake(){
		hitbox = GetComponent<Rigidbody>();
    }
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		//atkButton = Input.GetButton ("Fire1");
	}
}
