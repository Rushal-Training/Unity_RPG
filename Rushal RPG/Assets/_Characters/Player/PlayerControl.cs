using System;
using System.Collections;
using UnityEngine;
using RPG.CameraUI;

namespace RPG.Characters
{
	public class PlayerControl : MonoBehaviour  // todo extract weapon system
	{
		Character character;
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
			CameraRaycaster cameraRaycaster = FindObjectOfType<CameraRaycaster> ();
			cameraRaycaster.OnMouseOverEnemyObservers += OnMouseOverEnemy;
			cameraRaycaster.OnMouseOverPotentiallyWalkable += OnMouseOverPotentiallyWalkable;
		}

		bool IsTargetInRange( GameObject target )
		{
			float distanceToTarget = ( target.transform.position - transform.position ).magnitude;
			return distanceToTarget <= weaponSystem.currentWeapon.GetMaxAttackRange();
		}

		void OnMouseOverEnemy ( EnemyAI enemy )
		{
			if ( Input.GetMouseButton ( 0 ) && IsTargetInRange ( enemy.gameObject ) )
			{
				weaponSystem.AttackTarget ( enemy.gameObject );
			}
			else if ( Input.GetMouseButton( 0 ) && !IsTargetInRange( enemy.gameObject ) )
			{
				StartCoroutine( MoveAndAttack( enemy.gameObject ) );
			}
			else if ( Input.GetMouseButtonDown ( 1 ) && IsTargetInRange( enemy.gameObject ) )
			{
				abilities.AttemptSpecialAbility ( 0, enemy.gameObject );
			}
			else if ( Input.GetMouseButtonDown( 1 ) && !IsTargetInRange( enemy.gameObject ) )
			{
				StartCoroutine( MoveAndPowerAttack( enemy.gameObject ) );
			}
		}

		IEnumerator MoveToTarget( GameObject target )
		{
			character.SetDestination( target.transform.position );
			while ( !IsTargetInRange( target ) )
			{
				yield return new WaitForEndOfFrame();
			}
			yield return new WaitForEndOfFrame();
		}

		IEnumerator MoveAndAttack( GameObject target )
		{
			yield return StartCoroutine( MoveToTarget( target.gameObject ) );
			weaponSystem.AttackTarget( target.gameObject );
		}

		IEnumerator MoveAndPowerAttack( GameObject target )
		{
			yield return StartCoroutine( MoveToTarget( target.gameObject ) );
			abilities.AttemptSpecialAbility(0, target.gameObject );
		}

		void OnMouseOverPotentiallyWalkable ( Vector3 desination )
		{
			if ( Input.GetMouseButton ( 0 ) )
			{
				weaponSystem.StopAttacking ();
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