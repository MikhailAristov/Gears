using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class UIController : MonoBehaviour {

	public GameController Game;

	// The 0th element is always ignored...
	public GameObject[] GearPrefabs;

	public GameObject CurrentlyCarried;

	// Use this for initialization
	void Start() {
		Debug.Assert(Game != null);
	}
	
	// Update is called once per frame
	void Update() {
		// Quit on Escape
		if(Input.GetKeyDown(KeyCode.Escape)) {
			QuitGame();
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
			} else {
				// Move the gear closer to the mouse's current position...
				CurrentlyCarried.transform.position = GetMousePositionInWorldCoordinates();
			}
		} else if(Input.GetKeyDown(KeyCode.Mouse0)) {
			GameObject clickTarget = RaycastCheck<IClickable>(Input.mousePosition);
			if(clickTarget != null) {
				PickUpGear(clickTarget);
			}
		}
	}

	private Vector2 GetMousePositionInWorldCoordinates() {
		Vector3 mousePos = Input.mousePosition;
		return Camera.main.ScreenToWorldPoint(mousePos);
	}

	private void SpawnGearAtMouse(GameObject gearPrefab) {
		// Destroy the currently carried gear, if any
		DestroyCurrentlyCarriedGear();
		// Spawn the selected gear at the current position of the mouse
		PickUpGear(Game.SpawnGear(gearPrefab, GetMousePositionInWorldCoordinates()));
	}

	private void DestroyCurrentlyCarriedGear() {
		if(CurrentlyCarried != null) {
			Game.RemoveGear(CurrentlyCarried);
			EmptyHand();
		}
	}

	private void PickUpGear(GameObject gear) {
		Debug.Assert(gear != null && gear.CompareTag(GameController.GEAR_TAG));
		CurrentlyCarried = gear;
		Cursor.visible = false;
	}

	private void EmptyHand() {
		CurrentlyCarried = null;
		Cursor.visible = true;
	}

	// Projects a ray from the given position to a clickable element
	// and sets its values accordingly into the input variables
	private GameObject RaycastCheck<T>(Vector3 screenPosition) where T:IClickable {
		// Get ray and raycast hit
		Ray ray = Camera.main.ScreenPointToRay(screenPosition);
		RaycastHit2D hit = Physics2D.Raycast(ray.origin, ray.direction, Mathf.Infinity);
		// Check if it hits anything
		if(hit) {
			Transform objectHit = hit.transform;
			// Check if it has the necessary component
			T controller = objectHit.GetComponent<T>();
			if(controller != null) {
				return controller.GetGameObject();
			}
		}
		return null;
	}

	public void SwitchToScene(string SceneName) {
		SceneManager.LoadScene(SceneName);
	}

	public void QuitGame() {
		Application.Quit();
	}
}
