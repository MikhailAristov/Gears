using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GearController : MonoBehaviour, IClickable {

	public const string GEAR_TAG = "Gear";

	const float EMITTER_TOLERANCE = 0.05f;
	const float SINK_TOLERANCE = EMITTER_TOLERANCE;
	const float NEIGHBOUR_GEAR_TOLERANCE = 0.2f;
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
	
	// Update is called once per frame
	void Update() {
		
	}

	void FixedUpdate() {
		// Go through current neighbours and see if any of them have torque
		RotatableController myNewTorquer = null;
		foreach(GameObject neighbor in Neighbours) {
			RotatableController neighborRot = neighbor.GetComponent<RotatableController>();
			if(neighborRot != null) {
				// First check if we are at the force emitter
				if(neighbor.CompareTag("Respawn")) {
					if(Vector2.Distance(neighbor.transform.position, transform.position) < EMITTER_TOLERANCE) {
						myNewTorquer = neighborRot;
						break;
					}
				} else if(neighbor.CompareTag("Finish")) {
					// If it's a force sink, rotate it if close
					if(Mathf.Abs(MyRotator.RotationSpeed) > 0 && neighborRot.TorqueFrom != MyRotator &&
						Vector2.Distance(neighborRot.transform.position, transform.position) < SINK_TOLERANCE)  {
						neighborRot.SetTorquer(MyRotator);
					}
				} else if(neighbor.CompareTag(GEAR_TAG)) {		
					// If it's a gear, jam if the neighbor is too close
					float maxDistanceBeforeJam = MyRotator.Radius + neighborRot.Radius - NEIGHBOUR_GEAR_TOLERANCE;
					if(Vector2.Distance(neighbor.transform.position, transform.position) < maxDistanceBeforeJam) {
						// JAM!
						//Debug.LogWarningFormat("Jam!");
					} else if(neighborRot.HasTorque() && neighborRot.TorqueFrom != MyRotator) {
						if(myNewTorquer == null) {
							myNewTorquer = neighborRot;
							break; // TODO remove for jams...
						} else if(Mathf.Abs(myNewTorquer.RotationSpeed - neighborRot.RotationSpeed) > SPEED_DIFF_TOLERANCE) {
							// JAM if two nearby torquers have a large speed difference!
						}
					}
				}	
			} else {
				// Okay, three, two, one... let's JAM!
			}
		}
		// If a suitable torquer has been found, adopt its rotation speed
		if(myNewTorquer != MyRotator.TorqueFrom) {
			MyRotator.SetTorquer(myNewTorquer != null ? myNewTorquer : null);
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
