using System;
using UnityEngine;
using UnityEngine.Assertions;

using RPG.CameraUI;

namespace RPG.Characters
{
	public class PlayerControl : MonoBehaviour  // todo extract weapon system
	{
		CameraRaycaster cameraRaycaster;
		Character character;
		Enemy currentEnemy;
		SpecialAbilities abilities;
		WeaponSystem weaponSystem;

		void Start ()
		{
			abilities = GetComponent<SpecialAbilities> ();
			character = GetComponent<Character> ();
			weaponSystem = GetComponent<WeaponSystem> ();

			RegisterForMouseEvents ();
		}

		private void RegisterForMouseEvents ()
		{
			cameraRaycaster = FindObjectOfType<CameraRaycaster> ();
			cameraRaycaster.OnMouseOverEnemyObservers += OnMouseOverEnemy;
			cameraRaycaster.OnMouseOverPotentiallyWalkable += OnMouseOverPotentiallyWalkable;
		}

		void OnMouseOverEnemy ( Enemy enemy )
		{
			currentEnemy = enemy;
			if ( Input.GetMouseButton ( 0 ) && IsTargetInRange ( enemy.gameObject ) )
			{
				weaponSystem.AttackTarget ( enemy.gameObject );
			}
			else if ( Input.GetMouseButtonDown ( 1 ) ) //TODO check for ability range
			{
				abilities.AttemptSpecialAbility ( 0 );
			}
		}

		bool IsTargetInRange ( GameObject target )
		{
			float distanceToTarget = (target.transform.position - transform.position).magnitude;
			return distanceToTarget <= weaponSystem.currentWeapon.GetMaxAttackRange ();
		}

		void OnMouseOverPotentiallyWalkable ( Vector3 desination )
		{
			if ( Input.GetMouseButton ( 0 ) )
			{
				character.SetDestination ( desination );
			}
		}

		void Update ()
		{
			ScanForAbilityKeyDown ();
		}

		private void ScanForAbilityKeyDown ()
		{
			for ( int keyIndex = 1; keyIndex < abilities.GetNumberOfAbilities(); keyIndex++ )
			{
				if ( Input.GetKeyDown( keyIndex.ToString() ) )
				{
					abilities.AttemptSpecialAbility ( keyIndex );
				}
			}
		}
	}
}