using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GearController : MonoBehaviour {

	const float EMITTER_TOLERANCE = 0.05f;
	const float SINK_TOLERANCE = EMITTER_TOLERANCE;
	const float NEIGHBOUR_GEAR_TOLERANCE = 0.4f;

	private GameController Game;
	private RotatableController MyRotator;
	private RotatableController Emitter;
	private RotatableController Sink;

	public RotatableController NearestGear;
	public float DistToNearestGear;
	public float DistToInteract;

	// Use this for initialization
	void Start() {
		// Get my rotator and radius
		MyRotator = GetComponent<RotatableController>();
		// Find game controller and otherrotators
		Game = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameController>();
		Emitter = Game.ForceEmitter;
		Sink = Game.ForceSink;
	}
	
	// Update is called once per frame
	void Update() {
		
	}

	void FixedUpdate() {
		// First check if my previous torquer is still in range
		if(MyRotator.TorqueFrom != null) {
			CheckCurrentTorquer();
		}

		// Now, if torquer is still or just now null, look for a new one
		if(MyRotator.TorqueFrom == null) {
			LookForNewTorquer();
		}

		// Finally, if I am close to the sink, make it rotate, too
		if(Mathf.Abs(MyRotator.RotationSpeed) > 0 && Sink.TorqueFrom != MyRotator)  {
			CheckForceSinkProximity();
		}
	}

	private void CheckCurrentTorquer() {
		float tolerance = (MyRotator.TorqueFrom == Emitter) ? EMITTER_TOLERANCE : NEIGHBOUR_GEAR_TOLERANCE;
		if(Vector2.Distance(MyRotator.TorqueFrom.transform.position, transform.position) > tolerance) {
			MyRotator.SetTorquer(null);
		}
	}

	private void LookForNewTorquer() {
		// First, check if we are close enough to the emitter
		if(Vector2.Distance(Emitter.transform.position, transform.position) < EMITTER_TOLERANCE) {
			MyRotator.SetTorquer(Emitter);
		} else {
			// Look for the closest gear with torque
			RotatableController closestGearWithTorque = null;
			float closestGearDistance = float.MaxValue;
			foreach(RotatableController gear in Game.Gears) {
				if(gear.HasTorque()) {
					float distanceToGear = Vector2.Distance(gear.transform.position, transform.position);
					float interactionDistance = MyRotator.Radius + gear.Radius;
					if(distanceToGear < closestGearDistance &&
						Mathf.Abs(distanceToGear - interactionDistance) < NEIGHBOUR_GEAR_TOLERANCE) {
						closestGearWithTorque = gear;
					}
				}
			}
			// If a close by gear with torque has been found, set it as my new torquer
			if(closestGearWithTorque != null) {
				MyRotator.SetTorquer(closestGearWithTorque);
			}
		}
	}

	private void CheckForceSinkProximity() {
		if(Vector2.Distance(Sink.transform.position, transform.position) < SINK_TOLERANCE) {
			Sink.SetTorquer(MyRotator);
		}
	}
}
