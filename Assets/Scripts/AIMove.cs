using UnityEngine;
using System.Collections;

public class AIMove {

	private int[,] Board;
	private int    MAX_DEPTH;
	private int    Winner;

	private int    Column;

	private bool AIWin, PlayerWin;
	public int BoardsChecked = 0;

	private int MIN_INT = unchecked((int) -Mathf.Infinity + 1);
	private int MAX_INT = unchecked((int)  Mathf.Infinity - 1);

	public static int[] INCREMENT = { 0, 1, 4, 32, 128, 512 }; 

	public AIMove(int[,] board, int depth) {
		this.Board = board;
		this.MAX_DEPTH = depth;
	}



	public int MiniMax() {

		PlayerWin = AIWin = false;

		BoardsChecked = 0;
		Winner = 0;

		// Eval Player Move
		EvalMove(-1, 0, 1, -1, MIN_INT, MAX_INT);
		if(Winner != 0 || AIWin) return Column;

		EvalMove(1, 0, 1, -1, MIN_INT, MAX_INT);
		if(Winner != 0 || PlayerWin) return Column;

		EvalMove(-1, 0, MAX_DEPTH, -1, MIN_INT, MAX_INT);

		// No wins, reuturn the best stragetic move by the AI...
		return Column;
	}


	public int GetBoardsChecked() { return BoardsChecked; }


	public void PrintBoard (int[,] board) {
		
		string str = "";
		for(int i = Gameplay.BOARD_SIZE[0] - 1; i >= 0; i--) {
			
			for(int n = 0; n < Gameplay.BOARD_SIZE[1]; n++) {
				str += board[i, n].ToString() + " | ";
			}
			
			str += "\n";
			
		} // End outer for loop
		
		Debug.Log (str);
		
	}


	public int EvalMove(int player, int depth, int maxDepth, int column, int alpha, int beta) {

		//Debug.Log (player);

		BoardsChecked++;

		// Start in Middle to Always win!
		if(Gameplay.MovesMade[0] + Gameplay.MovesMade[1] == 0) {
			Column = 3;
			return 100000;
		}

		int Score = 0;
		int Min = unchecked((int) Mathf.Infinity);
		int Max = unchecked((int) -Mathf.Infinity);

		if(column != -1) {
			Score = EvalScore(-player, column, depth);
			Winner = Gameplay.CheckWin(Board);
			if(Winner != 0) {
				Column = column;
				return Score; // Highest Score...
			}
		}

		if(depth == MAX_DEPTH) return Score;

		for(int c = 0; c < Gameplay.BOARD_SIZE[1]; c++) {

			if (!Gameplay.ColsFull[c]) {

				// Make A "Fake" Move...
				Board[Gameplay.ColHeights[c], c] = (player == 1) ? 1 : -1;
				
				int Value = EvalMove(-player, depth + 1, maxDepth, c, alpha, beta);

				// Un-do the Move...
				Board[Gameplay.ColHeights[c], c] = 0;

				if(player == -1 && Value < Min) {
					//Debug.Log ("Min = Value, Depth = " + depth.ToString());
					Min = Value;
					//Debug.Log(c.ToString() + " COLUMN");
					if(depth == 0) Column = c;
				}

				if(player == 1 && Value > Max) {
					//Debug.Log ("Max = Value, Depth = " + depth.ToString());
					Max = Value;
					if(depth == 0) Column = c;
				}
				
				if(player == 1) {
					if (Value < beta) beta = Value;
					if (alpha >= beta) return beta;
				}
				else {
					if (Value > alpha) alpha = Value;
					if (alpha >= beta) return alpha;
				}

			} // End if (!Gameplay.ColsFull[c])

		} // End for loop

		if(player == -1) {
			if (Min == unchecked((int) Mathf.Infinity)) return 0;
			//Debug.Log(Min);
			return Min;
		}
		else {
			if (Max == unchecked((int) -Mathf.Infinity)) return 0;
			//Debug.Log(Max);
			return Max;
		}

	} // End EvalMove()

	public int EvalScore (int player, int column, int depth) {

		int Score = 0;
		int Row = Gameplay.ColHeights[column] + 1;
		int AICount, PlayerCount;
		AIWin = PlayerWin = false;
		
		// Check Horizontal
		AICount = PlayerCount = 0;

		int[] BoardRow = new int[Gameplay.BOARD_SIZE[1]];
		for(int i = 0; i < Board.GetLength(0); i++) { 
			for(int n = 0; n < Board.GetLength(1); n++) { 
				if(n == column) BoardRow[n] = Board[i, n];
			}
		}

		int CStart = column - 3;
		int ColStart = CStart >= 0 ? CStart : 0;
		int ColEnd = Gameplay.BOARD_SIZE [1] - 3 - (ColStart - CStart);

		for (int c = ColStart; c < ColEnd; c++) {

			AICount = PlayerCount = 0;

			for (int val = 0; val < 4; val++) {

				int mark = BoardRow[c + val];

				if (mark == 1) {
					PlayerCount++;
				}
				else if (mark == -1) {
					AICount++;
				}

			}
			if (PlayerCount == 4) {
				PlayerWin = true;
				if (depth <= 2) return MAX_INT;
			}
			else if (AICount == 4) {
				AIWin = true;
				if (depth <= 2) return MIN_INT;
			}

			Score += IncrementScore(PlayerCount, AICount, player);
		}
		
		// Check Vertical
		AICount = PlayerCount = 0;
		int RowEnd = Mathf.Min(Gameplay.BOARD_SIZE[0], Row + 4);
		for(int r = Row; r < RowEnd; r++) {
			int mark = Board[r, column];

			if(mark == 1) {
				PlayerCount++;
			}
			else if(mark == -1) {
				AICount++;
			}

			if (AICount == 4) {
				AIWin = true;
				if (depth <= 2) return MIN_INT;
			}
			else if (PlayerCount == 4) {
				PlayerWin = true;
				if (depth <= 2) return MAX_INT;
			}
		
			Score += IncrementScore(PlayerCount, AICount, player);
		}

	
		// Check Right Diagonal
		int MinValue = Mathf.Min(Row, column);
		int RowStart = Row - MinValue;
		ColStart = column - MinValue;

		for (int r = RowStart, c = ColStart; r <= Gameplay.BOARD_SIZE[0] - 4 && c <= Gameplay.BOARD_SIZE[1] - 4; r++, c++) {

			AICount = PlayerCount = 0;
			for (int val = 0; val < 4; val++) {
				int mark = Board[r + val, c + val];
				if (mark == 1) {
					PlayerCount++;
				}
				else if (mark == -1) {
					AICount++;
				}
			}

			if (PlayerCount == 4) {
				PlayerWin = true;
				if (depth <= 2) return MAX_INT;
			}
			else if (AICount == 4) {
				AIWin = true;
				if (depth <= 2) return MIN_INT;
			}

			Score += IncrementScore(PlayerCount, AICount, player);
		}
		
		// Check Left Diagonal
		MinValue = Mathf.Min(Gameplay.BOARD_SIZE[0] - 1 - Row, column);
		RowStart = Row    + MinValue;
		ColStart = column - MinValue;

		for(int r = RowStart, c = ColStart; r >= 3 && c <= Gameplay.BOARD_SIZE[1] - 4; r--, c++) {

			AICount = PlayerCount = 0;

			for(int val = 0; val < 4; val++) {

				int mark = Board[r - val, c + val];
				if (mark == 1) {
					PlayerCount++;
				} else if (mark == -1) {
					AICount++;
				}
			}
			if (AICount == 4) {
				AIWin = true;
				if (depth <= 2) return MIN_INT;
			} else if (PlayerCount == 4) {
				PlayerWin = true;
				if (depth <= 2) return MAX_INT;
			}

			Score += IncrementScore(PlayerCount, AICount, player);
		}

		return Score;
	}

	private int IncrementScore(int PlayerCount, int AICount, int player) {

		if(PlayerCount == AICount) {
			if (player == -1) return -1;
			return 1;
		}
		else if (AICount < PlayerCount) {
			if (player == -1) return INCREMENT[PlayerCount] - INCREMENT[AICount];
			return INCREMENT[PlayerCount + 1] - INCREMENT[AICount];
		}
		else {
			if (player == -1) return -INCREMENT[AICount + 1] + INCREMENT[PlayerCount];
			return -INCREMENT[AICount] + INCREMENT[PlayerCount];
		}
	}


}
