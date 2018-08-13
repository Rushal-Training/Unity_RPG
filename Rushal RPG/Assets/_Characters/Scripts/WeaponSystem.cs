using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

namespace RPG.Characters
{
	public class WeaponSystem : MonoBehaviour
	{
		[SerializeField] float baseDamage = 10f;

		[Range ( .1f, 1.0f )] [SerializeField] float criticalHitChance = .1f;
		[SerializeField] float criticalHitMultiplier = 1.25f;
		[SerializeField] ParticleSystem criticalHitParticle;

		[SerializeField] WeaponConfig currentWeaponConfig;

		const string ATTACK_TRIGGER = "Attack";
		const string DEFAULT_ATTACK = "DEFAULT ATTACK";

		Animator animator;
		Character character;
		GameObject target;
		GameObject weaponObject;

		float lastHitTime;

		public WeaponConfig currentWeapon { get { return currentWeaponConfig; } }

		public void PutWeaponInHand ( WeaponConfig weaponToUse )
		{
			currentWeaponConfig = weaponToUse;
			var weaponPrefab = weaponToUse.GetWeaponPrefab ();
			GameObject dominantHand = RequestDominantHand ();
			Destroy ( weaponObject );
			weaponObject = Instantiate ( weaponPrefab, dominantHand.transform );
			weaponObject.transform.localPosition = weaponToUse.gripTransform.localPosition;
			weaponObject.transform.localRotation = weaponToUse.gripTransform.localRotation;
		}

		public void AttackTarget ( GameObject targetToAttack )
		{
			target = targetToAttack;
			print ( "attacking " + targetToAttack );
			// todo use repeat attack coroutine
		}

		void Start ()
		{
			animator = GetComponent<Animator> ();
			character = GetComponent<Character> ();

			PutWeaponInHand ( currentWeaponConfig );
			SetAttackAnimation ();
		}

		void Update ()
		{

		}

		private void AttackTarget ()
		{
			/*Vector3 faceTheEnemy = (currentEnemy.transform.position - transform.position);
			transform.rotation = Quaternion.Slerp ( transform.rotation, Quaternion.LookRotation ( faceTheEnemy ), 0.2f );

			HealthSystem damagableComponent = currentEnemy.GetComponent<HealthSystem> ();
			if ( damagableComponent && Time.time - lastHitTime > currentWeaponConfig.GetMinTimeBetweenHits () )
			{
				SetAttackAnimation ();
				//animator.SetTrigger ( ATTACK_TRIGGER );
				damagableComponent.TakeDamage ( CalculateDamage () );
				lastHitTime = Time.time;
			}*/
			
			if ( Time.time - lastHitTime > currentWeaponConfig.GetMinTimeBetweenHits () )
			{
				SetAttackAnimation ();
				animator.SetTrigger ( ATTACK_TRIGGER );
				lastHitTime = Time.time;
			}
		}

		void SetAttackAnimation ()
		{
			animator = GetComponent<Animator> ();
			var animatorOverrideController = character.getOverrideController;
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
			if ( UnityEngine.Random.Range ( 0f, 1.0f ) <= criticalHitChance )
			{
				criticalHitParticle.Play ();
				totalDamage *= criticalHitMultiplier;
			}
			return totalDamage;
		}
	}
}