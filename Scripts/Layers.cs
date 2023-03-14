using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Layers : MonoBehaviour
{
    [SerializeField] LayerMask unitLayer;
	[SerializeField] LayerMask enemyLayer;
	[SerializeField] LayerMask targetLayer;
	[SerializeField] LayerMask objectsLayer;
	[SerializeField] LayerMask moveSquareLayer;
	[SerializeField] LayerMask userUnitActionLayer;


	public static Layers i {get; set;}
	
	private void Awake()
	{
		i = this;
	}
	
	public LayerMask UnitLayer {
		get => unitLayer;
	}
		
	public LayerMask EnemyLayer {
		get => enemyLayer;
	}
	public LayerMask TargetLayer {
		get => targetLayer;
	}
	public LayerMask ObjectsLayer {
		get => objectsLayer;
	}
	public LayerMask MoveSquareLayer {
		get => moveSquareLayer;
	}
	public LayerMask UserUnitActionLayer {
		get => userUnitActionLayer;
	}
}
