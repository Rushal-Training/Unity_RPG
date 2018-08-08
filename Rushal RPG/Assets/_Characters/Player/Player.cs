using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

// TODO consider rewiring
using RPG.CameraUI;
using RPG.Core;
using RPG.Weapons;

namespace RPG.Characters
{
	public class Player : MonoBehaviour, IDamagable
	{
		[SerializeField] float maxHealthPoints = 100f;
		[SerializeField] float meleeDamagePerClick = 10f;

		[SerializeField] Weapon weaponInUse = null;
		[SerializeField] AnimatorOverrideController animatorOverrideController = null;

		Animator animator;
		CameraRaycaster cameraRaycaster;
		float currentHealthPoints;
		float lastHitTime = 0f;

		public void TakeDamage ( float damage )
		{
			currentHealthPoints = Mathf.Clamp ( currentHealthPoints - damage, 0f, maxHealthPoints );
			//if ( currentHealthPoints <= 0 ) { Destroy( gameObject );  }
		}

		public float healthAsPercentage
		{
			get { return currentHealthPoints / maxHealthPoints;	}
		}

		void Start ()
		{
			RegisterForMouseClick ();
			SetCurrentMaxHealth ();
			PutWeaponInHand ();
			SetupRuntimeAnimator ();
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
			if (Input.GetMouseButton( 0 ) && IsTargetInRange ( enemy.gameObject ) )
			{
				AttackTarget ( enemy );
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
				animator.SetTrigger ( "Attack" );
				damagableComponent.TakeDamage ( meleeDamagePerClick );
				lastHitTime = Time.time;
			}
		}
	}
}