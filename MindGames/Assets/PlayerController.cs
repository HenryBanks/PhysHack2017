using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Text;
using System.IO;

public class PlayerController : MonoBehaviour {

	public float speed=20f;
	public float max_speed = 2f;
	public int count_down = -1;
	Rigidbody2D rb;
	public Text scoreText;
	public Vector3 resetPosition=new Vector3 (0,0,-1);
	public string input;


	public float jumpRate = 0.5F;
	private float nextJump = 0.0F;
	public float jumpLength = 0.5f;
	private float jumpEnd=0.0f;
	private bool isJumping=false;
	private bool automatic=true;
	private bool losing=false;
	private int score = 0;
	private int highscore=0;

	// Use this for initialization
	void Start () {
		rb = GetComponent<Rigidbody2D> ();
		ReadHighScore ("highscore.txt");
	}

	public void Lose(){
		losing = true;
		Debug.Log ("You lose!");
		if (score > highscore) {
			highscore = score;
			SaveHighscore ("highscore.txt");
		}
		ReadHighScore ("highscore.txt");
		StartCoroutine (DeathTimer ());
		//Destroy (this.gameObject);
		//transform.position = new Vector3 (0,0,-1);
		//rb.velocity = new Vector2 (0, 0);
		//transform.rotation = Quaternion.AngleAxis(0.0f, Vector3.forward);
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
		//Debug.Log (vIn);
		if (Input.GetButtonDown(input) && Time.time > nextJump) {
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
		if (Mathf.Abs(transform.position.y) > 5.5 && !losing) {
			losing = true;
			Lose ();
		}

		setText ();

	}

	void FixedUpdate(){
		Vector2 dir = rb.velocity;
		float angle = Mathf.Atan2(dir.y, dir.x+3.0f) * Mathf.Rad2Deg;
		transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
		if (!losing) {
			score += 1;
		}
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
						//Debug.Log("Go DOWN");
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

	IEnumerator DeathTimer(){
		Debug.Log ("Dead");
		transform.position = new Vector3 (0, -20, -1);
		yield return new WaitForSeconds (3);
		Debug.Log ("Alive");
		transform.position = resetPosition;
		rb.velocity = new Vector2 (0, 0);
		transform.rotation = Quaternion.AngleAxis(0.0f, Vector3.forward);
		losing = false;
		score = 0;
	}

	void setText(){
		string scoreString = score.ToString ();
		string highscoreString = highscore.ToString ();
		string fullString = "SCORE: " + scoreString + "\nHIGHSCORE: " + highscoreString;
		scoreText.text = fullString;
	}

	private bool ReadHighScore(string fileName)
	{
		string line;
		StreamReader theReader = new StreamReader(fileName, Encoding.Default);
		using (theReader)
		{
			do
			{
				line = theReader.ReadLine();

				if (line != null)
				{
					int.TryParse(line,out highscore);
				}
			}
			while (line != null);   
			theReader.Close();
			return false;
		}
	}

	private void SaveHighscore(string fileName){
		StreamWriter theWriter = new StreamWriter (fileName, false, Encoding.Default);
		using (theWriter) {
			theWriter.Write (highscore.ToString());
		}
		return;
	}



}
