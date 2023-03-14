using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveFinder : MonoBehaviour
{
	public Board board;
	[SerializeField] bool isEnemy = false;
	
	
	public void Awake() {
		board = GetComponent<Board>();
	}
	
    public List<Node> FindMove(Vector3 start, Vector3 end) {
		Node sNode = board.getNode(start);
		Node eNode = board.getNode(end);
		
		List<Node> openSet = new List<Node>();
		HashSet<Node> closedSet = new HashSet<Node>();

		openSet.Add(sNode);
		
		while (openSet.Count > 0) {
			Node current = openSet[0];

			for(int i = 1; i< openSet.Count; i++) {
				if(openSet[i].getfCost < current.getfCost || openSet[i].getfCost == current.getfCost && openSet[i].hCost < current.hCost) {
					current = openSet[i];
				}
			}
			openSet.Remove(current);
			closedSet.Add(current);
			
			if(current == eNode) {
				return RetraceMove(sNode,eNode);
				
			}
			foreach(Node adjacent in board.getAdjacentNodes(current)) {
				if(!adjacent.isWalkable || closedSet.Contains(adjacent)) {
					continue;
				}
				
				int moveCostToAdjacent = current.gCost + Distance(current, adjacent);
				if(moveCostToAdjacent < adjacent.gCost || !openSet.Contains(adjacent)) {
					adjacent.gCost = moveCostToAdjacent;
					adjacent.hCost = Distance(current,eNode);
					adjacent.parent = current;
					
					if(!openSet.Contains(adjacent)) {
						openSet.Add(adjacent);
					}
				}
			}
		}
		
		return new List<Node>();

	}
	
	 List<Node> RetraceMove(Node sNode,Node eNode) {
		List<Node> moves = new List<Node>();
		Node current = eNode;
		
		while(current != sNode) {
			moves.Add(current);
			current = current.parent;
		}
		
		moves.Reverse();
		if(isEnemy)
			moves.RemoveAt(moves.Count - 1);	

		return moves;
	}
	
	int Distance(Node n1,Node n2) {
		int distanceX = Mathf.Abs((int)n1.pos.x - (int)n2.pos.x);
		int distanceY = Mathf.Abs((int)n1.pos.y - (int)n2.pos.y);

		if(distanceX > distanceY)
			return 14*distanceY + 10*(distanceX - distanceY);
		
			
		return 14*distanceX + 10*(distanceY - distanceX);

	}
	
	
}
