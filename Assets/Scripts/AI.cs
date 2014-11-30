using UnityEngine;
using System.Collections;

public class AI {

	private int[,] board;
	private int column, boardsAnalyzed, maxDepth;
	private bool redWinFound, yellowWinFound;
	public static int[] INCREMENT = {0, 1, 4, 32, 128, 512}; 
	
	public AI(int[,] board, int maxDepth) {
		this.board = board;
		this.boardsAnalyzed = 0;
		this.maxDepth = maxDepth;
		Debug.Log (maxDepth);
	}
	
	public int getBoardsAnalyzed() {
		return boardsAnalyzed;
	}
	
	public int alphaBeta(int player) {

		Debug.Log (player);

		redWinFound = yellowWinFound = false;

		if (player == -1) {

			evaluateYellowMove(0, 1, -1, unchecked((int) -Mathf.Infinity + 1), unchecked((int) Mathf.Infinity - 1));
			if (yellowWinFound) return column;

			redWinFound = yellowWinFound = false;
			evaluateRedMove(0, 1, -1, unchecked((int) -Mathf.Infinity + 1), unchecked((int) Mathf.Infinity - 1));
			if (redWinFound) return column;

			evaluateYellowMove(0, maxDepth, -1, unchecked((int) -Mathf.Infinity + 1), unchecked((int) Mathf.Infinity - 1));
			if (yellowWinFound) return column;

		} else {

			evaluateRedMove(0, 1, -1, unchecked((int) -Mathf.Infinity + 1), unchecked((int) Mathf.Infinity - 1));
			if (redWinFound) return column;
			
			evaluateYellowMove(0, 1, -1, unchecked((int) -Mathf.Infinity + 1), unchecked((int) Mathf.Infinity - 1));
			if (yellowWinFound) return column;

			evaluateRedMove(0, maxDepth, -1, unchecked((int) -Mathf.Infinity + 1), unchecked((int) Mathf.Infinity - 1));
			if (redWinFound) return column;
		}

		return column;
	}
	
	private int evaluateRedMove(int depth, int maxDepth, int col, int alpha, int beta) {
		Debug.Log ("RED");
		boardsAnalyzed++;

		int min = unchecked((int)Mathf.Infinity), score = 0;

		if (col != -1) {
			score = 10; //getHeuristicScore(-1, col, depth, maxDepth);
			if(Gameplay.CheckWin(board) == -1) {
				yellowWinFound = true;
				return score;
			}
		}

		if (depth == maxDepth) {
			return score;
		}

		for (int c = 0; c < Gameplay.BOARD_SIZE[1]; c++) {

			if (!Gameplay.ColsFull[c]) {

				board[Gameplay.ColHeights[c], c] = 1;

				int value = evaluateYellowMove(depth + 1, maxDepth, c, alpha, beta);

				board[Gameplay.ColHeights[c], c] = 0;

				if (value < min) {
					min = value;
					if (depth == 0) column = c;
				}

				if (value < beta) beta = value;
				if (alpha >= beta) return beta;
			}
		}
		
		if (min == Mathf.Infinity) return 0;
		return min;
	}
	
	private int evaluateYellowMove(int depth, int maxDepth, int col, int alpha, int beta) {
		Debug.Log ("Yellow");
		boardsAnalyzed++;

		int max = unchecked((int) Mathf.Infinity), score = 0;
		if (col != -1) {
			score = 10; //getHeuristicScore(1, col, depth, maxDepth);
			if (Gameplay.CheckWin(board) == 1) {
				redWinFound = true;
				return score;
			}
		}
		if (depth == maxDepth) {
			return score;
		}

		for (int c = 0; c < Gameplay.BOARD_SIZE[1]; c++) {

			if (!Gameplay.ColsFull[c]) {

				board[Gameplay.ColHeights[c], c] = -1;

				int value = evaluateRedMove(depth + 1, maxDepth, c, alpha, beta);

				board[Gameplay.ColHeights[c], c] = 0;

				if (value > max) {
					max = value;
					if (depth == 0) column = c;
				}

				if (value > alpha) alpha = value;
				if (alpha >= beta) return alpha;
			}
		}

		if (max == Mathf.Infinity) return 0;
		return max;
	}

	public int getHeuristicScore(int player, int col, int depth, int maxDepth) {

		int score = 0,
		row = Gameplay.ColHeights[col] + 1,
		redCount, yellowCount;
		redWinFound = yellowWinFound = false;
		
		///////////////////////////////////////////////////////////////////////
		// Check row
		///////////////////////////////////////////////////////////////////////
		redCount = yellowCount = 0;
		int[] boardRow = new int[Gameplay.BOARD_SIZE[1]];

		for(int i = 0; i < Gameplay.BOARD_SIZE[0]; i++) {
			for(int n = 0; n < Gameplay.BOARD_SIZE[0]; n++) {
				if(i == row) boardRow[n] = board[i, n];
			}
		}

		int cStart = col - 3,
		colStart = cStart >= 0 ? cStart : 0,        
		colEnd = Gameplay.BOARD_SIZE[1] - 3 - (colStart - cStart);
		for (int c = colStart; c < colEnd; c++) {
			redCount = yellowCount = 0;
			for (int val = 0; val < 4; val++) {
				int mark = boardRow[c + val];
				if (mark == 1) {
					redCount++;
				} else if (mark == -1) {
					yellowCount++;
				}
			}
			if (redCount == 4) {
				redWinFound = true;
				if (depth <= 2) {
					return unchecked((int) -Mathf.Infinity + 1);
				}
			} else if (yellowCount == 4) {
				yellowWinFound = true;
				if (depth <= 2) {
					return unchecked((int) Mathf.Infinity - 1);
				}
			}
			score += getScoreIncrement(redCount, yellowCount, player);
		}
		
		///////////////////////////////////////////////////////////////////////
		// Check column
		///////////////////////////////////////////////////////////////////////
		redCount = yellowCount = 0;
		int rowEnd = Mathf.Min(Gameplay.BOARD_SIZE[0], row + 4);
		for (int r = row; r < rowEnd; r++) {
			int mark = board[r, col];
			if (mark == 1) {
				redCount++;
			} else if (mark == -1) {
				yellowCount++;
			}            
		}
		if (redCount == 4) {
			redWinFound = true;
			if (depth <= 2) {
				return unchecked((int) -Mathf.Infinity + 1);
			}
		} else if (yellowCount == 4) {
			yellowWinFound = true;
			if (depth <= 2) {
				return unchecked((int) Mathf.Infinity - 1) ;
			}
		}
		score += getScoreIncrement(redCount, yellowCount, player);
		
		///////////////////////////////////////////////////////////////////////
		// Check major diagonal
		///////////////////////////////////////////////////////////////////////
		int minValue = Mathf.Min(row, col),
		rowStart = row - minValue;
		colStart = col - minValue;
		for (int r = rowStart, c = colStart; r <= Gameplay.BOARD_SIZE[0] - 4 && c <= Gameplay.BOARD_SIZE[1] - 4; r++, c++) {
			redCount = yellowCount = 0;
			for (int val = 0; val < 4; val++) {
				int mark = board[r + val, c + val];
				if (mark == 1) {
					redCount++;
				} else if (mark == -1) {
					yellowCount++;
				}
			}
			if (redCount == 4) {
				redWinFound = true;
				if (depth <= 2) {
					return unchecked((int) -Mathf.Infinity + 1);
				}
			} else if (yellowCount == 4) {
				yellowWinFound = true;
				if (depth <= 2) {
					return unchecked((int) Mathf.Infinity - 1);
				}
			}
			score += getScoreIncrement(redCount, yellowCount, player);
		}
		
		///////////////////////////////////////////////////////////////////////
		// Check minor diagonal
		///////////////////////////////////////////////////////////////////////
		minValue = Mathf.Min(Gameplay.BOARD_SIZE[0] - 1 - row, col);
		rowStart = row + minValue;
		colStart = col - minValue;
		for (int r = rowStart, c = colStart; r >= 3 && c <= Gameplay.BOARD_SIZE[1] - 4; r--, c++) {
			redCount = yellowCount = 0;
			for (int val = 0; val < 4; val++) {
				int mark = board[r - val, c + val];
				if (mark == 1) {
					redCount++;
				} else if (mark == -1) {
					yellowCount++;
				}
			}
			if (redCount == 4) {
				redWinFound = true;
				if (depth <= 2) {
					return unchecked((int) -Mathf.Infinity + 1);
				}
			} else if (yellowCount == 4) {
				yellowWinFound = true;
				if (depth <= 2) {
					return unchecked((int) Mathf.Infinity - 1);
				}
			}
			score += getScoreIncrement(redCount, yellowCount, player);
		}
		return score;
	}


	private int getScoreIncrement(int redCount, int yellowCount, int player) {
		if (redCount == yellowCount) {
			if (player == 1) {
				return -1;
			}
			return 1;
		} else if (redCount < yellowCount) {
			if (player == 1) {
				return INCREMENT[yellowCount] - INCREMENT[redCount];
			}
			return INCREMENT[yellowCount + 1] - INCREMENT[redCount];
		} else {
			if (player == 1) {
				return -INCREMENT[redCount + 1] + INCREMENT[yellowCount];
			}
			return -INCREMENT[redCount] + INCREMENT[yellowCount];
		}
	}
}
