using System;
using UnityEngine;
using UnityEngine.Assertions;

using RPG.CameraUI;

namespace RPG.Characters
{
	public class PlayerControl : MonoBehaviour  // todo extract weapon system
	{
		
		[SerializeField] float baseDamage = 10f;
		[Range (.1f, 1.0f)] [SerializeField] float criticalHitChance = .1f;
		[SerializeField] float criticalHitMultiplier = 1.25f;

		[SerializeField] Weapon currentWeaponConfig;
		[SerializeField] AnimatorOverrideController animatorOverrideController;
		
		[SerializeField] ParticleSystem criticalHitParticle;

		const string ATTACK_TRIGGER = "Attack";
		const string DEFAULT_ATTACK = "DEFAULT ATTACK";

		CameraRaycaster cameraRaycaster;
		Character character;
		Enemy currentEnemy ;
		GameObject weaponObject;
		SpecialAbilities abilities;
		float lastHitTime = 0f;


		public void PutWeaponInHand (Weapon weaponToUse )
		{
			currentWeaponConfig = weaponToUse;
			var weaponPrefab = weaponToUse.GetWeaponPrefab ();
			GameObject dominantHand = RequestDominantHand ();
			Destroy ( weaponObject );
			weaponObject = Instantiate ( weaponPrefab, dominantHand.transform );
			weaponObject.transform.localPosition = weaponToUse.gripTransform.localPosition;
			weaponObject.transform.localRotation = weaponToUse.gripTransform.localRotation;
		}

		void Start ()
		{
			abilities = GetComponent<SpecialAbilities> ();
			character = GetComponent<Character> ();

			RegisterForMouseEvents();
			PutWeaponInHand(currentWeaponConfig); // todo move to weap system
			SetAttackAnimation(); // todo move to weap system
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
				AttackTarget ();
			}
			else if ( Input.GetMouseButtonDown ( 1 ) ) //TODO check for ability range
			{
				abilities.AttemptSpecialAbility ( 0 );
			}
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

		private void AttackTarget ()
		{
			Vector3 faceTheEnemy = (currentEnemy.transform.position - transform.position);
			transform.rotation = Quaternion.Slerp ( transform.rotation, Quaternion.LookRotation ( faceTheEnemy ), 0.2f );

			HealthSystem damagableComponent = currentEnemy.GetComponent<HealthSystem> ();
			if ( damagableComponent && Time.time - lastHitTime > currentWeaponConfig.GetMinTimeBetweenHits () )
			{
				SetAttackAnimation ();
				//animator.SetTrigger ( ATTACK_TRIGGER );
				damagableComponent.TakeDamage ( CalculateDamage () );
				lastHitTime = Time.time;
			}
		}

		bool IsTargetInRange ( GameObject target )
		{
			float distanceToTarget = (target.transform.position - transform.position).magnitude;
			return distanceToTarget <= currentWeaponConfig.GetMaxAttackRange ();
		}


		// todo move to weapon system
		void SetAttackAnimation ()
		{
			Animator animator = GetComponent<Animator> ();
			animator.runtimeAnimatorController = animatorOverrideController;
			animatorOverrideController [DEFAULT_ATTACK] = currentWeaponConfig.GetAttackAnimClip (); // TODO remove paramater
		}
		
		GameObject RequestDominantHand ()
		{
			var dominantHands = GetComponentsInChildren<DominantHand> ();
			int numberOfDominantHands = dominantHands.Length;
			Assert.IsFalse ( numberOfDominantHands <= 0, "No dominant hand found on player, please add one." );
			Assert.IsFalse ( numberOfDominantHands > 1, "Multiple dominant hand scripts on player, please remove one." );
			return dominantHands [0].gameObject;
		}

		float CalculateDamage ()
		{
			float totalDamage = baseDamage + currentWeaponConfig.GetAdditionalDamage ();
			if ( UnityEngine.Random.Range(0f, 1.0f) <= criticalHitChance )
			{
				criticalHitParticle.Play ();
				totalDamage *= criticalHitMultiplier;
			}
			return totalDamage;
		}
	}
}