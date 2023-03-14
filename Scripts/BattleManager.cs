using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

public class BattleManager : MonoBehaviour
{
    [SerializeField] GameObject selectActionMenu;
	[SerializeField] GameObject weaponsMenu;
	[SerializeField] GameObject tradeMenu;
	[SerializeField] GameObject battleForecastMenu;

	[SerializeField] List<Text> actionMenuTexts;
	
	[SerializeField] List<Text> tradeMenuOneTexts;
	[SerializeField] List<Image> tradeMenuOneImages;
	[SerializeField] List<Text> tradeMenuTwoTexts;
	[SerializeField] List<Image> tradeMenuTwoImages;
	
	[SerializeField] List<Text> weaponsMenuTexts;
	[SerializeField] List<Image> weaponImages;

	
	[SerializeField] Text battleForecastMenuPlayerName;
	[SerializeField] Text battleForecastMenuEnemyName;
	
	[SerializeField] Text battleForecastMenuPlayerHp;
	[SerializeField] Text battleForecastMenuEnemyHp;
	
	[SerializeField] Text battleForecastMenuPlayerMt;
	[SerializeField] Text battleForecastMenuEnemyMt;
	
	[SerializeField] Text battleForecastMenuPlayerHit;
	[SerializeField] Text battleForecastMenuEnemyHit;
	
	[SerializeField] Text battleForecastMenuPlayerCrit;
	[SerializeField] Text battleForecastMenuEnemyCrit;

	[SerializeField] Text battleForecastMenuPlayerWeapon;
	[SerializeField] Text battleForecastMenuEnemyWeapon;

	[SerializeField] Text battleForecastMenuPlayerMtDoubled;
	[SerializeField] Text battleForecastMenuEnemyMtDoubled;
	
	[SerializeField] GameObject selectionImage;
	
	public bool toDiscardWeapon = false;
	public Weapon newWeapon;

	public event Action OnShowDialog;
	public event Action enemyAttacking;
	public event Action OnCloseDialog;
	public event Action OnEnemyAttackingOver;
	
	AudioSource audioSource;
	AudioClips audioClips;
	
	int currentAction;
	int currentWeapon;
	int currentHealAction;
	int currentBattleAction;
	int currentTradeAction;
	int currentTradeOne;
	int currentTradeTwo;
	BattleState state;
	BattleCalculations bc;
	List<GameObject> selectedEnemies;
	List<GameObject> selectedPlayers;
	List<GameObject> selectedPlayersForTrade;
	
	
	private void Awake()
	{
		bc = GetComponent<BattleCalculations>();
		audioSource = GetComponent<AudioSource>();
		audioClips = GetComponent<AudioClips>();
	}
	
	public void Start() {
		selectedEnemies = new List<GameObject>();
		selectedPlayers = new List<GameObject>();
		selectedPlayersForTrade = new List<GameObject>();
	}
	
	public void handleBattle() {
		if (Input.GetKeyDown(KeyCode.DownArrow))
		{
			audioSource.clip = audioClips.GetClip("Hover");
		    audioSource.Play();
			++currentAction;
			++currentWeapon;
			++currentTradeAction;
			++currentHealAction;
			++currentBattleAction;
			
			if(state == BattleState.TradeItemOne) {
				++currentTradeOne;
			}
			else if(state == BattleState.TradeItemTwo) {
				++currentTradeTwo;  
			}
			
		}
		else if (Input.GetKeyDown(KeyCode.UpArrow))
		{
			audioSource.clip = audioClips.GetClip("Hover");
		    audioSource.Play();
			--currentAction;
			--currentWeapon;
			--currentTradeAction;
			--currentHealAction;
			--currentBattleAction;	
				
			if(state == BattleState.TradeItemOne) {
				--currentTradeOne;
			}
			else if(state == BattleState.TradeItemTwo) {
				--currentTradeTwo;  
			}
		}
		
		currentAction = Mathf.Clamp(currentAction,0 , 2);
		currentWeapon = Mathf.Clamp(currentWeapon,0 , 4);
		currentTradeOne = Mathf.Clamp(currentTradeOne,0 , 4);
		currentTradeTwo = Mathf.Clamp(currentTradeTwo,0 , 4);
		currentHealAction = Mathf.Clamp(currentHealAction,0 , selectedPlayers.Count-1);
		currentTradeAction = Mathf.Clamp(currentTradeAction,0 , selectedPlayersForTrade.Count-1);
		currentBattleAction = Mathf.Clamp(currentBattleAction,0 , selectedEnemies.Count-1);

		if(state == BattleState.ActionMenuOpen) {
			
			UpdateActionSelection(currentAction);
			
			if (Input.GetKeyDown(KeyCode.E)) {
				if (currentAction == 0) {
					switch (actionMenuTexts[0].text.ToString())
					{
						case "Attack":
							selectActionMenu.SetActive(false);
							state = BattleState.BFMenuOpen;
							ShowBattleForecast();
							break;
						case "Heal":
							selectActionMenu.SetActive(false);
							state = BattleState.HealMenu;
							break;
					}	
				}
				else if (currentAction == 1)
				{	
					showWeaponsMenu();
				}
				else if (currentAction == 2)
				{	
					selectActionMenu.SetActive(false);
					selectedEnemies = new List<GameObject>();
					resetAction();
					OnCloseDialog?.Invoke();
				}
			}
		} 
		else if (state == BattleState.WeaponsMenuOpen) {
			UpdateWeaponSelection(currentWeapon);

			if (Input.GetKeyDown(KeyCode.E)) {
				selectedEnemies = new List<GameObject>();
				chooseWeapon(currentWeapon);
				weaponsMenu.SetActive(false);
				resetAction();
				OnCloseDialog?.Invoke();	
			}
			else if(Input.GetKeyDown(KeyCode.Q)) {
				selectedEnemies = new List<GameObject>();
				newWeapon = null;
				toDiscardWeapon = false;
				weaponsMenu.SetActive(false);
				resetAction();
				OnCloseDialog?.Invoke();	
			}
		}
		else if (state == BattleState.BFMenuOpen) {
			
			showSelection(selectedEnemies[currentBattleAction]);
			setBattleData(bc.Attacker,selectedEnemies[currentBattleAction]);
			setBattleForecastMenu(bc.Attacker,selectedEnemies[currentBattleAction]);

			
			if (Input.GetKeyDown(KeyCode.E)) {
				selectedEnemies = new List<GameObject>();
				StartCoroutine(bc.attackUnit());
				battleForecastMenu.SetActive(false);
				selectionImage.SetActive(false);
				resetAction();
				OnCloseDialog?.Invoke();	
			} else if(Input.GetKeyDown(KeyCode.Q)) {
				selectionImage.SetActive(false);
				showActionMenu(actionMenuTexts[0].ToString());
				battleForecastMenu.SetActive(false);
			}
		}
		else if (state == BattleState.HealMenu) {
			showSelection(selectedPlayers[currentHealAction]);
			setBattleData(bc.Attacker,selectedPlayers[currentHealAction]);
			if (Input.GetKeyDown(KeyCode.E)) {
				StartCoroutine(bc.healUnit());
				selectionImage.SetActive(false);
				selectedPlayers = new List<GameObject>();
				resetAction();
				OnCloseDialog?.Invoke();
			}	
		}
		else if (state == BattleState.TradeSelection) {
			showSelection(selectedPlayersForTrade[currentTradeAction]);
			setBattleData(bc.Attacker,selectedPlayersForTrade[currentTradeAction]);
			Debug.Log(currentTradeAction);
			if (Input.GetKeyDown(KeyCode.E)) {
				tradeMenu.SetActive(true);
				selectionImage.SetActive(false);
				SetTradeMenu();
				state = BattleState.TradeItemOne;	
			}	
		}
		else if (state == BattleState.TradeItemOne) {
			UpdateTradeSelectionOne(currentTradeOne);
			if (Input.GetKeyDown(KeyCode.E)) {	
				state = BattleState.TradeItemTwo;	
			}	
		}
		else if (state == BattleState.TradeItemTwo) {
			UpdateTradeSelectionTwo(currentTradeTwo);
			if (Input.GetKeyDown(KeyCode.E)) {
				audioSource.clip = audioClips.GetClip("Trade");
				audioSource.Play();
				state = BattleState.TradeItemTwo;
				TradeWeapon(currentTradeOne,currentTradeTwo);
				tradeMenu.SetActive(false);
				selectedPlayersForTrade = new List<GameObject>();
				resetAction();
				OnCloseDialog?.Invoke();
			}	
		}
	}
	
	public void resetAction() {
		currentAction = 0;
		currentWeapon = 0;
		currentTradeAction = 0;
		currentHealAction = 0;
		currentBattleAction = 0;
		currentTradeOne = 0;
		currentTradeTwo = 0;  
	}
	
	public void setBattleForecastMenu(GameObject player, GameObject enemy) {
		
		var attacker = player.GetComponent<Unit>();
		var receiver = enemy.GetComponent<Unit>();
		
		battleForecastMenuPlayerName.text = attacker.unitName;
		battleForecastMenuEnemyName.text = receiver.unitName;
		 
		if(attacker.Weapons[0].Name == WeaponKind.Staff) {
			battleForecastMenuPlayerMt.text = (attacker.Sor + attacker.Weapons[0].Mt - receiver.Imm).ToString();	
		} 
		else {
			battleForecastMenuPlayerMt.text = (attacker.Atk + attacker.Weapons[0].Mt - receiver.Def).ToString();
		}
		if(receiver.Weapons[0].Name == WeaponKind.Staff)
		{
			battleForecastMenuEnemyMt.text = (receiver.Sor + receiver.Weapons[0].Mt - attacker.Imm).ToString();
		} 
		else {
			battleForecastMenuEnemyMt.text = (receiver.Atk + receiver.Weapons[0].Mt - attacker.Def).ToString();
		}
		
		
		battleForecastMenuPlayerHp.text = attacker.Hp.ToString();
		battleForecastMenuEnemyHp.text = receiver.Hp.ToString();
		
		battleForecastMenuPlayerHit.text = (attacker.Weapons[0].Hit + attacker.Prof).ToString();
		battleForecastMenuEnemyHit.text = (receiver.Weapons[0].Hit + receiver.Prof).ToString();
		
		battleForecastMenuPlayerCrit.text = "0";
		battleForecastMenuEnemyCrit.text = "0";

		battleForecastMenuPlayerWeapon.text = attacker.Weapons[0].getFullWeaponName();
		battleForecastMenuEnemyWeapon.text = receiver.Weapons[0].getFullWeaponName();
		
		
		if( attacker.Weapons[0].WeaponRange != receiver.Weapons[0].WeaponRange ) {
			battleForecastMenuEnemyMt.text = "--";	
			battleForecastMenuEnemyHit.text = "--";
		}
		
		
			
	}
	
	public void setPlayerTargets(GameObject player) {
		selectedPlayers.Add(player);
	}
	public void setPlayerTradeTargets(GameObject player) {
		selectedPlayersForTrade.Add(player);
	}
	public void setBattleTargets(GameObject enemy) {
		selectedEnemies.Add(enemy);
	}
	
	public void setBattleData(GameObject player, GameObject enemy = null) {
		selectActionMenu.transform.position = player.transform.position + new Vector3(-2,0.5f,0);
		weaponsMenu.transform.position = player.transform.position + new Vector3(-2,0.5f,0);
		battleForecastMenu.transform.position = player.transform.position + new Vector3(-2,-3,0);

		bc.Attacker = player;
		if(enemy != null)
			bc.Receiver = enemy;	
	}
	
	public void runEnemyAttack() {
		enemyAttacking?.Invoke();
		StartCoroutine(bc.attackUnit(EndEnemyAttack));
	}
	
	public void EndEnemyAttack() {
		OnEnemyAttackingOver?.Invoke();
	}
	public void UpdateActionSelection(int selectedAction) {
		for(int i = 0; i < actionMenuTexts.Count; ++i)
		{
			if (i == selectedAction)
				actionMenuTexts[i].color = Color.white;
			else
				actionMenuTexts[i].color = Color.black;
		}
	}
	
	public void UpdateWeaponSelection(int currentWeapon) {
		for(int i = 0; i < weaponsMenuTexts.Count; ++i)
		{
			if (i == currentWeapon)
				weaponsMenuTexts[i].color = Color.white;
			else
				weaponsMenuTexts[i].color = Color.black;
		}
	}
	
	public void UpdateTradeSelectionOne(int currentTradeOne) {
		for(int i = 0; i < tradeMenuOneTexts.Count; ++i)
		{
			if (i == currentTradeOne)
				tradeMenuOneTexts[i].color = Color.white;
			else
				tradeMenuOneTexts[i].color = Color.black;
		}
	}
	
	public void UpdateTradeSelectionTwo(int currentTradeTwo) {
		for(int i = 0; i < tradeMenuTwoTexts.Count; ++i)
		{
			if (i == currentTradeTwo)
				tradeMenuTwoTexts[i].color = Color.white;
			else
				tradeMenuTwoTexts[i].color = Color.black;
		}
	}
	
	public void TradeWeapon(int currentTradeOne, int currentTradeTwo) {
		
		var player = bc.Attacker.GetComponent<Unit>();
		var tradeWith = bc.Receiver.GetComponent<Unit>();
		
		Weapon savedWeapon = player.Weapons[currentTradeOne];
		player.Weapons[currentTradeOne] = tradeWith.Weapons[currentTradeTwo];
		tradeWith.Weapons[currentTradeTwo] = savedWeapon;
	}
	

	public void showActionMenu(string actiontext) {
		OnShowDialog?.Invoke();
		actionMenuTexts[0].text = actiontext;
		state = BattleState.ActionMenuOpen;
		selectActionMenu.SetActive(true);
	}
	
	public void showWeaponsMenu() {
		OnShowDialog?.Invoke();
		selectActionMenu.SetActive(false);
		weaponsMenu.SetActive(true);
		selectedEnemies = new List<GameObject>();
		setWeapons();
		state = BattleState.WeaponsMenuOpen;
	}
	
	public void showTradeMenu() {
		OnShowDialog?.Invoke();
		selectActionMenu.SetActive(false);
		state = BattleState.TradeSelection;
	}
	
	public void ShowBattleForecast() {
		state = BattleState.BFMenuOpen;
		battleForecastMenu.SetActive(true);
	}
	
	public void setWeapons() {
		var player = bc.Attacker.GetComponent<Unit>();
		for(int i = 0; i < player.Weapons.Count; i++) {
			weaponsMenuTexts[i].text = player.Weapons[i].getFullWeaponName();
			weaponImages[i].sprite = player.Weapons[i].Image;
			weaponImages[i].gameObject.SetActive(true);

		}
		for(int j = player.Weapons.Count; j < weaponsMenuTexts.Count; j++) {
			weaponsMenuTexts[j].text = "";
			weaponImages[j].gameObject.SetActive(false);
		}
	}
	
	public void SetTradeMenu() {
		var player = bc.Attacker.GetComponent<Unit>();
		for(int i = 0; i < player.Weapons.Count; i++) {
			tradeMenuOneTexts[i].text = player.Weapons[i].getFullWeaponName();
			tradeMenuOneImages[i].sprite = player.Weapons[i].Image;
			tradeMenuOneImages[i].gameObject.SetActive(true);

		}
		for(int j = player.Weapons.Count; j < tradeMenuOneTexts.Count; j++) {
			tradeMenuOneTexts[j].text = "";
			tradeMenuOneImages[j].gameObject.SetActive(false);
		}
		
		var tradeWith = bc.Receiver.GetComponent<Unit>();
		for(int k = 0; k < tradeWith.Weapons.Count; k++) {
			tradeMenuTwoTexts[k].text = tradeWith.Weapons[k].getFullWeaponName();
			tradeMenuTwoImages[k].sprite = tradeWith.Weapons[k].Image;
			tradeMenuTwoImages[k].gameObject.SetActive(true);

		}
		for(int l = tradeWith.Weapons.Count; l < tradeMenuTwoTexts.Count; l++) {
			tradeMenuTwoTexts[l].text = "";
			tradeMenuTwoImages[l].gameObject.SetActive(false);
		}
		
	}
	
	public void chooseWeapon(int currentWeapon) {
		var player = bc.Attacker.GetComponent<Unit>();
		currentWeapon = Mathf.Clamp(currentWeapon,0 , player.Weapons.Count-1);
		
		if(toDiscardWeapon) {
			player.Weapons[currentWeapon] = newWeapon;	
			
		} 
		else {	
			Weapon w = player.Weapons[0];
			player.Weapons[0] = player.Weapons[currentWeapon];
			player.Weapons[currentWeapon] = w;	
		}
		toDiscardWeapon = false;
	}
	
	public void showSelection(GameObject target) {
		selectionImage.SetActive(true);
		selectionImage.transform.position = target.transform.position + new Vector3(0,0.5f,0);
	}
}


public enum BattleState {ActionMenuOpen,WeaponsMenuOpen,BFMenuOpen,HealMenu, TradeSelection, TradeItemOne, TradeItemTwo}
