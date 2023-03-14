using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class Chest : MonoBehaviour
{
	
	[SerializeField] Sprite openImage;
	[SerializeField] Weapon weapon;
	bool isOpen = false;
	bool itemTaken = false;
	ChestUI ui;
	//[SerializeField] public Image selectedItemImage;
	//[SerializeField] public Text selectedItemName;
	//[SerializeField] public GameObject ChestOpenedPrompt;


	void Start() {
		ui = GameObject.Find("Battle").GetComponent<ChestUI>();
	}

    // Update is called once per frame
    void Update()
    {
		Collider2D col;
		
        if(isOpen) {
			GetComponent<SpriteRenderer>().sprite = openImage;
		}
		 col = Physics2D.OverlapPoint(transform.position, Layers.i.UserUnitActionLayer);
		 
		 if(col && !itemTaken) { 
		 	//ChestOpenedPrompt.transform.position = col.gameObject.GetComponent<Unit>().transform.position + new Vector3(-2,0.5f,0);
			getItem(col);
			StartCoroutine(ui.showItem(weapon,col));
		 }
    }	
	
	void getItem(Collider2D col) {
		// ChestOpenedPrompt.SetActive(true);
		 isOpen = true;
		 itemTaken = true;
		 //selectedItemName.text = weapon.getFullWeaponName();
		// selectedItemImage.sprite = weapon.Image;
		// yield return new WaitForSeconds(3f);
		 //ChestOpenedPrompt.SetActive(false); 
		 col.gameObject.GetComponent<Unit>().addWeapon(weapon);
	}

	
	
}
