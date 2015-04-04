using UnityEngine;
using System.Collections;

public class CameraFollow : MonoBehaviour 

{

	public Transform target, target2;
	public float smoothing = 5f;

	Vector3 offset;

	void Start()
	{
		offset = transform.position - ((target.position + target2.position) / 2);
	}

	void Update(){
	}

	void FixedUpdate()
	{
		Vector3 targetCamPos = (target.position + target2.position)/ 2 + offset;
		transform.position = Vector3.Lerp (transform.position, targetCamPos, smoothing * Time.deltaTime);
	}
}
