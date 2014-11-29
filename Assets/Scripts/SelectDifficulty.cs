using UnityEngine;
using System.Collections;

public class SelectDifficulty : MonoBehaviour {

	public int difficulty = 0;

	// Use this for initialization
	void OnClick() {
		Gameplay.Difficulty = difficulty;
		Application.LoadLevel ("Gameplay");
	}
}
