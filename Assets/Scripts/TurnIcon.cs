using UnityEngine;
using System.Collections;

public class TurnIcon : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {

		// Change the Turn Icon based on who's turn it is.
		UISprite tText = gameObject.GetComponent<UISprite> ();
		tText.spriteName = (Gameplay.Turn == 0) ? "YellowPiece" : "RedPiece";
		tText.MakePixelPerfect ();
		tText.MarkAsChanged ();

	} // End Update()
}
