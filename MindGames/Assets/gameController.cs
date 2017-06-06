using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class gameController : MonoBehaviour {

	public GameObject obstacle;
	public Vector3 spawnPosition=new Vector3(16,4,-1);
	public float spawnWait = 5.0f;

	void Start(){
		StartCoroutine (SpawnObs ());
	}

	IEnumerator SpawnObs ()
	{
		yield return new WaitForSeconds (spawnWait);
		while (true)
		{
			float pos=1;
			if (Random.Range (0.0f, 1.0f)>0.5f) {
				pos = -1;
			}
			Debug.Log (pos);
			Vector3 spawnPos = new Vector3 (spawnPosition.x, Random.Range (-4.0f, 9.0f),spawnPosition.z);
			Quaternion spawnRotation = Quaternion.identity;
			Instantiate (obstacle, spawnPos, spawnRotation);
			yield return new WaitForSeconds (spawnWait);
		}
	}
}
