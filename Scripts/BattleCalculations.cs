using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using System;

public class BattleCalculations : MonoBehaviour
{
   [SerializeField] GameObject attacker;
   [SerializeField] GameObject receiver;
   [SerializeField] GameObject playerHp;
   [SerializeField] GameObject enemyHp;
	AudioSource audioSource;
	AudioClips audioClips;
	SpriteRenderer spriteR;
	Color oColor;
	Vector3 oPos;


	private void Awake() {
		audioSource = GetComponent<AudioSource>();
		audioClips = GetComponent<AudioClips>();

	}
	 public GameObject Attacker   
	{
		get { return attacker; }
		set { attacker = value; }
	}
	
	 public GameObject Receiver   
	{
		get { return receiver; }
		set { receiver = value; }
	}
	
	public IEnumerator attackUnit(Action EndEnemyAttack = null) {	
		var inputAttacker = attacker.GetComponent<Unit>();
		var inputReceiver = receiver.GetComponent<Unit>();
		
		int randomNumber = UnityEngine.Random.Range(0, 100);
		
		playerHp.SetActive(true);
		enemyHp.SetActive(true);
		
		if(EndEnemyAttack == null) {
			playerHp.GetComponent<Health>().setName(inputAttacker.unitName);
			enemyHp.GetComponent<Health>().setName(inputReceiver.unitName);

			playerHp.GetComponent<Health>().setHealth((float)inputAttacker.Hp/inputAttacker.MaxHp);
			enemyHp.GetComponent<Health>().setHealth((float)inputReceiver.Hp/inputReceiver.MaxHp);
		}
		else {
			playerHp.GetComponent<Health>().setName(inputReceiver.unitName);
			enemyHp.GetComponent<Health>().setName(inputAttacker.unitName);
			
			playerHp.GetComponent<Health>().setHealth((float)inputReceiver.Hp/inputReceiver.MaxHp);
			enemyHp.GetComponent<Health>().setHealth((float)inputAttacker.Hp/inputAttacker.MaxHp);
		}

		attackAnimation(attacker);
		if(inputAttacker.Weapons[0].Hit + inputAttacker.Prof >= randomNumber) {
			
			if(inputAttacker.Weapons[0].Name == WeaponKind.Staff) {
				inputReceiver.Hp = (inputReceiver.Hp) - (inputAttacker.Sor + inputAttacker.Weapons[0].Mt - inputReceiver.Imm);
			} 
			else {
				inputReceiver.Hp = (inputReceiver.Hp) - (inputAttacker.Atk + inputAttacker.Weapons[0].Mt - inputReceiver.Def);
			}
					
			yield return new WaitForSeconds(0.5f);
			
			hitAnimation(receiver);
				
			if(EndEnemyAttack == null) {
				yield return enemyHp.GetComponent<Health>().changeHealth((float)inputReceiver.Hp/inputReceiver.MaxHp);
			}
			else {
				yield return playerHp.GetComponent<Health>().changeHealth((float)inputReceiver.Hp/inputReceiver.MaxHp);
			}
		}
		yield return new WaitForSeconds(0.5f);
		
		if (inputReceiver.Hp <= 0) {
			audioSource.clip = audioClips.GetClip("Death");
			audioSource.Play();
			if(inputAttacker.tag == "Player")
				AddExp(inputAttacker,inputReceiver);
			
			receiver.SetActive(false);
		} 
		else if(inputAttacker.Weapons[0].WeaponRange == inputReceiver.Weapons[0].WeaponRange  && inputReceiver.CanAttackWithWeapon()){
			yield return new WaitForSeconds(1f);

			randomNumber = UnityEngine.Random.Range(0, 100);
		
			attackAnimation(receiver);
			if(inputReceiver.Hp > 0 && inputReceiver.Weapons[0].Hit + inputReceiver.Prof >= randomNumber) {
				
				if(inputReceiver.Weapons[0].Name == WeaponKind.Staff) {
					inputAttacker.Hp = (inputAttacker.Hp) - (inputReceiver.Sor + inputReceiver.Weapons[0].Mt - inputAttacker.Imm);
				} 
				else {
					inputAttacker.Hp = (inputAttacker.Hp) - (inputReceiver.Atk + inputReceiver.Weapons[0].Mt - inputAttacker.Def);
				}
				
				yield return new WaitForSeconds(0.5f);
				hitAnimation(attacker);
					
				if(EndEnemyAttack == null) {
					yield return playerHp.GetComponent<Health>().changeHealth((float)inputAttacker.Hp/inputAttacker.MaxHp);
				}
				else {
					yield return enemyHp.GetComponent<Health>().changeHealth((float)inputAttacker.Hp/inputAttacker.MaxHp);
				}
				yield return new WaitForSeconds(0.5f);
				if (inputAttacker.Hp <= 0) {
					audioSource.clip = audioClips.GetClip("Death");
					audioSource.Play();
					if(inputReceiver.tag == "Player")
						AddExp(inputReceiver,inputAttacker);
					attacker.SetActive(false);
				}
			}
		}
		
		yield return new WaitForSeconds(1f);
		
		playerHp.SetActive(false);
		enemyHp.SetActive(false);
		attacker.GetComponent<Unit>().IsAttacking = false;
		receiver.GetComponent<Unit>().IsAttacking = false;
		attacker.GetComponent<Unit>().HasAttacked = true;
		receiver.GetComponent<Unit>().HasAttacked = true;
		attacker.GetComponent<Unit>().HasMoved = true;
		receiver.GetComponent<Unit>().HasMoved = true;

		
		EndEnemyAttack?.Invoke();
	}
	
	public IEnumerator healUnit() {	
		var inputAttacker = attacker.GetComponent<Unit>();
		var inputReceiver = receiver.GetComponent<Unit>();

		playerHp.SetActive(true);	
		playerHp.GetComponent<Health>().setName(inputReceiver.unitName);
		playerHp.GetComponent<Health>().setHealth((float)inputReceiver.Hp/inputAttacker.MaxHp);
			
		
		attackAnimation(attacker,false);
		inputReceiver.Hp = (inputReceiver.Hp) + (inputAttacker.Weapons[0].HealAmount);
		inputReceiver.Hp = Mathf.Clamp(inputReceiver.Hp,0,inputReceiver.MaxHp);
		
		yield return new WaitForSeconds(0.5f);
		
		healAnimation(receiver);	
		yield return playerHp.GetComponent<Health>().changeHealth((float)inputReceiver.Hp/inputReceiver.MaxHp);
		yield return new WaitForSeconds(1f);

		
		playerHp.SetActive(false);

		attacker.GetComponent<Unit>().HasAttacked = true;
		attacker.GetComponent<Unit>().HasMoved = true;

	}
	
	public void hitAnimation(GameObject g)
	{	

		audioSource.clip = audioClips.GetClip("Hit");
		audioSource.Play();
		
		spriteR = g.GetComponent<SpriteRenderer>();
		oColor = g.GetComponent<SpriteRenderer>().color;

		
		var sequence = DOTween.Sequence();
		sequence.Append(spriteR.DOColor(Color.red,0.1f));
		sequence.Append(spriteR.DOColor(oColor,0.1f));	

	}
	
	public void healAnimation(GameObject g)
	{			
		spriteR = g.GetComponent<SpriteRenderer>();
		oColor = g.GetComponent<SpriteRenderer>().color;
	
		var sequence = DOTween.Sequence();
		sequence.Append(spriteR.DOColor(Color.blue,0.1f));
		sequence.Append(spriteR.DOColor(oColor,0.1f));	
	}
	
	public void attackAnimation(GameObject g,bool playSound = true)
	{		
		if(playSound) {
			audioSource.clip = audioClips.GetClip("Attack");
			audioSource.Play();
		}
		
		oPos = g.GetComponent<SpriteRenderer>().transform.position;
		g.GetComponent<Animator>().CrossFade("attack", 0.2f);
		var sequence = DOTween.Sequence();

		sequence.Append(g.GetComponent<SpriteRenderer>().transform.DOLocalMoveX(oPos.x + 0.5f, 0.25f));
		sequence.Append(g.GetComponent<SpriteRenderer>().transform.DOLocalMoveX(oPos.x, 0.25f));
	}
	
	public void AddExp(Unit unit, Unit enemyUnit)
	{		
		float levelDif = (float)enemyUnit.Level / (float) unit.Level;
		unit.Exp +=  (int)(levelDif * 10);
		
		if(unit.Exp >= unit.ExpToLevel) {
			
			LevelUpAttributes(unit);
			unit.Exp = unit.Exp - unit.ExpToLevel;
			unit.ExpToLevel = unit.BaseExpToLevel * unit.Level;
			
		}
	}
	
	public void LevelUpAttributes(Unit unit){
		
		unit.Level += 1;
		int randomNumber;
		
		randomNumber = UnityEngine.Random.Range(0, 2);
		unit.Atk += randomNumber;
		
		randomNumber = UnityEngine.Random.Range(0, 2);
		unit.Sor += randomNumber;
		
		randomNumber = UnityEngine.Random.Range(0, 2);
		unit.Def += randomNumber;
		
		randomNumber = UnityEngine.Random.Range(0, 2);
		unit.Spd += randomNumber;
		
		randomNumber = UnityEngine.Random.Range(0, 2);
		unit.Prof += randomNumber;
		
		randomNumber = UnityEngine.Random.Range(0, 2);
		unit.Imm += randomNumber;
		
		randomNumber = UnityEngine.Random.Range(0, 2);
		unit.MaxHp += randomNumber;
	}
}
