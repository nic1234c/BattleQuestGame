using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Node
{
   public bool isWalkable;
   public Vector3 pos;
   
   public int gCost;
   public int hCost;
   public Node parent;
   
   public Node(bool isWalkable,Vector3 pos) {
		this.isWalkable = isWalkable;
		this.pos = pos;
		
   }
   
   public int getfCost {
		get {
			return gCost + hCost;
		}
   }
   
   
}
