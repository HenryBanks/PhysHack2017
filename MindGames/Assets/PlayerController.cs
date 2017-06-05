using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour {

	public float speed=20f;
	public float max_speed = 2f;
	Rigidbody2D rb;

	// Use this for initialization
	void Start () {
		rb = GetComponent<Rigidbody2D> ();
	}
	
	// Update is called once per frame
	void Update () {
		float vIn = Input.GetAxis ("Vertical");
		rb.AddForce (new Vector2(0, speed * vIn));

		Debug.Log (rb.velocity.y);
		rb.velocity = new Vector2 (0, Mathf.Clamp (rb.velocity.y, -2, 2));
	}

	void FixedUpdate(){
		Vector2 dir = rb.velocity;
		float angle = Mathf.Atan2(dir.y, dir.x+3.0f) * Mathf.Rad2Deg;
		transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
	}
}
