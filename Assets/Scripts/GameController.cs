using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// TODO: Check if still needed?
public class GameController : MonoBehaviour {

	public RotatableController ForceEmitter;
	public RotatableController ForceSink;
	public List<RotatableController> Gears;
	private int GearCounter;

	// Use this for initialization
	void Start () {
		Gears = new List<RotatableController>();
		foreach(GameObject go in GameObject.FindGameObjectsWithTag(GearController.GEAR_TAG)) {
			Gears.Add(go.GetComponent<RotatableController>());
		}
		GearCounter = Gears.Count;
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	public void RemoveGear(GameObject gameObj) {
		if(gameObj.CompareTag(GearController.GEAR_TAG)) {
			RotatableController gearRot = gameObj.GetComponent<RotatableController>();
			Debug.AssertFormat(gearRot != null & Gears.Remove(gearRot), "Could not remove {0}!", gameObj.name);
			Destroy(gameObj);
		}
	}

	public GameObject SpawnGear(GameObject prefab, Vector2 spawnPos) {
		Debug.Assert(prefab.CompareTag(GearController.GEAR_TAG));
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
