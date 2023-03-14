using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Unit : MonoBehaviour
{
    [SerializeField] float speed;
	
	[SerializeField] int hp = 30;
	[SerializeField] int maxHp = 30;
	[SerializeField] int atk = 5;
	[SerializeField] int sor = 5;
	[SerializeField] int def = 5;
	[SerializeField] int spd = 5;
    [SerializeField] int prof = 5;
	[SerializeField] int imm = 5;
	[SerializeField] int level = 1;
	[SerializeField] int exp = 0;
	[SerializeField] int expToLevel = 30;
	[SerializeField] int baseExpToLevel = 30;
	[SerializeField] Class unitClass;
	
	public bool HasMoved{get; set;}
	public bool HasAttacked{get; set;}
	
	public List<Vector3> attackVector;
	public List<Vector3> tradeVector;
	
	public Vector3 startPosition;
	public Vector3 previousPosition;
	public GameObject target;  
	public string unitName;
	[SerializeField] public List<Weapon> weapons;
	[SerializeField] int movement = 5; 
	BattleManager m;

	public bool IsMoving{get; set;}
	public bool IsAttacking{get; set;}

	Animator anim;
	
	private void Awake()
	{
		anim = GetComponent<Animator>();
		
		m = GameObject.Find("Battle").GetComponent<BattleManager>();
		HasMoved = false;
		HasAttacked = true;
		int range = weapons[0].WeaponRange;
		
		attackVector.Add(transform.position + new Vector3(0,1,0));
		attackVector.Add(transform.position + new Vector3(1,0,0));
		attackVector.Add(transform.position + new Vector3(-1,0,0));
		attackVector.Add(transform.position + new Vector3(0,-1,0));
		
		tradeVector.Add(transform.position + new Vector3(0,1,0));
		tradeVector.Add(transform.position + new Vector3(1,0,0));
		tradeVector.Add(transform.position + new Vector3(-1,0,0));
		tradeVector.Add(transform.position + new Vector3(0,-1,0));
		
		attackVector.Add(transform.position + new Vector3(0,1,0));
		attackVector.Add(transform.position + new Vector3(1,0,0));
		attackVector.Add(transform.position + new Vector3(-1,0,0));
		attackVector.Add(transform.position + new Vector3(0,-1,0));
		
	}
	
	public void Start() {
		startPosition = transform.position;
		previousPosition = transform.position;
	}
	
	public void Update() {

		int range = weapons[0].WeaponRange;
		
		attackVector[0] = transform.position + new Vector3(0,range,0);
		attackVector[1] = transform.position + new Vector3(range,0,0);
		attackVector[2] = transform.position + new Vector3(-range,0,0);
		attackVector[3] = transform.position + new Vector3(0,-range,0);
		
		tradeVector[0] = transform.position + new Vector3(0,1,0);
		tradeVector[1] = transform.position + new Vector3(1,0,0);
		tradeVector[2] = transform.position + new Vector3(-1,0,0);
		tradeVector[3] = transform.position + new Vector3(0,-1,0);

		if(range == 2) {
			attackVector[4] = transform.position + new Vector3(1,1,0);
			attackVector[5] = transform.position + new Vector3(1,-1,0);
			attackVector[6] = transform.position + new Vector3(-1,1,0);
			attackVector[7] = transform.position + new Vector3(-1,-1,0);
		}
		
	}
	
	public void attack() {
		Collider2D col;
		
		for(int i = 0; i < attackVector.Count; i++) {
			col = Physics2D.OverlapPoint(gameObject.GetComponent<Unit>().attackVector[i],Layers.i.UnitLayer);
			if(col) {
				if(col.gameObject.tag == "Player" && !gameObject.GetComponent<Unit>().HasAttacked && CanAttackWithWeapon()) {
					m.setBattleData(gameObject,col.gameObject);
					m.runEnemyAttack();	
				}
			}
		}	
	}
	public void playerAttack() {
		Collider2D col;
		for(int i = 0; i < attackVector.Count; i++) {
			col = Physics2D.OverlapPoint(gameObject.GetComponent<Unit>().attackVector[i],Layers.i.UnitLayer);
			if(col) {
				if(col.gameObject.tag == "Enemy" && !gameObject.GetComponent<Unit>().HasAttacked && CanAttackWithWeapon() ) {
					m.showActionMenu("Attack");
					m.setBattleTargets(col.gameObject);
					m.setBattleData(gameObject,col.gameObject);
					m.setBattleForecastMenu(gameObject,col.gameObject);
				}
			}
		}
	}
	
	public void playerHeal() {
		Collider2D col;
		for(int i = 0; i < attackVector.Count; i++) {
			col = Physics2D.OverlapPoint(gameObject.GetComponent<Unit>().attackVector[i],Layers.i.UserUnitActionLayer);
			if(col) {
				if(col.gameObject.tag == "Player" && !gameObject.GetComponent<Unit>().HasAttacked && (weapons[0].Name == WeaponKind.Staff && unitClass == Class.Cleric || weapons[0].Name == WeaponKind.Staff && unitClass == Class.HolyKnight) ) {
					if(col.gameObject.GetComponent<Unit>().hp != col.gameObject.GetComponent<Unit>().maxHp) {
						m.showActionMenu("Heal");
						m.setPlayerTargets(col.gameObject);
						m.setBattleData(gameObject,col.gameObject);
					}
				}
			}
		}	
	}
	
	public void PlayerTrade() {
		Collider2D col;
		for(int i = 0; i < attackVector.Count; i++) {
			col = Physics2D.OverlapPoint(gameObject.GetComponent<Unit>().tradeVector[i],Layers.i.UserUnitActionLayer);
			if(col) {
				if(col.gameObject.tag == "Player" && !gameObject.GetComponent<Unit>().HasAttacked) {
					m.showTradeMenu();
					m.setPlayerTradeTargets(col.gameObject);
					m.setBattleData(gameObject,col.gameObject);
				}
			}
		}	
	}
	
	public void showWeaponsMenu(){
		m.setBattleData(gameObject,null);
		m.showWeaponsMenu();
	}
	
	public void addWeapon(Weapon w) {	
		if(weapons.Count >= 5) {
			showWeaponsMenu();
			m.toDiscardWeapon = true;
			m.newWeapon = w;
		} else{
			weapons.Add(w);
		}
	}
	
	public void HandleAnimationUpdate()
	{
		anim.SetBool("IsWalking",IsMoving);
	}
	
	public Unit GetTarget() {
		var targets = GameObject.FindGameObjectsWithTag ("Player");
		float oldDistance = Mathf.Infinity;
		
		foreach (GameObject g in targets)
        {
            float distance = Vector3.Distance(gameObject.transform.position, g.transform.position);
			
            if (distance < oldDistance)
            {
                target = g;
                oldDistance = distance;
            }
        }
		target.gameObject.layer = LayerMask.NameToLayer("Target");

		return target.GetComponent<Unit>();
	}
	
	public bool CanAttackWithWeapon() {
		return (weapons[0].Name == WeaponKind.Sword && unitClass == Class.SwordKnight) ||
		(weapons[0].Name == WeaponKind.Axe && unitClass == Class.AxeKnight) ||
		(weapons[0].Name == WeaponKind.Lance && unitClass == Class.LanceKnight) ||
		(weapons[0].Name == WeaponKind.Staff && unitClass == Class.Cleric)||
		(weapons[0].Type == WeaponType.Heal && weapons[0].Name == WeaponKind.Staff && unitClass == Class.HolyKnight)||
		(weapons[0].Name == WeaponKind.Sword && unitClass == Class.HolyKnight) || 
		(weapons[0].Name == WeaponKind.Dagger && unitClass == Class.Thief);
	}
	
	public void initAttacking() {
		HasAttacked = false;
		HasMoved = false;
		IsAttacking = true;
	}
	
	public void disableAttacking() {
		HasAttacked = true;
	}
	
	public void setPreviousPos() {
		previousPosition = transform.position;
	}
	
	public float Speed {
		get => speed;
	}
	 public int Hp   
	{
		get { return hp; }
		set { hp = value; }
	}
	public int MaxHp
	{
		set { maxHp = value; }
		get => maxHp;
	}
	public int Atk {
		set { atk = value; }
		get => atk;
	}
	
	public int Sor {
		set { sor = value; }
		get => sor;
	}
	
	public int Def {
		set { def = value; }
		get => def;
	}
	
	public int Spd {
		set { spd = value; }
		get => spd;
	}
	
	public int Prof {
		set { prof = value; }
		get => prof;
	}
	
	public int Imm {
		set { imm = value; }
		get => imm;
	}
	
	public int Level {
		set { level = value; }
		get => level;
	}
	
	public int Exp {
		get => exp;
		set { exp = value; }
	}
	
	public int ExpToLevel {
		get => expToLevel;
		set { expToLevel = value; }
	}
	
	public Vector3 StartPosition {
		get => startPosition;
	}
	
	public int BaseExpToLevel {
		get => baseExpToLevel;
	}
	
	public int Movement {
		get => movement;
	}
	public bool hasAttacked {
		get => HasAttacked;
	}
	
	public List<Weapon> Weapons {
		get => weapons;
	}
	
	public List<Vector3> AttackVector {
		get => attackVector;
	}
	

}

public enum Class {Thief, SwordKnight, AxeKnight, LanceKnight, Cleric, HolyKnight}
