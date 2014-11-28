using UnityEngine;
using System.Collections;

public class Gameplay : MonoBehaviour {

	public static bool 		GameInPlay;	// Is the game in play, or has it been won?
	public static int 		Turn;		// 0 For Computer, 1 For Player
	public static int[,] 	Board;		// The board.
	
	public static int[] 	FlatBoard;  // The board in a static format (for win checking) -->

	/**
	 * 
	 * 	FlatBoard Example
	 * 	--------------------------------------------------------------
	 *  Each index holds an int value, just like for the regular board
	 *  0 == No Move, 1 == Player Move, -1 == Computer Move
	 * 	--------------------------------------------------------------
	 * 	35	36	37	38	39	40	41
	 * 			......
	 * 	14	15	16	17	18	19	20
	 * 	07	08	09	10	11	12	13
	 * 	00	01	02	03	04	05	06
	 * 
	 */

	// The size of the board:
	const int BOARD_SIZE_COLS = 7;
	const int BOARD_SIZE_ROWS = 6;

	const int COMP_MIN_THINKING_TIME = 1; // In Seconds
	const int COMP_MAX_THINKING_TIME = 2;

	public static int[] BOARD_SIZE = new int[2] { BOARD_SIZE_ROWS, BOARD_SIZE_COLS };

	// The y-position from which to drop the pieces onto the board:
	const float DROP_PIECE_FROM_Y_POS = 1.078818f;

	// The AI difficulty level
	// 0 == Easy, 1 == Medium, 2 == Hard... 3 == Insane?
	public static int Difficulty = 0;

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

	private GameObject selectColumn;
	private GameObject thinking;
	private GameObject turnText;
	private GameObject turnIcon;
	private GameObject winObjects;
	
	// Use this for initialization
	void Awake () {

		// The "Select Column" GameObject
		selectColumn 	= GameObject.Find("Select").gameObject;
		thinking 		= GameObject.Find("Thinking").gameObject;
		turnText 		= GameObject.Find("TurnText").gameObject;
		turnIcon 		= GameObject.Find("TurnIcon").gameObject;
		winObjects 		= GameObject.Find("WinLoose").gameObject;

		// Get the column positions, based on the test pieces...
		GameObject testPieces = GameObject.Find ("TestPieces").gameObject;
		foreach(Transform child in testPieces.transform) ColPosition[int.Parse(child.name.Replace("Col", ""))] = child.transform.position.x;

		// <------------------------ COMMENT TO PLAY WITH THE TEST PEICES ------------------------> //
		//        DO NOT DELETE THE TEST PIECES, THEY ARE USED TO POSITION THE REAL PIECES!         //
		Destroy (testPieces);

		// Reset the game to start...
		ResetGame ();

		// Hide the Win / Loose Game Objects
		NGUITools.SetActive (winObjects, false);
	
	} // End Start()


	// Update is called once per frame
	void Update () {

		// Check For Win
		int Winner = CheckWin ();
		Debug.Log (Winner.ToString () + " WIN DEBUG");
		if(Winner != 0) {
			GameInPlay = false;
			Debug.Log("WINNER IS " + Winner.ToString());
		}

		
		if(GameInPlay == true) { // If the game is won, or lost don't make moves...

			// Keep track of full columns, so we don't have to iterate each time...
			for(int i = 0; i < Board.GetLength(1); i++) {

				int sum = 0;

				for(int n = 0; n < Board.GetLength(0); n++) {

					// Debug.Log ("BOARD: " + Board[n, i].ToString () + " - ROW" + n.ToString() + ", COL" + i.ToString());

					if(Board[n, i] != 0) sum++;
					if(sum >= BOARD_SIZE_COLS - 1) ColsFull[i] = true;

				} // End inner for loop

			} // End outer for loop

			// Only show the "Select Column" Arrows if it's the player's turn:
			bool showArrows = (Turn == 1) ? true : false;
			GameObject arrowContainer = GameObject.Find ("Arrows");
			foreach(Transform child in arrowContainer.transform) NGUITools.SetActive (child.gameObject, showArrows);

			// Only Show "Select A Column" When it's the player's turn:
			NGUITools.SetActive (selectColumn, showArrows);

			// Only Show "Computer Is Thinking" When it's the AI's turn:
			NGUITools.SetActive (thinking, !showArrows);

			// If it's the computer's turn, make an AI Move:
			if(Turn == 0 && DisplayingAIMove == false) StartCoroutine(MakeAIMove());

		} // End if(GameInPlay) { ... }

		else { // The game has been won:

			UISprite winSprite = winObjects.transform.FindChild("WinText").GetComponent<UISprite>();

			if(Winner == 1) {
				winSprite.spriteName = "YouWin";
			}
			else {
				winSprite.spriteName = "YouLoose";
			}

			NGUITools.SetActive (thinking, 		false);
			NGUITools.SetActive (selectColumn, 	false);
			NGUITools.SetActive (turnText, 		false);
			NGUITools.SetActive (turnIcon, 		false);
			NGUITools.SetActive (winObjects, 	true);

			StartCoroutine(EndGame());

		}// End if/else(GameInPlay) { ... }
	
	} // End Update()


	IEnumerator EndGame () {

		yield return new WaitForSeconds (5);
		Application.LoadLevel("GameOver");

	} // End EndGame()

	
	IEnumerator MakeAIMove () {

		DisplayingAIMove = true;

		yield return new WaitForSeconds (Random.Range (COMP_MIN_THINKING_TIME, COMP_MAX_THINKING_TIME));

		DisplayMove(AIMove());
		MovesMade[0]++;

		// Change to The Player's Move
		Turn = 1;

		DisplayingAIMove = false;

	} // End MakeAIMove()


	public static void MakePlayerMove(int col) {

		DisplayMove(col);
		MovesMade[1]++;
		
		// Change to The Computers's Move
		Turn = 0;

	} // End MakeMove()


	public static void DisplayMove(int col) {

		// Update the Board & FlatBoard...
		Board[ColHeights[col], col] = FlatBoard[ColHeights[col] * (BOARD_SIZE[0] + 1) + col] = (Turn == 0) ? -1 : 1;
		ColHeights [col]++;

		// Debug.Log (ColHeights [col].ToString () + " - COL - " + col.ToString ());

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
		// RIGHT NOW IT COMP JUST MAKES A RANDOM MOVE...
		return MakeRandomAIMove ();

	} // End MakeAIMove()


	private int MakeRandomAIMove() {

		int col = Random.Range (0, BOARD_SIZE_COLS);
		while(ColsFull[col] == true) col = Random.Range (0, BOARD_SIZE_COLS);
		return col;

	} // End MakeRandomAIMove


	public static int CheckWin() {
		
		for(int i = 0; i < FlatBoard.Length; i++) {
			
			int player = FlatBoard[i];

			// Debug.Log (FlatBoard[i].ToString() + " _ " + i.ToString());
			
			if(player != 0) { // 0 == No Move, -1 == Computer, 1 == Player
				
				// Horizonal Win
				if(i % Gameplay.BOARD_SIZE[1] <= 3 && player == FlatBoard[i + 1] && player == FlatBoard[i + 2] && player == FlatBoard[i + 3]) {
					return player;
				}
				
				// Vertical Win
				if(i <= 14 && player == FlatBoard[i + 7] && player == FlatBoard[i + 14] && player == FlatBoard[i + 21]) {
					return player;
				}
				
				// Diagonal Right Win
				if(i <= 17 && player == FlatBoard[i + 8] && player == FlatBoard[i + 16] && player == FlatBoard[i + 24]) {
					return player;
				}
				
				// Diagonal Left Win
				if(i >= 24 && player == FlatBoard[i - 6] && player == FlatBoard[i - 12] && player == FlatBoard[i - 18]) {
					return player;
				}
				
			} // End if block
			
		} // End for loop
		
		return 0;
		
	} // End Check()


	public void ResetGame () {

		// Set the game in play:
		GameInPlay = true;

		// Select A Random First Move
		Turn = Random.Range (0, 2);

		// Reset the Board...
		Board = new int[BOARD_SIZE_ROWS, BOARD_SIZE_COLS];
		FlatBoard = new int[BOARD_SIZE_ROWS * BOARD_SIZE_COLS];

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
