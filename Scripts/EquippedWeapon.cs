using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EquippedWeapon : MonoBehaviour
{
    public SpriteRenderer spriteRenderer;
    public Unit unit;
	void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
		unit = GetComponentInParent<Unit>();
    }
	
	void Update() {
		spriteRenderer.sprite = unit.weapons[0].HeldWeapon;
	}
}
