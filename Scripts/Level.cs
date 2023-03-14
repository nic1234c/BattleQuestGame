using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class Level : MonoBehaviour
{
	[SerializeField] Text turnWindowText;
	[SerializeField] GameObject levelWindow;
	[SerializeField] Text levelWindowText;
	[SerializeField] Text winCondition;
	
	public IEnumerator ShowLevel() {
		levelWindow.SetActive(true);
		levelWindowText.text = "Level " + (SceneManager.GetActiveScene().buildIndex+1).ToString();
		yield return new WaitForSeconds(2f);
		levelWindow.SetActive(false);
	}
	
	public void ChangeLevel() {
		DontDestroyOnLoad(GameObject.Find("Battle"));
		GameObject[] units = GameObject.FindGameObjectsWithTag("Player");
		
		for(int i = 0; i < units.Length; i++) {
			DontDestroyOnLoad(units[i].gameObject);
			units[i].GetComponent<Unit>().Hp = units[i].GetComponent<Unit>().MaxHp;
			units[i].GetComponent<Unit>().transform.position = units[i].GetComponent<Unit>().StartPosition;
			units[i].GetComponent<Unit>().Start();
		}
		
		DontDestroyOnLoad(GameObject.Find("Selector"));
		DontDestroyOnLoad(GameObject.Find("Canvas"));
		
		SceneManager.LoadScene (SceneManager.GetActiveScene().buildIndex+1);
		
		
		Battle b = GameObject.Find("Battle").GetComponent<Battle>();
		b.state = State.Setup;	
		
		if(SceneManager.GetActiveScene().buildIndex+1 > 3) {
			Debug.Log(SceneManager.GetActiveScene().buildIndex);
			User u = GameObject.Find("Player").GetComponent<User>();
		    Enemy e = GameObject.Find("Enemy").GetComponent<Enemy>();
			Board userBoard = u.GetComponent<Board>();
			userBoard.rows = 26;
			userBoard.cols = 9;
			Board enemyBoard = e.GetComponent<Board>();
			enemyBoard.rows = 26;
			enemyBoard.cols = 9;
		}

		
	}
	
	public IEnumerator Turn(string type) {
		turnWindowText.gameObject.SetActive(true);
		turnWindowText.text = type;
		yield return new WaitForSeconds(2f);
		turnWindowText.gameObject.SetActive(false);

	}
	
	
	public IEnumerator GameOver() {
		levelWindow.SetActive(true);
		levelWindowText.text = "Game Over";
		winCondition.text = "";
		yield return new WaitForSeconds(4f);
		SceneManager.LoadScene (0);

	}
	
	
	
	
}
