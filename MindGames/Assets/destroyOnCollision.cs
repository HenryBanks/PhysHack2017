using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class destroyOnCollision : MonoBehaviour {

	void OnTriggerEnter2D(Collider2D other) {
		Debug.Log ("Collision");
		if (other.gameObject.tag == "Player")
			other.gameObject.GetComponent<PlayerController> ().Lose ();

	}
}
