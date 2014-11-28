using UnityEngine;
using System.Collections;

public class SelectColumnButton : MonoBehaviour {

	public int Column = 1;

	// Use this for initialization
	void Start () {

		if(Column == null) throw new UnityException("You must define a column number for this Column Button!");
		NGUITools.SetActive (gameObject, false);

	} // End Start()
	
	// Update is called once per frame
	void Update () {

		// Kill this button if this column is full...
		if(Gameplay.ColsFull[Column] || Gameplay.GameInPlay == false) Destroy(gameObject);

	} // End Update()

	// When the player clicks this button make a move in this column...
	void OnClick() {
		Gameplay.MakePlayerMove (Column);
	}
}
