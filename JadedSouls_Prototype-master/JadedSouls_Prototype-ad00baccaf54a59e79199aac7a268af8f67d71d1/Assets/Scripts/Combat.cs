using UnityEngine;
using System.Collections;

public class Combat : MonoBehaviour {


	public bool atkButton;
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		atkButton = Input.GetButton ("Basic_atk");
	}
}
