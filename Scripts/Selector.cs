using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Selector : MonoBehaviour
{
     float speed = 5f;
	 public bool IsMoving{get; private set;}
	 public bool HasSelected{get; set;}
	 
	 public bool IsMovingTo{get; set;}

	 public GameObject selectedUnit;
	 public GameObject hoveredUnit;
	 public GameObject prefab;
	 [SerializeField] GameObject statDetailMenu;
	 
	 [SerializeField] Text statDetailHp;
	 [SerializeField] Text statDetailAtk;
	 [SerializeField] Text statDetailDef;
	 [SerializeField] Text statDetailSpd;
	 [SerializeField] Text statDetailSor;
	 [SerializeField] Text statDetailProf;
	 [SerializeField] Text statDetailImm;
     [SerializeField] Text statDetailLv;	
	 [SerializeField] public Image unitImage;
	 [SerializeField] public Text unitName;
	
	 Vector3 input;
	 [SerializeField] Camera camera;
	 int z = 0;
	 int m = 0;
	 string previousUnitName;
	 void Awake() {
		
		 HasSelected = false;
		 IsMovingTo = false;
	 }
	 
	 public void enemyPhaseMovingSelector() {
		input.x = Input.GetAxisRaw("Horizontal");
		input.y = Input.GetAxisRaw("Vertical");
		 if(!IsMoving)
		 {
			StartCoroutine(MoveSelector(input));
		 }
	 }
			
		public void movingSelector() {
			Collider2D col;
			Collider2D col2;
			
			input.x = Input.GetAxisRaw("Horizontal");
		    input.y = Input.GetAxisRaw("Vertical");
			 if(!IsMoving)
			 {
				StartCoroutine(MoveSelector(input));
			 }

			if (Input.GetKeyDown(KeyCode.E) && HasSelected == false)
			{			
				  col = Physics2D.OverlapPoint(transform.position, Layers.i.UserUnitActionLayer);
				  if(col != null && !col.gameObject.GetComponent<Unit>().HasMoved) {
					  if (col.gameObject.tag == "Player" ) {
						  selectedUnit = col.gameObject;
						  HasSelected = true;
						  Debug.Log ("Selected " + col.gameObject.name);
					  }
				  }
			}
			
			col2 = Physics2D.OverlapPoint(transform.position, Layers.i.UnitLayer | Layers.i.UserUnitActionLayer);
			
			if(col2 != null) {
				hoveredUnit = col2.gameObject; 
				Unit u = hoveredUnit.gameObject.GetComponent<Unit>();
					if(z == 0) {
						statDetailMenu.SetActive(true);
						
						statDetailHp.text = "HP: "+ u.Hp.ToString();
						statDetailAtk.text = "Atk: "+ u.Atk.ToString();
						statDetailDef.text = "Def: "+ u.Def.ToString();
						statDetailSpd.text = "Spd: "+ u.Spd.ToString();
						statDetailSor.text = "Sor: "+ u.Sor.ToString();
						statDetailProf.text = "Prof: "+ u.Prof.ToString();
						statDetailImm.text = "Imm: "+ u.Imm.ToString();
						statDetailLv.text = "Lv: "+ u.Level.ToString();
						unitName.text = u.GetComponent<Unit>().unitName;
						previousUnitName =  u.GetComponent<Unit>().unitName;
						
						prefab = Instantiate(hoveredUnit.gameObject, statDetailMenu.transform.position + new Vector3(1f,-0.5f,0), Quaternion.identity) as GameObject;
						prefab.transform.parent = GameObject.Find("CharStatDetails").transform;
						Destroy(prefab.GetComponent<BoxCollider2D>());
						Destroy(prefab.GetComponent<Rigidbody2D>());
						prefab.gameObject.layer = LayerMask.NameToLayer("UI");
						prefab.gameObject.tag = "Untagged"; 
						SpriteRenderer[] s = prefab.GetComponentsInChildren<SpriteRenderer>();
						for(int j = 0; j < s.Length; j++) {
							s[j].sortingLayerID = SortingLayer.NameToID("UI");
							s[j].sortingOrder = 1;
						}
						z = 1;
					}
					if(u.GetComponent<Unit>().unitName != previousUnitName){
						z = 0;
						Destroy(prefab);
					}
			} 
			else {
				z = 0;
				Destroy(prefab);	
				statDetailMenu.SetActive(false);
			}
		}
		
		public GameObject getSelectedUnit() {
			return selectedUnit;
		}
		
		public IEnumerator MoveSelector(Vector2 moveVec)
		{
			var targetpos = transform.position;
			targetpos.x += moveVec.x;
			targetpos.y += moveVec.y;
			
		    IsMoving = true; 

			while ((targetpos - transform.position).sqrMagnitude > Mathf.Epsilon)
			{

				transform.position = Vector3.MoveTowards(transform.position, targetpos, speed * Time.deltaTime);
				yield return null;

			}
			IsMoving = false;   

			transform.position = targetpos;
			
    }
	

	
}
