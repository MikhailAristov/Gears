using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GearController : MonoBehaviour {

	public const string TAG_GEAR = "Gear";
	public const string TAG_OBSTACLE = "Obstacle";

	const float EMITTER_TOLERANCE = 0.05f;
	const float SINK_TOLERANCE = EMITTER_TOLERANCE;
	const float NEIGHBOUR_GEAR_TOLERANCE = 0.1f;
	const float SPEED_DIFF_TOLERANCE = 0.1f;

	private RotatableController MyRotator;

	// List all current collisions
	private List<GameObject> Neighbours;

	// Use this for initialization
	void Start() {
		// Get my rotator and radius
		MyRotator = GetComponent<RotatableController>();
		Debug.Assert(MyRotator != null);
		// Initialize neighbors
		Neighbours = new List<GameObject>();
	}

	void FixedUpdate() {
		MyRotator.ResetJammingConditions();
		// Go through current neighbours and see if any of them have torque
		RotatableController myNewTorquer = null, myNeighbourSink = null;
		foreach(GameObject neighbor in Neighbours) {
			RotatableController neighborRot = neighbor.GetComponent<RotatableController>();
			if(neighborRot != null) {
				// First check if we are at the force emitter
				if(neighbor.CompareTag("Respawn")) {
					if(Vector2.Distance(neighbor.transform.position, transform.position) < EMITTER_TOLERANCE) {
						myNewTorquer = neighborRot;
					}
				} else if(neighbor.CompareTag("Finish")) {
					// If it's a force sink, rotate it if close
					if(Vector2.Distance(neighborRot.transform.position, transform.position) < SINK_TOLERANCE)  {
						myNeighbourSink = neighborRot;
					}
				} else if(neighbor.CompareTag(TAG_GEAR)) {
					// Check if this gear is jammed by someone else
					if(neighborRot.IsJammedBySomethingOtherThanMe(MyRotator)) {
						MyRotator.PropagateJamFrom(neighborRot);
					}
					// If it's a gear, jam if the neighbor is too close
					float maxDistanceBeforeJam = MyRotator.Radius + neighborRot.Radius - NEIGHBOUR_GEAR_TOLERANCE;
					if(Vector2.Distance(neighbor.transform.position, transform.position) < maxDistanceBeforeJam) {
						MyRotator.JamByCollidingGear();
					} else if(neighborRot.HasTorque() && neighborRot.TorqueFrom != MyRotator) {
						if(myNewTorquer == null) {
							myNewTorquer = neighborRot;
						} else if(Mathf.Abs(myNewTorquer.RotationSpeed - neighborRot.RotationSpeed) > SPEED_DIFF_TOLERANCE) {
							// JAM if two nearby torquers have a large speed difference!
							MyRotator.JamByRotationalDifference();
						}
					}
				}	
			} else if(!neighbor.CompareTag(ElectrodeController.TAG_ELECTRODE)) {
				// Electrodes don't interfere with gears...
				MyRotator.JamByObstacle();
			}
		}
		// If a suitable torquer has been found, adopt its rotation speed
		if(myNewTorquer != MyRotator.TorqueFrom) {
			MyRotator.SetTorquer(myNewTorquer != null ? myNewTorquer : null);
		}
		// If I am in contact with a sink, propagate either my speed or my jam to it
		if(myNeighbourSink != null) {
			// Check for jams
			if(MyRotator.IsJammed) {
				myNeighbourSink.PropagateJamFrom(MyRotator);
			} else if(myNeighbourSink.IsJammedByMe(MyRotator)) {
				myNeighbourSink.ResetJammingConditions();
			}
			// Check whether my speed can be propagated
			if(!MyRotator.IsJammed && Mathf.Abs(MyRotator.RotationSpeed) > 0 && myNeighbourSink.TorqueFrom != MyRotator) {
				myNeighbourSink.SetTorquer(MyRotator);
			}
		}
	}

	public GameObject GetGameObject() {
		return gameObject;
	}

	void OnCollisionEnter2D(Collision2D coll) {
		if(!Neighbours.Contains(coll.gameObject)) {
			Neighbours.Add(coll.gameObject);
		}
	}

	void OnCollisionExit2D(Collision2D coll) {
		Neighbours.Remove(coll.gameObject);
	}
}
