using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// TODO consider rewiring
using RPG.Core;

namespace RPG.Characters
{
	public class Enemy : MonoBehaviour, IDamagable //todo remove interface
	{
		[SerializeField] float attackRadius = 5f;
		[SerializeField] float chaseRadius = 5f;

		[SerializeField] float damagePerShot = 9f;
		[SerializeField] float firingPeriodInSeconds = 0.5f;
		[SerializeField] float firingPeriodVariation = 0.1f;
		[SerializeField] GameObject projectileToUse;
		[SerializeField] GameObject projectileSocket;
		[SerializeField] Vector3 aimOffset = new Vector3 ( 0f, 1f, 0f );

		bool isAttacking = false;
		Player player;

		public void TakeDamage (float amount)
		{
			//todo remove
		}

		void Start ()
		{
			player = FindObjectOfType<Player> ();
		}

		void Update ()
		{			
			float distanceToPlayer = Vector3.Distance ( player.transform.position, transform.position );
			if ( distanceToPlayer > attackRadius )
			{
				isAttacking = false;
				CancelInvoke ();
			}

			if ( distanceToPlayer <= attackRadius && !isAttacking )
			{
				isAttacking = true;
				float randomizedDelay = Random.Range ( firingPeriodInSeconds - firingPeriodVariation, firingPeriodInSeconds + firingPeriodVariation );
				InvokeRepeating ( "SpawnProjectile", 0, randomizedDelay ); //TODO switch to coroutine
			}

			if ( distanceToPlayer <= chaseRadius )
			{
				//aiCharacterControl.SetTarget ( player.transform );
			}
			else
			{
				//aiCharacterControl.SetTarget ( transform );
			}
			
		}

		// TODO separate character firing logic
		void SpawnProjectile ()
		{
			GameObject projectile = Instantiate ( projectileToUse, projectileSocket.transform.position, Quaternion.identity );
			Projectile projectileComponent = projectile.GetComponent<Projectile> ();
			projectileComponent.SetDamage ( damagePerShot );
			projectileComponent.SetParentLayer ( gameObject );

			Vector3 unitVectorToPlayer = (player.transform.position + aimOffset - projectileSocket.transform.position).normalized;
			float projectileSpeed = projectileComponent.GetDefaultLaunchSpeed ();
			projectile.GetComponent<Rigidbody> ().velocity = unitVectorToPlayer * projectileSpeed;
		}

		private void OnDrawGizmos ()
		{
			// draw move
			Gizmos.color = new Color ( 0f, 0f, 255f, .5f );
			Gizmos.DrawWireSphere ( transform.position, chaseRadius );

			// draw attack
			Gizmos.color = new Color ( 255f, 0f, 0f, .5f );
			Gizmos.DrawWireSphere ( transform.position, attackRadius );
		}
	}
}