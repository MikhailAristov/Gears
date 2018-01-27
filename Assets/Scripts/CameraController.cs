using System;
using System.IO;
using UnityEngine;

public class CameraController : MonoBehaviour {

	public const float VISIBLE_TABLE_WIDTH = 19f;
	public const float VISIBLE_TABLE_HEIGHT = 10f;

	private float adjustedToAspectRatio;

	// Use this for initialization
	void Start() {
		adjustToAspectRatio();
		// Set screen timeout to never-sleep
		Screen.sleepTimeout = SleepTimeout.NeverSleep;
	}

	// Update is called once per frame
	void Update() {
		if(Mathf.Abs(Camera.main.aspect - adjustedToAspectRatio) > 0.01f) {
			adjustToAspectRatio();
		}
	}

	// Adjust the camera's orthographic "zoom" so that the entire table is visible in the current resolution
	private void adjustToAspectRatio() {
		Camera.main.orthographicSize = (float)Math.Round(VISIBLE_TABLE_WIDTH / 2 / Camera.main.aspect, 1);
		adjustedToAspectRatio = Camera.main.aspect;
	}

	// Stores a screenshot to My Pictures
	public void takeScreenshot() {
		// Create directory if necessary
		string targetDir = String.Format("{0}\\{1}", Environment.GetFolderPath(Environment.SpecialFolder.MyPictures), Application.productName);
		if(!Directory.Exists(targetDir)) {
			Directory.CreateDirectory(targetDir);
		}
		// Take and store the screenshot
		string filepath = String.Format("{0}\\screenshot_{1:yyyyMMddHHmmssfff}.png", targetDir, System.DateTime.Now);
		ScreenCapture.CaptureScreenshot(filepath);
		Debug.LogFormat("Screenshot saved to {0}.", filepath);
	}

	void OnApplicationQuit() {
		Screen.sleepTimeout = SleepTimeout.SystemSetting;
	}
}
