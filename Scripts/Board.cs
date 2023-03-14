using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Board : MonoBehaviour
{
	public Node[,] board;
	[SerializeField] public int rows = 13;
	[SerializeField] public int cols = 9;
	
    // Start is called before the first frame update
    void Start()
    {
	   createBoard();
    }
	
	public void createBoard() {
		Collider2D col;

		board = new Node[rows,cols];		
		for(int i = 0; i < rows ; i++) {
			for(int j = 0; j < cols; j++) {

			  col = Physics2D.OverlapPoint (new Vector3(i,j,0), Layers.i.UnitLayer | Layers.i.ObjectsLayer);
			  board[i,j] = new Node(!col, new Vector3(i,j,0));
			}
		}
	}
	
	public Node getNode(Vector3 pos) {
		int posX = (int)pos.x;
		int posY = (int)pos.y;
		
		return board[posX,posY];
		
		
	}
	
	public List<Node> getAdjacentNodes(Node n) {
		List<Node> aNodes = new List<Node>();
		
		for(int x = -1; x <= 1; x++) {
			for(int y = -1; y <= 1; y++) {
				if(x == 0 && y == 0 || Diagonals(x,y)) 
					continue;
				
				int reviewX = (int)n.pos.x + x;
				int reviewY = (int)n.pos.y + y;
				
				if(reviewX >= 0 && reviewX < rows && reviewY >= 0 && reviewY < cols ) {		
					aNodes.Add(board[reviewX,reviewY]);
				}
			}
		}
		
		return aNodes;
	}
	
	public void drawBoard() {
				
		for(int i = 0; i < rows ; i++) {
			for(int j = 0; j < cols; j++) {
				Debug.DrawLine(Vector3.zero,board[i,j].pos, Color.black);			 
			}
		}
	}
	
	public bool Diagonals(int x,int y){
		return x == -1 && y== -1 || x == 1 && y == 1 || x == 1 && y == -1 || x == -1 && y == 1;
	}
}
