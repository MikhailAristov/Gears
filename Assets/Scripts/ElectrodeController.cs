using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ElectrodeController : MonoBehaviour {

	public const string TAG_ELECTRODE = "Electrode";
	const float MOVEMENT_THRESHOLD = 0.01f;

	public int Charge;
	public bool HasPotential;
	public bool HasPower;
	public SpriteRenderer MyGlow;

	private Vector2 LastPosition;

	private List<ElectrodeController> ContactingElectrodes;

	// Use this for initialization
	void Start() {
		LastPosition = transform.position;
		ContactingElectrodes = new List<ElectrodeController>();
		MyGlow.enabled = false;
	}
	
	// Update is called once per frame
	void FixedUpdate() {
		// The electrode has potential when the electrode is moving
		HasPotential = (Vector2.Distance(LastPosition, transform.position) > MOVEMENT_THRESHOLD);
		MyGlow.enabled = HasPotential;
		LastPosition = transform.position;
		// If you have potential, check if any neighbors also have it
		HasPower = false;
		if(HasPotential) {
			foreach(ElectrodeController electr in ContactingElectrodes) {
				// To generate charge, the electrodes must have opposite charges, and both have power
				HasPower |= electr.HasPotential && (Charge + electr.Charge == 0) ;
			}
		}
	}

	void OnCollisionEnter2D(Collision2D coll) {
		GameObject go = coll.gameObject;
		if(go.CompareTag(TAG_ELECTRODE)) {
			ElectrodeController electr = go.GetComponent<ElectrodeController>();
			if(electr != null && !ContactingElectrodes.Contains(electr)) {
				ContactingElectrodes.Add(electr);
			}
		}
	}

	void OnCollisionExit2D(Collision2D coll) {
		GameObject go = coll.gameObject;
		if(go.CompareTag(TAG_ELECTRODE)) {
			ElectrodeController electr = go.GetComponent<ElectrodeController>();
			if(electr != null) {
				ContactingElectrodes.Remove(electr);
			}
		}
	}
}
