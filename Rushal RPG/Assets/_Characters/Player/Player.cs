using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.SceneManagement;

// TODO consider rewiring
using RPG.CameraUI;
using RPG.Core;
using RPG.Weapons;

namespace RPG.Characters
{
	public class Player : MonoBehaviour, IDamagable
	{
		[SerializeField] float maxHealthPoints = 100f;
		[SerializeField] float baseDamage = 10f;

		[SerializeField] Weapon weaponInUse = null;
		[SerializeField] AnimatorOverrideController animatorOverrideController = null;
		[SerializeField] AudioClip deathSound = null;

		[SerializeField] SpecialAbility[] abilities;

		Animator animator;
		CameraRaycaster cameraRaycaster;
		float currentHealthPoints;
		float lastHitTime = 0f;
		bool isDead = false;

		public bool GetIsDead()
		{
			return isDead;
		}

		public void TakeDamage ( float damage )
		{
			ReduceHeath( damage );
			isDead = ( currentHealthPoints - damage <= 0 );
			if ( isDead )
			{
				StartCoroutine( KillPlayer() );
			}
		}

		IEnumerator KillPlayer ()
		{
			isDead = true;
			if ( deathSound )
			{
				AudioSource.PlayClipAtPoint( deathSound, transform.position );
			}
			animator.SetBool( "Death", true );
			yield return new WaitForSecondsRealtime (3f);
			SceneManager.LoadScene( 0 );
		}

		private void ReduceHeath( float damage )
		{
			currentHealthPoints = Mathf.Clamp( currentHealthPoints - damage, 0f, maxHealthPoints );
		}

		public float healthAsPercentage
		{
			get { return currentHealthPoints / maxHealthPoints;	}
		}

		void Start ()
		{
			RegisterForMouseClick();
			SetCurrentMaxHealth();
			PutWeaponInHand();
			SetupRuntimeAnimator();

			abilities[0].AttachComponentTo( gameObject );
		}

		private void SetCurrentMaxHealth ()
		{
			currentHealthPoints = maxHealthPoints;
		}

		private void SetupRuntimeAnimator ()
		{
			animator = GetComponent<Animator> ();
			animator.runtimeAnimatorController = animatorOverrideController;
			animatorOverrideController ["DEFAULT ATTACK"] = weaponInUse.GetAttackAnimClip (); // TODO remove paramater
		}

		private void RegisterForMouseClick ()
		{
			cameraRaycaster = FindObjectOfType<CameraRaycaster> ();
			cameraRaycaster.OnMouseOverEnemyObservers += OnMouseOverEnemy;
		}

		void OnMouseOverEnemy ( Enemy enemy )
		{
			if ( !isDead )
			{
				if ( Input.GetMouseButton( 0 ) && IsTargetInRange( enemy.gameObject ) )
				{
					AttackTarget( enemy );
				}
				else if ( Input.GetMouseButtonDown( 1 ) ) //TODO check for ability range
				{
					AttemptSpecialAbility( 0, enemy );
				}
			}
		}

		private void AttemptSpecialAbility (int abilityIndex, Enemy enemy )
		{
			var energyComponent = GetComponent<Energy> ();
			var energyCost = abilities [abilityIndex].GetEnergyCost ();

			if (energyComponent.IsEnergyAvailable( energyCost ) )
			{
				energyComponent.ConsumeEnergy ( energyCost );

				var abilityParams = new AbilityUseParams ( enemy, baseDamage );
				abilities [abilityIndex].Use ( abilityParams );				
			}
		}

		private void PutWeaponInHand ()
		{
			var weaponPrefab = weaponInUse.GetWeaponPrefab ();
			GameObject dominantHand = RequestDominantHand ();
			var weapon = Instantiate ( weaponPrefab, dominantHand.transform );
			weapon.transform.localPosition = weaponInUse.gripTransform.localPosition;
			weapon.transform.localRotation = weaponInUse.gripTransform.localRotation;
		}

		private GameObject RequestDominantHand ()
		{
			var dominantHands = GetComponentsInChildren<DominantHand> ();
			int numberOfDominantHands = dominantHands.Length;
			Assert.IsFalse ( numberOfDominantHands <= 0, "No dominant hand found on player, please add one." );
			Assert.IsFalse ( numberOfDominantHands > 1, "Multiple dominant hand scripts on player, please remove one." );
			return dominantHands [0].gameObject;
		}

		private bool IsTargetInRange ( GameObject target )
		{
			float distanceToTarget = (target.transform.position - transform.position).magnitude;
			return distanceToTarget <= weaponInUse.GetMaxAttackRange();
		}

		private void AttackTarget ( Enemy enemy  )
		{
			Vector3 faceTheEnemy = (enemy.transform.position - transform.position);
			transform.rotation = Quaternion.Slerp ( transform.rotation, Quaternion.LookRotation ( faceTheEnemy ), 0.2f );

			IDamagable damagableComponent = enemy.GetComponent<IDamagable> ();
			if ( damagableComponent != null && Time.time - lastHitTime > weaponInUse.GetMinTimeBetweenHits() )
			{
				animator.SetTrigger ( "Attack" ); // TODO make const
				damagableComponent.TakeDamage ( baseDamage );
				lastHitTime = Time.time;
			}
		}
	}
}