using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class UIController : MonoBehaviour {

	const string MOUSE_SCROLL_WHEEL = "Mouse ScrollWheel";

	public GameController Game;
	public Canvas TutorialOverlay;

	// The 0th element is always ignored...
	public GameObject[] GearPrefabs;
	private int LastSelectedPrefabIndex;

	// 0th scene in build index is called via F1, 1st via F2, and so on...
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

	public Canvas EndgameMessaging;
	public UnityEngine.UI.Text VictoryMainMessage;
	public UnityEngine.UI.Text VictoryExtraMessage;
	public UnityEngine.UI.Text LossMainMessage;
	public UnityEngine.UI.Text LossExtraMessage;
	private bool EndgameMessagingEnabled;

	private string[] CongratulationsMessages = {
		"Well done!",
		"Awesome job!",
		"Good work!"
	};
	private string BeatingTheGameMessage = "Congratulations!";
	private string BeatingTheGameSubmessage = "You have beaten the final challenge!\r\nPress F1 to play again, or Esc to exit.";

	public GameObject CurrentlyCarried;

	// Update is called once per frame
	void Update() {
		if(Input.anyKeyDown) {
			CheckControlButtons();
		}

		// Do not continue if level has been failed or completed
		if(Game.LevelCompleted || Game.LevelFailed) {
			if(EndgameMessagingEnabled) {
				return;
			}
			EmptyHand();
			if(Game.LevelCompleted) {
				StartCoroutine(DisplayCongratulations());
			} else {
				DisplayHint(Game.LevelFailedHint);
			}
			EndgameMessagingEnabled = true;
		}

		// Check if a number has been entered via numpad
		if(Input.inputString != "") {
			int numericKey = 0;
			int.TryParse(Input.inputString, out numericKey);
			if(numericKey > 0 && numericKey < GearPrefabs.Length) {
				LastSelectedPrefabIndex = numericKey;
				SpawnGearAtMouse(GearPrefabs[LastSelectedPrefabIndex]);
			}
		} else if(Mathf.Abs(Input.GetAxis(MOUSE_SCROLL_WHEEL)) > 0) {
			// Count the prefab index up and normalize it to proper range
			LastSelectedPrefabIndex = Mathf.RoundToInt(LastSelectedPrefabIndex + Mathf.Sign(Input.GetAxis(MOUSE_SCROLL_WHEEL)));
			LastSelectedPrefabIndex = (LastSelectedPrefabIndex + GearPrefabs.Length) % GearPrefabs.Length;
			// Spawn the prefab or destroy currently held one
			if(LastSelectedPrefabIndex > 0) {
				SpawnGearAtMouse(GearPrefabs[LastSelectedPrefabIndex]);
			} else {
				DestroyCurrentlyCarriedGear();
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
		// Or take a screenshot?
		if(Input.GetKeyDown(KeyCode.S)) {
			Camera.main.GetComponent<CameraController>().takeScreenshot();
		}
		// Otherwise check for level select
		foreach(KeyCode k in FKeyToLevelMapping.Keys) {
			if(Input.GetKeyDown(k)) {
				int LevelIndex = FKeyToLevelMapping[k];
				if(LevelIndex < SceneManager.sceneCountInBuildSettings) {
					SwitchToScene(LevelIndex);
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

	private string GetKeyForLevelIndex(int index) {
		index %= SceneManager.sceneCountInBuildSettings;
		foreach(KeyCode k in FKeyToLevelMapping.Keys) {
			if(FKeyToLevelMapping[k] == index) {
				return k.ToString();
			}
		}
		throw new KeyNotFoundException();
	}

	private IEnumerator DisplayCongratulations() {
		// Hide the tutorial
		if(TutorialOverlay != null) {
			TutorialOverlay.gameObject.SetActive(false);
		}
		// Take a screenshot
		Camera.main.GetComponent<CameraController>().takeScreenshot();
		yield return null;
		// Activate messages
		EndgameMessaging.gameObject.SetActive(true);
		VictoryMainMessage.gameObject.SetActive(true);
		VictoryExtraMessage.gameObject.SetActive(true);
		// Check if it's the last level
		int thisLevelIndex = SceneManager.GetActiveScene().buildIndex;
		if(thisLevelIndex + 1 == SceneManager.sceneCountInBuildSettings) {
			VictoryMainMessage.text = BeatingTheGameMessage;
			VictoryExtraMessage.text = BeatingTheGameSubmessage;
		} else {
			// Pick a random congratulation from the list...
			VictoryMainMessage.text = CongratulationsMessages[UnityEngine.Random.Range(0, CongratulationsMessages.Length)];
			// Manipulate secondary message
			string txt = VictoryExtraMessage.text.Replace("<ThisKey>", GetKeyForLevelIndex(thisLevelIndex)).Replace("<NextKey>", GetKeyForLevelIndex(thisLevelIndex + 1));
			VictoryExtraMessage.text = txt;
		}
	}

	private void DisplayHint(string HintText) {
		// Activate messages
		EndgameMessaging.gameObject.SetActive(true);
		LossMainMessage.gameObject.SetActive(true);
		LossExtraMessage.gameObject.SetActive(true);
		// Manipulate secondary message
		int thisLevelIndex = SceneManager.GetActiveScene().buildIndex;
		string txt = LossExtraMessage.text.Replace("<ThisKey>", GetKeyForLevelIndex(thisLevelIndex)).Replace("<Hint>", HintText);
		LossExtraMessage.text = txt;
	}

	public void SwitchToScene(int SceneIndex) {
		SceneManager.LoadScene(SceneIndex);
	}

	public void SwitchToScene(string SceneName) {
		SceneManager.LoadScene(SceneName);
	}

	public void QuitGame() {
		Debug.Log("Qutting the game...");
		Application.Quit();
	}
}
