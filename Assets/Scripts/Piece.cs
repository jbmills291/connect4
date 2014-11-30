using UnityEngine;
using System.Collections;

public class Piece : MonoBehaviour {

	public bool isAIPiece = false, enableHit = true;
	public int numberOfHits = 3;

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
		if (enableHit) {
			numberOfHits--;
			audio.Play ();
			if(numberOfHits == 0)
				enableHit = false;
		}
	}
}
