using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// TODO: Check if still needed?
public class GameController : MonoBehaviour {
	
	public List<RotatableController> Gears;
	private int GearCounter;

	// Use this for initialization
	void Start () {
		Gears = new List<RotatableController>();
		foreach(GameObject go in GameObject.FindGameObjectsWithTag(GearController.TAG_GEAR)) {
			Gears.Add(go.GetComponent<RotatableController>());
		}
		GearCounter = Gears.Count;
	}

	public void RemoveGear(GameObject gameObj) {
		if(gameObj.CompareTag(GearController.TAG_GEAR)) {
			RotatableController gearRot = gameObj.GetComponent<RotatableController>();
			Debug.AssertFormat(gearRot != null & Gears.Remove(gearRot), "Could not remove {0}!", gameObj.name);
			Destroy(gameObj);
		}
	}

	public GameObject SpawnGear(GameObject prefab, Vector2 spawnPos) {
		Debug.Assert(prefab.CompareTag(GearController.TAG_GEAR));
		// Spawn from prefab
		GameObject newGear = Instantiate(prefab) as GameObject;
		newGear.transform.position = spawnPos;
		newGear.name = prefab.name + GearCounter;
		newGear.GetComponent<SpriteRenderer>().sortingOrder = GearCounter;
		newGear.transform.SetParent(transform);
		// Add newly spawned prefab to list
		Gears.Add(newGear.GetComponent<RotatableController>());
		GearCounter += 1;
		return newGear;
	}
}
