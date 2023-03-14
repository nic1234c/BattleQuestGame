using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;
using UnityEngine.UI;

public class Enemy : MonoBehaviour
{
	
	[SerializeField] public List<Unit> units;
	public int currentSelection = 0;
	Unit target;  
	public event Action EndTurn;
	int m = 0;
	Unit unit;	
	[SerializeField] public Text numOfEnemiesLeft;
	AudioSource audioSource;
	AudioClips audioClips;

	public MoveFinder mf;
	List<Node> ml;
	EnemyState state;
	private void Awake()
	{
	    unit = GetComponent<Unit>();
		ml = new List<Node>();
		audioSource = GetComponentInParent<AudioSource>();
	    audioClips = GetComponentInParent<AudioClips>();
	}

   public void Start()
   {
		units = new List<Unit>();
		GameObject[] enemyUnits = GameObject.FindGameObjectsWithTag("Enemy");
		
		for(int j = 0; j < enemyUnits.Length; j++) {
			units.Add(enemyUnits[j].GetComponent<Unit>());
		}
	   
	   mf = GetComponent<MoveFinder>();
	   state = EnemyState.FindingMove;
   }
	
	void Update() {
		numOfEnemiesLeft.text =  numLeft().ToString();
	}
	public void EnemyTurn() {
			
			if(currentSelection == units.Count - 1 && units[currentSelection].HasMoved) {
				EndTurn();
			} 
			else {	
				if(state == EnemyState.FindingMove) {		
					
					if(units[currentSelection].gameObject.activeSelf  == true) {
						target = units[currentSelection].GetTarget();
						mf = GetComponent<MoveFinder>();
					    mf.board.createBoard();
						var targetPos = target.transform.position;
						ml = mf.FindMove(units[currentSelection].previousPosition,targetPos);
						state = EnemyState.Moving;
					}
					else {
						state = EnemyState.Idle;
					}
				}
				else if(!(ml.Count == 0) && state == EnemyState.Moving) {
					ml = ml.Take(units[currentSelection].Movement).ToList<Node>();
					moveEnemy(ml);
					units[currentSelection].HandleAnimationUpdate();
				}
				else if(state == EnemyState.Idle) {
					EnemyActive();
				} 
				else {
					state = EnemyState.Idle;
				}
			}
	}

	private void moveEnemy(List<Node> moveList) {
		Vector3 dirToTarget = moveList[m].pos-units[currentSelection].transform.position;	
		units[currentSelection].IsMoving = true;
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

		
		units[currentSelection].transform.position = Vector3.MoveTowards(
						 units[currentSelection].transform.position,
						 units[currentSelection].transform.position+dirToTarget,
						  units[currentSelection].Speed * Time.deltaTime 
		);
		
		
		if( (units[currentSelection].transform.position - moveList[m].pos).sqrMagnitude == 0) {
			units[currentSelection].IsMoving = false;
			EndMove(moveList);
		}	
	}

	private void EndMove(List<Node> moveList)
	{	
		if(m < moveList.Count - 1) {
			++m;
			m = Mathf.Clamp(m,0,moveList.Count - 1);
		}
		else {
			state = EnemyState.Idle;
			
		}
	}
	
	private void EnemyActive() {
		target.gameObject.layer = LayerMask.NameToLayer("Unit");
		if(!units[currentSelection].HasAttacked && units[currentSelection].gameObject.activeSelf  == true) {
			units[currentSelection].attack();
		}
		units[currentSelection].HasMoved = true;
		
		m = 0;
		
		units[currentSelection].setPreviousPos();
		++currentSelection;	
		currentSelection = Mathf.Clamp(currentSelection,0,units.Count - 1);
		state = EnemyState.FindingMove;
		
	}
	
	
	
	public void initAttacking() {
		for(int i = 0; i < units.Count; i++) {
			currentSelection = 0;	
			units[i].initAttacking();
		}
	}
	
	public void disableAttacking() {
		for(int i = 0; i < units.Count; i++)
			units[i].disableAttacking();
	}
	
	public bool isDefeated() {
		bool isDefeated =  GameObject.FindGameObjectsWithTag ("Enemy").Length == 0;
		
		return isDefeated;

	}
	
	public int numLeft() {
		int amount =  GameObject.FindGameObjectsWithTag ("Enemy").Length;
		
		return amount;
	}
	
}
public enum EnemyState {FindingMove, Moving, Idle}
