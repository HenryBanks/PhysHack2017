﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Text;
using System.IO;

public class PlayerController : MonoBehaviour {

	public float speed=20f;
	public float max_speed = 2f;
	public int count_down = -1;
	Rigidbody2D rb;

	public float jumpRate = 0.5F;
	private float nextJump = 0.0F;
	public float jumpLength = 0.5f;
	private float jumpEnd=0.0f;
	private bool isJumping=false;
	private bool automatic=true;

	// Use this for initialization
	void Start () {
		rb = GetComponent<Rigidbody2D> ();
	}

	void Lose(){
		Debug.Log ("You lose!");
		Destroy (this.gameObject);
		transform.position = new Vector3 (0,0,-1);
	}
	
	// Update is called once per frame
	void Update () {
		/*
		if (count_down < 0) {
			float vIn = Input.GetAxis ("Vertical");
			Debug.Log (vIn);
			if (vIn != 0) {
				rb.AddForce (new Vector2 (0, speed * vIn));
				count_down = 5;
			}
		} else {
			count_down -= 1;
			rb.AddForce (new Vector2 (0, speed * 1));
		}
		Debug.Log (count_down);
		*/

		float vIn = Input.GetAxis ("Vertical");
		Debug.Log (vIn);
		if (Mathf.Abs(vIn)>Mathf.Epsilon && Time.time > nextJump) {
			nextJump = Time.time + jumpRate;
			jumpEnd = Time.time + jumpLength;
			isJumping = true;
		}

		if (isJumping) {
			if (Time.time > jumpEnd) {
				isJumping = false;
			}
			rb.AddForce (new Vector2 (0, speed-3*rb.velocity.y));
		}

		//float vIn = Input.GetAxis ("Vertical");
		//rb.AddForce (new Vector2 (0, 3*speed * vIn));

		if (Input.anyKeyDown) {
			automatic = false;
		}

		if (Input.GetKeyDown (KeyCode.Escape)) {
			Application.Quit ();
		}

		if (automatic) {
			if (Load ("input.dat")) {
				if (Time.time > nextJump) {
					nextJump = Time.time + jumpRate;
					jumpEnd = Time.time + jumpLength;
					isJumping = true;
				}
				Reset ("input.dat");
			}
		}


		//Debug.Log (rb.velocity.y);
		if (Mathf.Abs(transform.position.y) > 5.5) {
			Lose ();
		}

	}

	void FixedUpdate(){
		Vector2 dir = rb.velocity;
		float angle = Mathf.Atan2(dir.y, dir.x+3.0f) * Mathf.Rad2Deg;
		transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
	}

	private bool Load(string fileName)
	{
		string line;
		StreamReader theReader = new StreamReader(fileName, Encoding.Default);
		// Immediately clean up the reader after this block of code is done.
		// You generally use the "using" statement for potentially memory-intensive objects
		// instead of relying on garbage collection.
		// (Do not confuse this with the using directive for namespace at the 
		// beginning of a class!)
		using (theReader)
		{
			// While there's lines left in the text file, do this:
			do
			{
				line = theReader.ReadLine();

				if (line != null)
				{
					// Do whatever you need to do with the text line, it's a string now
					// In this example, I split it into arguments based on comma
					// deliniators, then send that array to DoStuff()
					if(line=="1"){
						Debug.Log("Go UP");
						return true;
					}
					if(line=="0"){
						Debug.Log("Go DOWN");
						return false;
					}
				}
			}
			while (line != null);
			// Done reading, close the reader and return true to broadcast success    
			theReader.Close();
			return false;
		}
	}

	private void Reset(string fileName){
		StreamWriter theWriter = new StreamWriter (fileName, false, Encoding.Default);
		using (theWriter) {
			theWriter.Write ("0");
		}
		return;
	}

}
