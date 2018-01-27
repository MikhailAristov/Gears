using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class UIController : MonoBehaviour {

	public GameController Game;

	// The 0th element is always ignored...
	public GameObject[] GearPrefabs;

	// 0th element is called via F1, 1st via F2, and so on...
	public string[] LevelNames;
	private Dictionary<KeyCode, int> FKeyToLevelMapping = new Dictionary<KeyCode, int>() {
		{KeyCode.F1, 0},
		{KeyCode.F2, 1},
		{KeyCode.F3, 2},
		{KeyCode.F4, 3},
		{KeyCode.F5, 4},
		{KeyCode.F6, 5},
		{KeyCode.F7, 6},
		{KeyCode.F8, 7},
		{KeyCode.F9, 8},
		{KeyCode.F10, 9},
		{KeyCode.F11, 10},
		{KeyCode.F12, 11}
	};

	public GameObject CurrentlyCarried;
	
	// Update is called once per frame
	void Update() {
		if(Input.anyKeyDown) {
			CheckControlButtons();
		}

		// Check if a number has been entered via numpad
		if(Input.inputString != "") {
			int numericKey = 0;
			int.TryParse(Input.inputString, out numericKey);
			if(numericKey > 0 && numericKey < GearPrefabs.Length) {
				SpawnGearAtMouse(GearPrefabs[numericKey]);
			}
		}

		// Manipulate the currently carried gear
		if(CurrentlyCarried != null) {
			if(Input.GetKeyUp(KeyCode.Mouse0)) {
				// Simply release the gear...
				EmptyHand();
			} else if(Input.GetKeyUp(KeyCode.Mouse1)) {
				DestroyCurrentlyCarriedGear();
			}
		} else if(Input.GetKeyDown(KeyCode.Mouse0) || Input.GetKeyUp(KeyCode.Mouse1)) {
			GameObject clickTarget = RaycastCheck<CarriableController>(Input.mousePosition);
			if(clickTarget != null) {
				if(Input.GetKeyUp(KeyCode.Mouse1)) {
					DestroyGear(clickTarget);
				} else {
					PickUpGear(clickTarget);
				}
			}
		}
	}

	private void CheckControlButtons() {
		// Quit on Escape
		if(Input.GetKeyDown(KeyCode.Escape)) {
			QuitGame();
		}
		// Otherwise check for level select
		foreach(KeyCode k in FKeyToLevelMapping.Keys) {
			if(Input.GetKeyDown(k)) {
				int LevelIndex = FKeyToLevelMapping[k];
				if(LevelIndex < LevelNames.Length) {
					SwitchToScene(LevelNames[LevelIndex]);
					break;
				}
			}
		}
	}

	public static Vector2 GetMousePositionInWorldCoordinates() {
		Vector3 mousePos = Input.mousePosition;
		return Camera.main.ScreenToWorldPoint(mousePos);
	}

	private void SpawnGearAtMouse(GameObject gearPrefab) {
		// Destroy the currently carried gear, if any
		DestroyCurrentlyCarriedGear();
		// Spawn the selected gear at the current position of the mouse
		PickUpGear(Game.SpawnGear(gearPrefab, GetMousePositionInWorldCoordinates()));
	}

	private void DestroyGear(GameObject gear) {
		Game.RemoveGear(gear);
	}

	private void DestroyCurrentlyCarriedGear() {
		if(CurrentlyCarried != null) {
			DestroyGear(CurrentlyCarried.gameObject);
			EmptyHand();
		}
	}

	private void PickUpGear(GameObject gear) {
		Debug.Assert(gear != null && gear.CompareTag(GearController.TAG_GEAR));
		CurrentlyCarried = gear;
		CurrentlyCarried.GetComponent<CarriableController>().Grab();
		Cursor.visible = false;
	}

	private void EmptyHand() {
		if(CurrentlyCarried != null) {
			CurrentlyCarried.GetComponent<CarriableController>().Release();
			CurrentlyCarried = null;
		}
		Cursor.visible = true;
	}

	// Projects a ray from the given position to a clickable element
	// and sets its values accordingly into the input variables
	private GameObject RaycastCheck<T>(Vector3 screenPosition) where T:MonoBehaviour {
		// Get ray and raycast hit
		Ray ray = Camera.main.ScreenPointToRay(screenPosition);
		RaycastHit2D hit = Physics2D.Raycast(ray.origin, ray.direction, Mathf.Infinity);
		// Check if it hits anything
		if(hit) {
			Transform objectHit = hit.transform;
			// Check if it has the necessary component
			T controller = objectHit.GetComponent<T>();
			if(controller != null && controller.isActiveAndEnabled) {
				return objectHit.gameObject;
			}
		}
		return null;
	}

	public void SwitchToScene(string SceneName) {
		SceneManager.LoadScene(SceneName);
	}

	public void QuitGame() {
		Debug.Log("Qutting the game...");
		Application.Quit();
	}
}
