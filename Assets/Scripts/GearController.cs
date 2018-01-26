using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GearController : MonoBehaviour {

	const float EMITTER_TOLERANCE = 0.05f;
	const float SINK_TOLERANCE = EMITTER_TOLERANCE;
	const float NEIGHBOUR_GEAR_TOLERANCE = 0.35f;
	const float ROTATION_SPEED_THRESHOLD = 0.1f;

	private float Radius;
	private RotatableController MyRotator;
	private RotatableController Emitter;
	private RotatableController Sink;

	public RotatableController NearestGear;
	public float DistToNearestGear;
	public float DistToInteract;

	// Use this for initialization
	void Start() {
		// Find rotators
		MyRotator = GetComponent<RotatableController>();
		Radius = MyRotator.Radius;
		Emitter = GameObject.FindGameObjectWithTag("Respawn").GetComponent<RotatableController>();
		Sink = GameObject.FindGameObjectWithTag("Finish").GetComponent<RotatableController>();
	}
	
	// Update is called once per frame
	void Update() {
		
	}

	void FixedUpdate() {
		if(Vector2.Distance(Emitter.transform.position, transform.position) < EMITTER_TOLERANCE) {
			MyRotator.RotationSpeed = Emitter.RotationSpeed;
		} else {
			// Reset rotation speed
			MyRotator.RotationSpeed = 0;
			foreach(GameObject gear in GameObject.FindGameObjectsWithTag("Gear")) {
				if(gear == this.gameObject) {
					continue;
				}
				// Check if any of them is close enough
				RotatableController gearRot = gear.GetComponent<RotatableController>();
				DistToInteract = Radius + gearRot.Radius;
				DistToNearestGear = Vector2.Distance(gear.transform.position, transform.position);
				NearestGear = gearRot;
				if(Mathf.Abs(DistToInteract - DistToNearestGear) < NEIGHBOUR_GEAR_TOLERANCE && Mathf.Abs(gearRot.RotationSpeed) > ROTATION_SPEED_THRESHOLD) {
					// If so, adopt its rotation in reverse
					MyRotator.RotationSpeed = -gearRot.RotationSpeed * gearRot.Radius / Radius;
					break;
				}
			}
		}

		// Now, if I am close to the sink, make it rotate, too
		if(Vector2.Distance(Sink.transform.position, transform.position) < SINK_TOLERANCE) {
			Sink.RotationSpeed = MyRotator.RotationSpeed;
		}
	}
}
