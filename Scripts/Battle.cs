using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Battle : MonoBehaviour
{
	
	[SerializeField] User player;
	[SerializeField] Enemy enemy;
	BattleManager m;
	Level l;
	State currentUserState;
    public State state;

    void Start()
    {		
		m = GetComponent<BattleManager>();
		l = GetComponent<Level>();
		
		state = State.Setup;
		player.EndTurn += changeUserTurn;
		enemy.EndTurn += changeUserTurn;
		player.initAttacking();
		
		m.enemyAttacking += () =>
		{
			state = State.EnemyBattling;
		};
		
		m.OnShowDialog += () =>
		{
			state = State.Battling;
		};
		m.OnCloseDialog += () =>
		{
			if (state == State.Battling)
				state = currentUserState;
		};
		m.OnEnemyAttackingOver += () =>
		{
			if (state == State.EnemyBattling)
				state = currentUserState;
		};

    }

    void Update()
    {
		if(state == State.Setup) {
			StartCoroutine(l.ShowLevel());
			state = State.PlayerTurn;
			player.initAttacking();
			enemy.Start();
		}
        else if (state == State.PlayerTurn)
		{
			
			currentUserState = State.PlayerTurn;
			player.UserTurn();
		}
		else if (state == State.EnemyTurn)
		{	
			
			currentUserState = State.EnemyTurn;
			enemy.EnemyTurn();
		}
		else if (state == State.Battling)
		{
			m.handleBattle();	
		}
		
		if (enemy.isDefeated()) 
		{
			player.reviveUnits();
			l.ChangeLevel();
		} 
		else if(player.isDefeated())
		{
			StartCoroutine(l.GameOver());
		}			
		
		
    }
	
	void changeUserTurn() {
		 if (state == State.PlayerTurn)
		{
			enemy.initAttacking();
			player.disableAttacking();
			state = State.EnemyTurn;
			StartCoroutine(l.Turn("Enemy Turn"));	
		}
		else if (state == State.EnemyTurn)
		{
			enemy.disableAttacking();
			player.initAttacking();
			state = State.PlayerTurn;
			StartCoroutine(l.Turn("Player Turn"));
		}
		
	}

}
public enum State {Setup, PlayerTurn, EnemyTurn, Battling, EnemyBattling}
