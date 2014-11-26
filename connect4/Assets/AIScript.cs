using UnityEngine;
using System.Collections;

public class AIScript : MonoBehaviour {

	public Columns[] boardRow = new Columns[7];
	public KeyCode check;
	public static AIScript script;

	// Use this for initialization
	void Awake () {
		script = this;
		for(int i = 0; i < 7; i++){
			//Columns column = board[i];
			for(int j = 0; j < 8; j++){
				boardRow[i].column[j] = 0;
			}
		}
		
	}
	
	// Update is called once per frame
	void Update () {
		if(Input.GetKeyDown(check)){
			print(BeginMiniMax(3, boardRow)); 		
		}
	}	
	
	public int BeginMiniMax(int newDepth, Columns[] board){
		Node root = new Node();
		root.Initialize(null, 0, "max", CopyBoard(board), newDepth);
		Node bestNode = AlphaBetaMinimax(root, new Node(), new Node(), newDepth, true);
		int bestSlot = bestNode.score;
		return bestSlot;
	}
	
	public Node AlphaBetaMinimax(Node node, Node alpha, Node beta, int depth, bool maxPlayer){
		// check if at search bound
		bool win = CheckWin(node.boardState);
		if(depth == 0 || win){
			if(win){
				node.score = 1000000;
				return node;
			}
			else
				return node.Eval();
		}
		if(maxPlayer){
			for(int i = 1; i < 8; i++){
				if(node.boardState[6].column[i] == 0){
					Node newNode = new Node();
					newNode.Initialize(node, i, "min", CopyBoard(node.boardState), depth--); 
					node.AddToChildren(newNode);
				}
			}
			for(int i = 0; i < node.children.Count; i++){
				alpha = Max(alpha, AlphaBetaMinimax(node.children[i] as Node, alpha, beta, depth, false));
				if(beta.score <= alpha.score)
					break;
			}
			return alpha;
		}
		else{
			for(int i = 0; i < node.children.Count; i++){
				beta = Min(beta, AlphaBetaMinimax(node.children[i] as Node, alpha, beta, depth, false));
				if(beta.score <= alpha.score)
					break;
			}
			return beta;
		}
		
	}
	
	Node Max(Node alphaBeta, Node node){
		if(alphaBeta.score >= node.score)
			return alphaBeta;
		else	
			return node;
	}
	
	Node Min(Node alphaBeta, Node node){
		if(alphaBeta.score <= node.score)
			return alphaBeta;
		else
			return node;
	}
	



	
	
//-------------------Custom Classes----------------------------
	
	[System.Serializable]
	public class Node{
		public int slot;
		public Columns[] boardState;
		public string minmax;
		public Node parent;
		public ArrayList children = new ArrayList();
		public int score;
		
		public void Initialize(Node newParent, int newSlot, string newMinMax, Columns[] newBoard, int newDepth){
			parent = newParent;
			slot = newSlot;
			minmax = newMinMax;
			boardState = newBoard;
			if(slot != 0)
				AddChecker(slot);
		}
		
		public void AddChecker(int inSlot){
			int i = 6;
			while(boardState[i].column[inSlot] == 0){
				i--;
			}
			if(minmax == "min")
				boardState[i+1].column[inSlot] = 1;
			else	
				boardState[i+1].column[inSlot] = 2;
		}
		
		public void AddToChildren(Node child){
			children.Add(child as Node);
		}
		
		public Node Eval(){
			score = 10;
			return this;
		}
	}
	
	[System.Serializable]
	public class Columns
	{
	 public int[] column = new int[8];

	}
	
//---------------------Make Copy of Board-----------------

	public Columns[] CopyBoard(Columns[] board){
		Columns[] newBoard = new Columns[7];
		for(int i = 0; i < 7; i++){
			//Columns column = board[i];
			for(int j = 0; j < 8; j++){
				print("row :" + i + "  column : " + j + "  value : " + board[i].column[j]);
				newBoard[i].column[j] = board[i].column[j];
			}
		}
		return newBoard;
	}
	
	public Columns[] CopyBoard(int[,] board){
		Columns[] newBoard = new Columns[7];
		for(int i = 0; i < 7; i++){
			//Columns column = board[i];
			for(int j = 0; j < 8; j++){
				newBoard[i].column[j] = board[i,j];
			}
		}
		return newBoard;
	}
	
//---------------------Win Check---------------------------	
	
	public bool CheckWin(Columns[] row){
		int counter = 1;
		// Check Horizontal
		for(int i = 1; i < 7; i++){
			counter = 1;
			for(int j = 2; j < 8; j++){
				if(row[i].column[j] == row[i].column[j-1] && row[i].column[j] != 0){
					counter++;
				}
				else{
					counter = 1;
				}
				if(counter == 4){
					//print("win horizontal");
					return true;
				}
			}
		}
		counter = 1;
		// Check Vertical
		for(int i = 1; i < 8; i++){
			counter = 1;
			for(int j = 2; j < 7; j++){
				if(row[j].column[i] == row[j-1].column[i] && row[j].column[i] != 0){
					counter++;
				}
				else{
					counter = 1;
				}
				if(counter == 4){
					//print("win vertical");
					return true;
				}
			}
		}
		// Check Diagonals BL to TR
		counter = 1;
		for(int i = 2; i < 7; i++){
			if(row[i].column[i] == row[i-1].column[i-1] && row[i].column[i] != 0)
				counter++;
			else
				counter = 1;
			if(counter == 4){
				//print("win diagonal");
				return true;
			}
		}
		counter = 1;
		for(int i = 2; i < 7; i++){
			if(row[i].column[i+1] == row[i-1].column[i] && row[i].column[i+1] != 0)
				counter++;
			else
				counter = 1;
			if(counter == 4){
				//print("win diagonal");
				return true;
			}
		}
		counter = 1;
		for(int i = 2; i < 6; i++){
			if(row[i].column[i+2] == row[i-1].column[i+1] && row[i].column[i+2] != 0)
				counter++;
			else
				counter = 1;
			if(counter == 4){
				//print("win diagonal");
				return true;
			}
		}
		counter = 1;
		for(int i = 2; i < 5; i++){
			if(row[i].column[i+3] == row[i-1].column[i+2] && row[i].column[i+3] != 0)
				counter++;
			else
				counter = 1;
			if(counter == 4){
				//print("win diagonal");
				return true;
			}
		}
		counter = 1;
		for(int i = 2; i < 6; i++){
			if(row[i+1].column[i] == row[i].column[i-1] && row[i+1].column[i] != 0)
				counter++;
			else
				counter = 1;
			if(counter == 4){
				//print("win diagonal");
				return true;
			}
		}
		counter = 1;
		for(int i = 2; i < 5; i++){
			if(row[i+2].column[i] == row[i+1].column[i-1] && row[i+2].column[i] != 0)
				counter++;
			else
				counter = 1;
			if(counter == 4){
				//print("win diagonal");
				return true;
			}
		}
		// Check Diagonals TL to BR
				counter = 1;
		for(int i = 2; i < 7; i++){
			if(row[7-i].column[i] == row[7-i+1].column[i-1] && row[7-i].column[i] != 0)
				counter++;
			else
				counter = 1;
			if(counter == 4){
				//print("win diagonal");
				return true;
			}
		}
		counter = 1;
		for(int i = 2; i < 7; i++){
			if(row[6-i].column[i] == row[6-i+1].column[i-1] && row[6-i].column[i] != 0)
				counter++;
			else
				counter = 1;
			if(counter == 4){
				//print("win diagonal");
				return true;
			}
		}
		counter = 1;
		for(int i = 2; i < 6; i++){
			if(row[5-i].column[i] == row[5-i+1].column[i-1] && row[5-i].column[i] != 0)
				counter++;
			else
				counter = 1;
			if(counter == 4){
				//print("win diagonal");
				return true;
			}
		}counter = 1;
		for(int i = 2; i < 5; i++){
			if(row[7-i].column[i+1] == row[7-i+1].column[i] && row[7-i].column[i+1] != 0)
				counter++;
			else
				counter = 1;
			if(counter == 4){
				//print("win diagonal");
				return true;
			}
		}
		counter = 1;
		for(int i = 2; i < 6; i++){
			if(row[7-i].column[i+2] == row[7-i+1].column[i+1] && row[7-i].column[i+2] != 0)
				counter++;
			else
				counter = 1;
			if(counter == 4){
				//print("win diagonal");
				return true;
			}
		}
		counter = 1;
		for(int i = 2; i < 5; i++){
			if(row[7-i].column[i+3] == row[7-i+1].column[i+2] && row[7-i].column[i+3] != 0)
				counter++;
			else
				counter = 1;
			if(counter == 4){
				//print("win diagonal");
				return true;
			}
		}
		
		
		print("No winner");
		return false;
	}
	
}

 
