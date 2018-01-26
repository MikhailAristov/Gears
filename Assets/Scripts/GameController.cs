using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameController : MonoBehaviour {

	const string GEAR_TAG = "Gear";

	public RotatableController ForceEmitter;
	public RotatableController ForceSink;
	public List<RotatableController> Gears;

	// Use this for initialization
	void Start () {
		Gears = new List<RotatableController>();
		foreach(GameObject go in GameObject.FindGameObjectsWithTag(GEAR_TAG)) {
			Gears.Add(go.GetComponent<RotatableController>());
		}
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
