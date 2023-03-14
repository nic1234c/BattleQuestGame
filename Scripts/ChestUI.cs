using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class ChestUI : MonoBehaviour
{
	[SerializeField] public Image selectedItemImage;
	[SerializeField] public Text selectedItemName;
	[SerializeField] public GameObject ChestOpenedPrompt;

 
	public IEnumerator showItem(Weapon weapon,Collider2D col) {
		ChestOpenedPrompt.transform.position = col.gameObject.GetComponent<Unit>().transform.position + new Vector3(-2,0.5f,0);
		ChestOpenedPrompt.SetActive(true);
		 selectedItemName.text = weapon.getFullWeaponName();
		 selectedItemImage.sprite = weapon.Image;
		 yield return new WaitForSeconds(3f);
		 ChestOpenedPrompt.SetActive(false); 
		
	}

	
	
}
