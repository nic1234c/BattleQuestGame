using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class Health : MonoBehaviour
{
    [SerializeField] public GameObject health;
	[SerializeField] public Text name;

		
		public void setHealth(float hp)
		{
			health.transform.localScale = new Vector3(hp,1f);
		}
		
		public void setName(string n)
		{
			name.text = n;
		}
		
		public IEnumerator changeHealth(float newHp)
		{
			if(newHp < 0)
				newHp = 0;
			
			float currentHp = health.transform.localScale.x;
			float lostHp = currentHp - newHp;
			
			while ( currentHp - newHp > Mathf.Epsilon)
			{
				currentHp -= lostHp * Time.deltaTime;
				health.transform.localScale = new Vector3(currentHp, 1f);
				yield return null;
				
			}

				health.transform.localScale = new Vector3(newHp, 1f);
		}
}
