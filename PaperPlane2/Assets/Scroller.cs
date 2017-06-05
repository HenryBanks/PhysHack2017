using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Scroller : MonoBehaviour {
		public float scrollSpeed;
		public float tileSizeZ;

		private Vector3 startPosition;

		void Start ()
		{
			startPosition = transform.position;
		}

		void Update ()
		{
			float newPosition = Mathf.Repeat(Time.time * scrollSpeed, tileSizeZ);
		transform.position = startPosition + new Vector3 (-1.0f,0.0f,0.0f) * newPosition;
		}
}
