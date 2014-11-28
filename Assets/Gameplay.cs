using UnityEngine;
using System.Collections;

public class Gameplay : MonoBehaviour {

	public static bool 		GameInPlay;	// Is the game in play, or has it been won?
	public static int 		Turn;		// 0 For Computer, 1 For Player
	public static int[,] 	Board;		// The board.

	// The size of the board:
	const int BOARD_SIZE_COLS = 7;
	const int BOARD_SIZE_ROWS = 6;

	// The y-position from which to drop the pieces onto the board:
	const float DROP_PIECE_FROM_Y_POS = 1.078818f;

	// The AI difficulty level
	// 0 == Easy, 1 == Medium, 2 == Hard... 3 == Insane?
	public static int difficulty = 0;

	// Keeps track of full columns and column heights:
	public static bool[] ColsFull;
	public static int[] ColHeights;

	// The position in which to instantiate dropping pieces based on the column number:
	public static float[] ColPosition = new float[BOARD_SIZE_COLS];

	// Keeps track of the number of peices for each unit... so we don't have to calc each time.
	// MovesMade[0] == Computer, MovesMade[1] == Player
	public static int[] MovesMade = new int[2] { 0, 0 };

	// So we can run a Courotine on the AI's thinking process.
	private bool DisplayingAIMove = false;


	// Use this for initialization
	void Awake () {

		// Get the column positions, based on the test pieces...
		GameObject testPieces = GameObject.Find ("TestPieces").gameObject;
		foreach(Transform child in testPieces.transform) ColPosition[int.Parse(child.name.Replace("Col", ""))] = child.transform.position.x;


		// <----------------------- UNCOMMENT TO PLAY WITH THE TEST PEICES -----------------------> //
		//        DO NOT DELETE THE TEST PIECES, THEY ARE USED TO POSITION THE REAL PIECES!         //

		Destroy (testPieces);


		// Reset the game to start...
		ResetGame ();
	
	} // End Start()


	// Update is called once per frame
	void Update () {

		// Keep track of full columns, so we don't have to iterate each time...
		for(int i = 0; i < Board.GetLength(0); i++) {

			int sum = 0;

			for(int n = 0; n < Board.GetLength(1); n++) {

				if(Board[i, n] != 0) sum++;
				if(sum >= BOARD_SIZE_COLS) ColsFull[n] = true;

			} // End inner for loop

		} // End outer for loop


		// Only show the "Select Column" Arrows if it's the player's turn:
		bool showArrows = (Turn == 1) ? true : false;
		GameObject arrowContainer = GameObject.Find ("Arrows");
		foreach(Transform child in arrowContainer.transform) NGUITools.SetActive (child.gameObject, showArrows);

		// If it's the computer's turn, make an AI Move:
		if(Turn == 0 && DisplayingAIMove == false) StartCoroutine(MakeAIMove());

		Debug.Log (Turn);
	
	} // End Update()

	
	IEnumerator MakeAIMove () {

		DisplayingAIMove = true;

		yield return new WaitForSeconds (Random.Range (2, 7));

		DisplayMove(AIMove());
		MovesMade[0]++;

		// Change to The Player's Move
		Turn = 1;

		DisplayingAIMove = false;

	} // End MakeAIMove()


	public static void MakePlayerMove(int col) {

		// Update the board...

		MovesMade [1]++;

	} // End MakeMove()


	public static void DisplayMove(int col) {

		GameObject piece = Resources.Load<GameObject> ("Piece");

		Piece pieceScript = piece.GetComponent<Piece> ();
		pieceScript.isAIPiece = (Turn == 0) ? true : false;

		GameObject newPiece = Instantiate (piece.gameObject, new Vector3 (ColPosition [col], DROP_PIECE_FROM_Y_POS, 0), Quaternion.identity) as GameObject;
		newPiece.transform.parent = GameObject.Find ("Panel").gameObject.transform;

	} // End DisplayAIMove()


	/**
	 *  @TODO - JOSH
	 */
	private int AIMove() {

		// IMPLEMENT AI LOGIC HERE!
		// OR POSSIBLY CALL ANOTHER SCRIPT'S METHOD
		// SO THIS CLASS DOESN'T GET 'JUNKY.'

		// JUST MODIFY THE BOARD, THEN RETURN AN INT
		// REPRESENTING THE COLUMN CHOSEN BY THE AI...

		// THIS WILL BE CALLED AFTER X RANDOM SECONDS IN A COUROTINE...

		// Update the Turn var, so the player can now make their move...
		return 0;

	} // End MakeAIMove()


	public void ResetGame () {

		// Set the game in play:
		GameInPlay = true;

		// Select A Random First Move
		Turn = Random.Range (0, 2);
		Debug.Log (Turn);

		// Reset the Board...
		Board = new int[BOARD_SIZE_ROWS, BOARD_SIZE_COLS];

		// 0 == Empty, -1 == Computer Move, 1 == Player Move
		for(int i = 0; i < Board.GetLength(0); i++) {
			for(int n = 0; n < Board.GetLength(1); n++) { Board[i, n] = 0; }
		} // End outer foreach loop

		// Reset the ColsFull Array
		ColsFull = new bool[BOARD_SIZE_COLS];
		for(int i = 0; i < ColsFull.Length; i++) ColsFull[i] = false;

		// Reset the ColHeights Array
		ColHeights = new int[BOARD_SIZE_COLS];
		for(int i = 0; i < ColHeights.Length; i++) ColHeights[i] = 0;

		// Reset the number of moves made for the AI & Player
		MovesMade = new int[2] { 0, 0 };

	} // End resetGame()


} // End Gameplay Class
