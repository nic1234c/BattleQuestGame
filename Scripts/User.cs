using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public class User : MonoBehaviour
{
	
	[SerializeField] public List<Unit> units;
    private Vector2 input;
	public event Action EndTurn;
	[SerializeField] public Selector selector;
	[SerializeField] public MovementArea ma;
	UserState state;
	List<GameObject> prevMoveAreaList;
	public MoveFinder mf;
	List<Node> ml;
	int m = 0;
	bool cm;
	Battle b;
	AudioSource audioSource;
	AudioClips audioClips;
	
   void Awake()
   {
	  prevMoveAreaList =  new List<GameObject>();
	  state = UserState.NotSelectedUnit;
	  ml = new List<Node>();
	  mf = GetComponent<MoveFinder>();
	  audioSource = GetComponentInParent<AudioSource>();
	  audioClips = GetComponentInParent<AudioClips>();
	  cm = false;
	  b = GetComponentInParent<Battle>();

   }

    // Update is called once per frame
    public void Update()
    {	
		 if(Input.GetKeyDown(KeyCode.V))
		 {
			EndTurn();
		 } 	
		 if(b.state == State.EnemyTurn ||b.state == State.EnemyBattling) {
			selector.enemyPhaseMovingSelector();
		 }
    }
	
	public void UserTurn() {
		Collider2D col;
		Collider2D col2;

		selector.movingSelector();	
		var u = selector.getSelectedUnit().GetComponent<Unit>();
		
		if(selector.HasSelected && !u.HasMoved && !ma.IsShown  && state == UserState.NotSelectedUnit) {
			state = UserState.SelectedUnit;
			mf = GetComponent<MoveFinder>();
			mf.board.createBoard();
			ma.getMoveArea(u,mf);	
			prevMoveAreaList = ma.obj;
		} 
		else if (Input.GetKeyDown(KeyCode.Q) && state == UserState.SelectedUnit) {
			deselect();	
		}
		else if (Input.GetKeyDown(KeyCode.E) && state == UserState.SelectedUnit) {
			col = Physics2D.OverlapPoint (selector.transform.position, Layers.i.MoveSquareLayer);
			col2 = Physics2D.OverlapPoint (selector.transform.position, Layers.i.UserUnitActionLayer);
			
			if(col && !col2) {
				cm = true;
				selector.IsMovingTo = true;
				selector.HasSelected = false;
				ma.IsShown = false;
				ml = mf.FindMove(u.previousPosition,selector.transform.position);
				state = UserState.Moving;
				audioSource.clip = audioClips.GetClip("Press");
				audioSource.Play ();
			} else {
				deselect();	
			}
		}
		else if(state == UserState.Moving && cm) {
			 move(u,ml);
			 u.HandleAnimationUpdate();
		}
		else if(!(ml.Count == 0) && state == UserState.Moving) {
			EndMove(ml,u);
		}
		else if(Input.GetKeyDown(KeyCode.Z) && !u.HasAttacked){
			if(u.Weapons[0].Type == WeaponType.Heal) {
				u.playerHeal();
			}
			else {
				u.playerAttack();
			}
		}
		else if(Input.GetKeyDown(KeyCode.X) && !u.HasAttacked){
			u.showWeaponsMenu(); 
		}
		else if(Input.GetKeyDown(KeyCode.T) && !u.HasAttacked){
			u.PlayerTrade(); 
		}
	}
	
	public void move(Unit u,List<Node> moveList) {
			Vector3 dirToTarget = moveList[m].pos-u.transform.position;	
			u.IsMoving = true;  
			if (!audioSource.isPlaying) {
				audioSource.clip = audioClips.GetClip("Move");
				audioSource.Play ();
			}
			if( Mathf.Abs( dirToTarget.x )<Mathf.Abs( dirToTarget.y ) && Mathf.Abs( dirToTarget.x )>Mathf.Epsilon ) {
				dirToTarget.y = 0f;
			}
			else if( Mathf.Abs( dirToTarget.y )>Mathf.Epsilon ) {
				dirToTarget.x = 0f;
			}
			
			
			u.transform.position = Vector3.MoveTowards(
							u.transform.position,
							u.transform.position+dirToTarget,
							u.Speed  * Time.deltaTime 
			);

			if((u.transform.position - moveList[m].pos).sqrMagnitude == 0) {	
				selector.IsMovingTo = false;
				u.IsMoving = false;  
				ma.hideMoveArea();
				EndMove(moveList,u);
			}
	}
	
	private void deselect() {
		selector.HasSelected = false;
		ma.IsShown = false;
		ma.hideMoveArea();
		state = UserState.NotSelectedUnit;	
	}
	
	public void reviveUnits() {
		for(int i = 0; i < units.Count; i++)
			units[i].gameObject.SetActive(true);
	}
	
	private void EndMove(List<Node> moveList,Unit u)
	{	
		if(m < moveList.Count - 1) {
			++m;
			m = Mathf.Clamp(m,0,moveList.Count - 1);
		}
		else {
			cm = false;
			u.HasMoved = true;
			m = 0;
			u.setPreviousPos();
			state = UserState.NotSelectedUnit;

		}
	}

	public void initAttacking() {
		AllowMove();
		m = 0;
		for(int i = 0; i < units.Count; i++)
			units[i].initAttacking();
	}
	
	public void disableAttacking() {
		SetToUnit();
		for(int i = 0; i < units.Count; i++)
			units[i].disableAttacking();
	}
	
	public bool isDefeated() {
		bool isDefeated =  GameObject.FindGameObjectsWithTag ("Player").Length == 0;
		
		return isDefeated;
	}
	
	public void AllowMove() {
		for(int i = 0; i < units.Count; i++)
			units[i].gameObject.layer = LayerMask.NameToLayer("UserUnitAction");
	}
	
	public void SetToUnit() {
		for(int i = 0; i < units.Count; i++)
			units[i].gameObject.layer = LayerMask.NameToLayer("Unit");
	}
}

public enum UserState {NotSelectedUnit, SelectedUnit, Moving}
