using UnityEngine;
using System.Collections;

public class Piece : MonoBehaviour {

	public bool isAIPiece = false;

	// Use this for initialization
	void Start () {
		if(isAIPiece) {
			gameObject.GetComponent<UISprite>().spriteName = "YellowPiece";
		}
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	void OnCollisionEnter2D(Collision2D o) {
		audio.Play ();
	}
}
