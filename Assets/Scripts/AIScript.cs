using UnityEngine;
using System.Collections;

public class AIScript : MonoBehaviour {

	public int BeginMiniMax(int depth, int[,] board){

		Node root = new Node(null, -1, 1, CopyBoard(board), depth);
		Node node = AlphaBetaMinimax (root, new Node (), new Node (), depth, true);

		//Debug.Log (node.score);
		//Debug.Log (node.column);

		printBoard (node.board);
		return node.column;

	} // End BeginMiniMax()


	public void printBoard (int[,] board) {

		string str = "";
		for(int i = Gameplay.BOARD_SIZE[0] - 1; i >= 0; i--) {
			
			for(int n = 0; n < Gameplay.BOARD_SIZE[1]; n++) {
				str += board[i, n].ToString() + " | ";
			}

			str += "\n";
			
		} // End outer for loop

		Debug.Log (str);

	}



	/**
	 * Copies the Board, since 'board' is passed by reference.
	 */
	public int[,] CopyBoard (int[,] board) { 

		int[,] newBoard = new int[Gameplay.BOARD_SIZE[0], Gameplay.BOARD_SIZE[1]];

		for(int i = 0; i < Gameplay.BOARD_SIZE[0]; i++) {
			
			for(int n = 0; n < Gameplay.BOARD_SIZE[1]; n++) newBoard[i, n] = board[i, n];
			
		} // End outer for loop

		return newBoard;
	
	} // End CopyBoard()

	
	public Node AlphaBetaMinimax(Node node, Node alpha, Node beta, int depth, bool maxPlayer) {

		int win = Gameplay.CheckWin (node.board);
		//Debug.Log (win.ToString() + " WIN");

		if(depth == 0 || win != 0) {

			if(win == -1) {
				node.score = 1000000;
				return node;
			}
			else {
				return node.Eval();
			}

		}

		if(maxPlayer) {

			for(int i = 0; i < Gameplay.BOARD_SIZE[1]; i++) {
				if(Gameplay.ColsFull[i] == false){
					Node newNode = new Node(node, i, -1, CopyBoard(node.board), depth--);
					node.AddChild(newNode);
				}

			}

			for(int i = 0; i < node.children.Count; i++) {

				alpha = Max(alpha, AlphaBetaMinimax(node.children[i] as Node, alpha, beta, depth, false));
				if(beta.score <= alpha.score) break;
			}

			return alpha;
		}
		else{ // Not the maxPlayer

			for(int i = 0; i < node.children.Count; i++){
				beta = Min(beta, AlphaBetaMinimax(node.children[i] as Node, alpha, beta, depth, false));
				if(beta.score <= alpha.score) break;
			}
			return beta;

		} // End if(maxPlayer)
		
	} // End AlphaBetaMinimax()


	Node Max(Node alphaBeta, Node node){
		return (alphaBeta.score >= node.score) ? alphaBeta : node;
	}
	
	Node Min(Node alphaBeta, Node node){
		return (alphaBeta.score <= node.score) ? alphaBeta : node;
	}
	

	public class Node {

		// The Column Chosen by the Player
		public int column;

		// The Current State of the Board
		public int[,] board;

		// The player, -1 Min, 1 Max
		public int player;

		// The parent node
		public Node parent;

		// The children nodes
		public ArrayList children = new ArrayList();

		// The Minimax Score
		public int score;

		// The depth
		public int depth;

		public Node() { }
		
		public Node(Node parent, int column, int player, int[,] board, int depth){
			this.parent 	= parent;
			this.column 	= column;
			this.player 	= player;
			this.board 		= board;
			this.depth 		= depth;

			if(column != -1) {
				AddChecker(column);
			}
		}

		public void AddChecker(int inSlot){

			int i = 5;
			while(this.board[i, inSlot] == 0 && i != 0) {
				i--;
			}

			if(player == -1)
				board[i, inSlot] = -1;
			else	
				board[i, inSlot] = 1;

			//Debug.Log (board[i, inSlot].ToString() + " MOVE MADE IN SLOT " + i.ToString() + " - " + inSlot.ToString());
		}
		
		public void AddChild(Node child) { children.Add(child); }

		public Node Eval() {
			this.score = 10;
			return this;
		}

	} // End Node Class

	
}

 
