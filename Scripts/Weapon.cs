using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Weapon", menuName = "Create Weapon")]
public class Weapon : ScriptableObject {
	
	
	[SerializeField] int mt;
	[SerializeField] WeaponType type;
	[SerializeField] WeaponKind name;
	[SerializeField] int hit;
	[SerializeField] int weaponRange = 1;
	[SerializeField] int healAmount;
	[SerializeField] Sprite image;
	[SerializeField] Sprite heldWeapon;
  
	public int Mt {
		get => mt;
	}
	public int Hit {
		get => hit;
	}
	public WeaponType Type {
		get => type;
	}
	public WeaponKind Name {
		get => name;
	}
	public int WeaponRange {
		get => weaponRange;
	}
	public int HealAmount {
		get => healAmount;
	}
	public Sprite Image {
		get => image;
	}
	
	public Sprite HeldWeapon {
		get => heldWeapon;
	}
	
	public string getFullWeaponName() {
		return type.ToString() + " " + name.ToString();
	}
	
}

public enum WeaponType {Iron, Steel, Silver, Special, Fire, Throwing, Heal, Ice, Golden, Dragon}
public enum WeaponKind {Sword, Lance, Axe, Bow, Hammer, Staff, Fire, Dagger }
