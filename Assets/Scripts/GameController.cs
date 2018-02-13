using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameController : MonoBehaviour {

	public bool LevelCompleted;
	public bool LevelFailed;
	public string LevelFailedHint;

	public List<RotatableController> Gears;
	private int GearCounter;

	private List<RotatableController> Sinks;
	private List<ElectrodeController> Electrodes;

	// Use this for initialization
	void Start() {
		// Find all gears (if any)
		Gears = new List<RotatableController>();
		foreach(GameObject go in GameObject.FindGameObjectsWithTag(GearController.TAG_GEAR)) {
			Gears.Add(go.GetComponent<RotatableController>());
		}
		GearCounter = Gears.Count;
		// Find all force sinks
		Sinks = new List<RotatableController>();
		foreach(GameObject go in GameObject.FindGameObjectsWithTag("Finish")) {
			Sinks.Add(go.GetComponent<RotatableController>());
		}
		// Find all force sinks
		Electrodes = new List<ElectrodeController>();
		foreach(GameObject go in GameObject.FindGameObjectsWithTag(ElectrodeController.TAG_ELECTRODE)) {
			Electrodes.Add(go.GetComponent<ElectrodeController>());
		}
	}

	void FixedUpdate() {
		// Check victory and loss conditions
		LevelCompleted |= CheckVictoryConditions();
		LevelFailed |= !LevelCompleted && CheckLossConditions();
	}

	private bool CheckVictoryConditions() {
		// Loop through all force sinks and see if they have turned enough times
		bool result = true;
		foreach(RotatableController fs in Sinks) {
			result &= fs.HasSinkFullyTurnedTheRightWay;
		}
		// Loop through all electrodes and find if all are powered up
		foreach(ElectrodeController electr in Electrodes) {
			result &= electr.HasPower;
		}
		return result;
	}

	private bool CheckLossConditions() {
		// Loop through all force sinks and see if they have turned enough times
		bool wrongTurn = false;
		foreach(RotatableController fs in Sinks) {
			wrongTurn |= fs.HasSinkFullyTurnedTheWrongWay;
		}
		if(wrongTurn) {
			LevelFailedHint = "A blue axle has turned the wrong way!";
		}
		bool powerDrain = false;
		foreach(ElectrodeController electr in Electrodes) {
			powerDrain |= electr.PowerDrain;
		}
		if(powerDrain) {
			LevelFailedHint = "Both electrodes must glow when they touch!";
		}
		return wrongTurn || powerDrain;
	}

	public void RemoveGear(GameObject gameObj) {
		if(gameObj.CompareTag(GearController.TAG_GEAR)) {
			RotatableController gearRot = gameObj.GetComponent<RotatableController>();
			Gears.Remove(gearRot);
			Destroy(gameObj);
		}
	}

	public GameObject SpawnGear(GameObject prefab, Vector2 spawnPos) {
		Debug.Assert(prefab.CompareTag(GearController.TAG_GEAR));
		// Spawn from prefab
		GameObject newGear = Instantiate(prefab) as GameObject;
		newGear.transform.position = spawnPos;
		newGear.name = prefab.name + "_" + GearCounter;
		newGear.GetComponent<SpriteRenderer>().sortingOrder = GearCounter;
		newGear.transform.SetParent(transform);
		// Add newly spawned prefab to list
		Gears.Add(newGear.GetComponent<RotatableController>());
		GearCounter += 1;
		return newGear;
	}
}
