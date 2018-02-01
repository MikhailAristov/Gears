using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotatableController : MonoBehaviour {

	const float FULL_REVOLUTION = 360f;

	public float Radius {
		get { return MyCollider.radius * transform.localScale.x; }
	}

	// 1.0 = 20 rps
	public float RotationSpeed;
	private float TargetRotationZ;
	private Quaternion TargetRotation;
	private float _totalRotation;

	public float TotalRotation {
		get { return _totalRotation; }
	}

	public RotatableController TorqueFrom;
	public CircleCollider2D MyCollider;

	// Jamming
	private bool JammedByObstacle, JammedByTooCloseGear, JammedByRotationSpeedDifference, JammedByPropagation;
	private RotatableController JammedByGear;

	public bool IsJammed {
		get { return (JammedByObstacle || JammedByTooCloseGear || JammedByRotationSpeedDifference || JammedByPropagation); }
	}

	public bool IsJammedByMe(RotatableController me) {
		return JammedByPropagation && JammedByGear == me;
	}

	public bool IsJammedBySomethingOtherThanMe(RotatableController me) {
		return JammedByObstacle || JammedByTooCloseGear || JammedByRotationSpeedDifference || (JammedByPropagation && JammedByGear != me);
	}

	// Dirty, but it works...
	public bool IsSinkCounterClockwise {
		get { return true; } 
	}

	// If the "right way" is counter-clockwise, TotalRotation should be positive, otherwise negative
	public bool HasSinkFullyTurnedTheRightWay {
		get { return GetComponent<SpriteRenderer>().flipX ? (TotalRotation >= FULL_REVOLUTION) : (TotalRotation <= -FULL_REVOLUTION); }
	}
	public bool HasSinkFullyTurnedTheWrongWay {
		get { return GetComponent<SpriteRenderer>().flipX ? (TotalRotation < -FULL_REVOLUTION) : (TotalRotation > FULL_REVOLUTION); }
	}

	// Use this for initialization
	void Start() {
		TargetRotation = transform.localRotation;
		TargetRotationZ = transform.localRotation.eulerAngles.z;
	}
	
	// Update is called once per frame
	void Update() {
		transform.rotation = Quaternion.Lerp(transform.localRotation, TargetRotation, Time.deltaTime * 10);
	}

	void FixedUpdate() {
		// Reset torque if necessary
		if(Mathf.Abs(RotationSpeed) > 0 && !HasTorque()) {
			SetTorquer(null);
		}
		// Check for jams
		if(IsJammed) {
			RotationSpeed = 0;
		} else if(RotationSpeed == 0 && HasTorque()) {
			RotationSpeed = GetRotationSpeedTransmittedFrom(TorqueFrom);
		}
		// Update rotation according to speed
		if(Mathf.Abs(RotationSpeed) > 0) {
			float rotationDelta = -0.05f * RotationSpeed / Time.fixedDeltaTime;
			TargetRotationZ += rotationDelta;
			_totalRotation += rotationDelta;
			if(TargetRotationZ > 180f) {
				TargetRotationZ -= 360f;
			}
			if(TargetRotationZ < -180f) {
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
		Debug.AssertFormat(t == null || t != TorqueFrom, gameObject.name);
		TorqueFrom = t;
		RotationSpeed = GetRotationSpeedTransmittedFrom(t);
	}

	private float GetRotationSpeedTransmittedFrom(RotatableController rot) {
		if(rot == null) {
			return 0;
		} else if(rot.CompareTag("Respawn") || CompareTag("Finish")) {
			// Force emitter and sink have direct speed transmission
			return rot.RotationSpeed;
		} else {
			// Gears transmit in reverse
			return -rot.RotationSpeed * rot.Radius / Radius;
		}
	}

	public void JamByObstacle() {
		JammedByObstacle = true;
	}

	public void JamByCollidingGear() {
		JammedByTooCloseGear = true;
	}

	public void JamByRotationalDifference() {
		JammedByRotationSpeedDifference = true;
	}

	public void PropagateJamFrom(RotatableController rot) {
		JammedByPropagation = true;
		JammedByGear = rot;
	}

	public void ResetJammingConditions() {
		JammedByObstacle = false;
		JammedByTooCloseGear = false;
		JammedByRotationSpeedDifference = false;
		JammedByPropagation = false;
		JammedByGear = null;
	}
}
