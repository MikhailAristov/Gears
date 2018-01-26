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

	public RotatableController TorqueFrom;

	// Use this for initialization
	void Start() {
		TargetRotation = transform.localRotation;
	}
	
	// Update is called once per frame
	void Update() {
		transform.rotation = Quaternion.Lerp(transform.localRotation, TargetRotation, Time.deltaTime * 10);
	}

	void FixedUpdate() {
		// Reset torque if necessary
		if(!HasTorque()) {
			SetTorquer(null);
		}
		// Update rotation according to speed
		if(Mathf.Abs(RotationSpeed) > 0) {
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

	// The force emitter has torque, as do all rotators directly connected to it
	public bool HasTorque() {
		return (CompareTag("Respawn") || (TorqueFrom != null && TorqueFrom.HasTorque()));
	}

	public void SetTorquer(RotatableController t) {
		Debug.AssertFormat(t == null || !HasTorque(), gameObject.name);
		TorqueFrom = t;
		if(t == null) {
			RotationSpeed = 0;
		} else if(t.CompareTag("Respawn") || CompareTag("Finish")) {
			// Force emitter and sink have direct speed transmission
			RotationSpeed = t.RotationSpeed;
		} else {
			// Gears transmit in reverse
			RotationSpeed = -t.RotationSpeed * t.Radius / Radius;
		}
	}
}
