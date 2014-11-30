﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class AIScript : MonoBehaviour {

	public int BeginMiniMax(int depth, int[,] board){

		Debug.Log (depth);

		Node root = new Node(null, 0, 1, CopyBoard(board), depth);
		Node node = AlphaBetaMinimax (root, new Node(), new Node(), depth, true);

		//Debug.Log (node.board);
		Debug.Log ("Column " + node.column.ToString());

		//printBoard (node.board);
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

		if(depth == 0 || win == -1) {

			if(win == -1) {
				Debug.Log ("Returning Win!");
				node.score = 1000000;
				return node;
			}
			else {
				Debug.Log ("Returning Eval");
				return node.Eval();
			}

		}

		if(maxPlayer) {

			for(int i = 0; i < Gameplay.BOARD_SIZE[1]; i++) {
				Debug.Log ("FULL?: " + Gameplay.ColHeights[i].ToString());
				if(Gameplay.ColHeights[i] < Gameplay.BOARD_SIZE[1] - 1){
					Debug.Log ("HERE " + i.ToString ());
					Node newNode = new Node(node, i, -1, CopyBoard(node.board), depth--);
					Debug.Log (newNode.column + " >>> COLUMN");
					node.AddChild(newNode);
				}

			}
			//Debug.Log (node.children.Count.ToString() + " CHILD COUNT");
			for(int i = 0; i < node.children.Count; i++) {
				alpha = Max(alpha, AlphaBetaMinimax(node.children[i], alpha, beta, depth, false));
				Debug.Log ("ALPHA " + alpha.column.ToString());
				printBoard(alpha.board);
			}
			//Debug.Log ("Returning Alpha");
			return alpha;
		}
		else{ // Not the maxPlayer

			for(int i = 0; i < node.children.Count; i++){
				beta = Min(beta, AlphaBetaMinimax(node.children[i], alpha, beta, depth, false));
				Debug.Log ("BETA " + beta.column.ToString());
				printBoard(beta.board);
			}
			//Debug.Log ("Returning Beta");
			return beta;

		} // End if(maxPlayer)
		
	} // End AlphaBetaMinimax()


	Node Max(Node alphaBeta, Node node){
		Debug.Log ("AlphaBeta Score " + alphaBeta.score.ToString ());
		Debug.Log ("Node Score " + node.score.ToString ());
		if(alphaBeta.score >= node.score) {
			Debug.Log ("Returning alphaBeta");
			return alphaBeta;
		}
		else {
			Debug.Log ("Returning node");
			return node;
		}
	}
	
	Node Min(Node alphaBeta, Node node){
		if(alphaBeta.score <= node.score) {
			return alphaBeta;
		}
		else {
			return node;
		}
	}
	

	public class Node {

		// The Column Chosen by the Player
		public int column = 0;

		// The Current State of the Board
		public int[,] board = new int[,] { 
			{ 0, 0, 0, 0, 0, 0, 0 },
			{ 0, 0, 0, 0, 0, 0, 0 },
			{ 0, 0, 0, 0, 0, 0, 0 },
			{ 0, 0, 0, 0, 0, 0, 0 },
			{ 0, 0, 0, 0, 0, 0, 0 },
			{ 0, 0, 0, 0, 0, 0, 0 },
		};

		// The player, -1 Min, 1 Max
		public int player = 0;

		// The parent node
		public Node parent = null;

		// The children nodes
		public List<Node> children = new List<Node>();

		// The Minimax Score
		public int score;

		// The depth
		public int depth = 0;

		public Node() {
			this.score = (player == -1) ? unchecked((int) -Mathf.Infinity) : unchecked((int) Mathf.Infinity);
		}
		
		public Node(Node parent, int column, int player, int[,] board, int depth){
			this.parent 	= parent;
			this.column 	= column;
			this.player 	= player;
			this.board 		= board;
			this.depth 		= depth;

			AddChecker(column);

			this.score = (player == -1) ? unchecked((int) -Mathf.Infinity) : unchecked((int) Mathf.Infinity);
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
		
		public void AddChild(Node child) { this.children.Add(child); }

		public Node Eval() {
			this.score = Random.Range (1, 5000);
			return this;
		}

	} // End Node Class

	
}

 
