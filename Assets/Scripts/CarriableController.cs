using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarriableController : MonoBehaviour {

	public bool IsBeingCarried;
	private Vector2 GrippingPoint;
	
	// Update is called once per frame
	void Update() {
		if(IsBeingCarried) {
			FollowMouse();
		}
	}

	public void Grab() {
		IsBeingCarried = true;
		GrippingPoint = new Vector2(transform.position.x, transform.position.y) - UIController.GetMousePositionInWorldCoordinates();
	}

	public void Release() {
		IsBeingCarried = false;
		GrippingPoint = Vector2.zero;
	}

	private void FollowMouse() {
		transform.position = UIController.GetMousePositionInWorldCoordinates() + GrippingPoint;
	}
}
