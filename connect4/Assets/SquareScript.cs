using UnityEngine;
using System.Collections;

public class SquareScript : MonoBehaviour {
	
	SpriteRenderer renderer;
	public int row;
	public int column;
	
	// Use this for initialization
	void Start () {
		renderer = GetComponent<SpriteRenderer>();
		renderer.color = Color.white;
	}
	
	// Update is called once per frame
	void Update () {
	
	}
	
	void OnMouseDown(){
		if(renderer.color == Color.white){
			AIScript.script.boardRow[row].column[column] = 1;
			renderer.color = Color.red;
		}
		else if(renderer.color == Color.red){
			AIScript.script.boardRow[row].column[column] = 2;
			renderer.color = Color.black;
		}
		else{
			AIScript.script.boardRow[row].column[column] = 0;
			renderer.color = Color.white;
		}
	}
}
