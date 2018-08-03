using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.Characters.ThirdPerson;

public class Enemy : MonoBehaviour
{
	[SerializeField] float maxHealthPoints = 100f;
	[SerializeField] float attackRadius= 5f;

	float currentHealthPoints = 100f;
	AICharacterControl aiCharacterControl = null;
	Player player;

	void Start () {
		aiCharacterControl = GetComponent<AICharacterControl> ();
		player = FindObjectOfType<Player> ();

	}
	
	void Update () {

		float distanceToPlayer = Vector3.Distance ( player.transform.position, aiCharacterControl.transform.position );

		if ( distanceToPlayer <= attackRadius )
		{
			aiCharacterControl.SetTarget ( player.transform );
		}
		else
		{
			aiCharacterControl.SetTarget ( transform );
		}

	}

	public float healthAsPercentage
	{
		get
		{
			return currentHealthPoints / maxHealthPoints;
		}
	}

	private void OnDrawGizmos ()
	{
		// draw attack
		Gizmos.color = new Color ( 255f, 0f, 0f, .5f );
		Gizmos.DrawWireSphere ( transform.position, attackRadius );
	}
}
