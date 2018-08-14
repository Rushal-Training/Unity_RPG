using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPG.Characters
{
	[RequireComponent(typeof(Character))]
	[RequireComponent(typeof(HealthSystem))]
	[RequireComponent( typeof( WeaponSystem ) )]
	public class EnemyAI : MonoBehaviour
	{
		[SerializeField] float chaseRadius = 6f;
		[SerializeField] WaypointContainer patrolPath;
		[SerializeField] float waypointTolerance = 2f;
		[SerializeField] float waypointDwellTime = 0.5f;

		Character character;
		PlayerControl player;

		float currentWeaponRange;
		float distanceToPlayer;
		int nextWaypointIndex;

		enum State { idle, patrolling, attacking, chasing }
		State state = State.idle;

		void Start ()
		{
			character = GetComponent<Character>();
			player = FindObjectOfType<PlayerControl>();
		}

		void Update ()
		{
			WeaponSystem weaponSystem = GetComponent<WeaponSystem>();
			currentWeaponRange = weaponSystem.currentWeapon.GetMaxAttackRange();
			distanceToPlayer = Vector3.Distance ( player.transform.position, transform.position );

			if ( distanceToPlayer > chaseRadius && state != State.patrolling )
			{
				StopAllCoroutines();
				weaponSystem.StopAttacking();
				StartCoroutine( Patrol() );
			}
			if ( distanceToPlayer <= chaseRadius && state != State.chasing )
			{
				StopAllCoroutines();
				weaponSystem.StopAttacking();
				StartCoroutine( ChasePlayer() );
			}
			if ( distanceToPlayer <= chaseRadius && state != State.attacking )
			{
				StopAllCoroutines();
				state = State.attacking;
				weaponSystem.AttackTarget( player.gameObject );
			}
		}

		IEnumerator Patrol()
		{
			state = State.patrolling;
			while ( true )
			{
				Vector3 nextWaypointPos = patrolPath.transform.GetChild(nextWaypointIndex).position;
				character.SetDestination( nextWaypointPos );
				CycleWaypointWhenClose( nextWaypointPos );
				yield return new WaitForSeconds( waypointDwellTime );
			}
		}

		private void CycleWaypointWhenClose( Vector3 nextWaypointPos )
		{
			if ( Vector3.Distance( transform.position, nextWaypointPos ) <= waypointTolerance )
			{
				nextWaypointIndex = ( nextWaypointIndex + 1 ) % patrolPath.transform.childCount;
			}
		}

		IEnumerator ChasePlayer()
		{
			state = State.chasing;
			while ( distanceToPlayer >= currentWeaponRange )
			{
				character.SetDestination( player.transform.position );
				yield return new WaitForEndOfFrame();
			}
		}

		private void OnDrawGizmos ()
		{
			// draw move
			Gizmos.color = new Color ( 0, 0, 255f, .5f );
			Gizmos.DrawWireSphere ( transform.position, chaseRadius );

			// draw attack
			Gizmos.color = new Color ( 255f, 0f, 0f, .5f );
			Gizmos.DrawWireSphere ( transform.position, currentWeaponRange );
		}
	}
}