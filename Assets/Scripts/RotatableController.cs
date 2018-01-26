using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotatableController : MonoBehaviour {

	public float Radius;
	//public float Force;
	//public float Torque;

	// 1.0 = 20 rps
	public float RotationSpeed;
	private float TargetRotationZ;
	private Quaternion TargetRotation;

	// Use this for initialization
	void Start() {
		TargetRotation = transform.localRotation;
	}
	
	// Update is called once per frame
	void Update() {
		transform.rotation = Quaternion.Lerp(transform.localRotation, TargetRotation, Time.deltaTime * 10);
	}

	void FixedUpdate() {
		//Torque = Radius * Force;
		TargetRotationZ -= 0.05f * RotationSpeed / Time.fixedDeltaTime;
		while(TargetRotationZ > 180f) {
			TargetRotationZ -= 360f;
		}
		while(TargetRotationZ < -180f) {
			TargetRotationZ += 360f;
		}
		TargetRotation = Quaternion.Euler(0, 0, TargetRotationZ);
	}
}
