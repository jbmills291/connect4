using UnityEngine;
using System.Collections;

public class TurnText : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {

		// Change the Turn Text based on who's turn it is.
		UISprite tText = gameObject.GetComponent<UISprite> ();
		tText.spriteName = (Gameplay.Turn == 0) ? "Computer" : "Player1";
		tText.MakePixelPerfect ();
		tText.MarkAsChanged ();

	} // End Update()
}
