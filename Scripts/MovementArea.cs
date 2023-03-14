using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class MovementArea : MonoBehaviour
{
        [SerializeField] public GameObject tile;                               
        List<Vector3> moveArea;	
		public List<GameObject> obj;
		public bool IsShown{get; set;}

		public void Awake() {
			obj = new List<GameObject>();
		}

		public void getMoveArea(Unit u, MoveFinder mf) {	
				obj = new List<GameObject>();
				IsShown = false;
				Quaternion rotation = new Quaternion(0, 0, 0, 0);	
				for(int j = 0; j <= u.Movement; j++) {
				   for(int i = 0; i <= u.Movement; i++) {
							setMoveArea(i,j,u,mf);
							setMoveArea(-i,j,u,mf);
							setMoveArea(i,-j,u,mf);
							setMoveArea(-i,-j,u,mf);
					   }	
				}
				IsShown = true;
		}
		
		
		public void setMoveArea(int x,int y,Unit u,MoveFinder mf){
			Quaternion rotation = new Quaternion(0, 0, 0, 0);
			Vector3 v = u.transform.position + new Vector3(x,y,0);	

			
			if (v.x < mf.board.rows && v.y < mf.board.cols && v.x >= 0 && v.y >= 0) {
				List<Node> ml = mf.FindMove(u.transform.position,v);
				if(ml.Count <= u.Movement) {
					for(int z = 0; z < ml.Count; z++) {
						obj.Add(Instantiate(tile,ml[z].pos, rotation) as GameObject);
					}	
				}
			}
			
			
			
		}

		public void hideMoveArea() {
			for(int i = 0; i < obj.Count; i++)
				Destroy(obj[i]);
				
			obj.Clear();
		}
		
		
		
		
	
}
