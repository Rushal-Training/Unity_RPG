using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.SceneManagement;

// TODO consider rewiring
using RPG.CameraUI;
using RPG.Core;

namespace RPG.Characters
{
	public class Player : MonoBehaviour, IDamagable
	{
		[SerializeField] float maxHealthPoints = 100f;
		[SerializeField] float baseDamage = 10f;
		[Range (.1f, 1.0f)] [SerializeField] float criticalHitChance = .1f;
		[SerializeField] float criticalHitMultiplier = 1.25f;

		[SerializeField] Weapon currentWeaponConfig = null;
		[SerializeField] AnimatorOverrideController animatorOverrideController = null;
		[SerializeField] AudioClip [] damageSounds = null;
		[SerializeField] AudioClip [] deathSounds = null;
		[SerializeField] ParticleSystem criticalHitParticle;


		[SerializeField] AbilityConfig [] abilities;

		const string ATTACK_TRIGGER = "Attack";
		const string DEATH_TRIGGER = "Death";
		const string DEFAULT_ATTACK = "DEFAULT ATTACK";

		Animator animator = null;
		AudioSource audioSource = null;
		CameraRaycaster cameraRaycaster = null;
		Enemy currentEnemy = null;
		GameObject weaponObject = null;
		float currentHealthPoints;
		float lastHitTime = 0f;

		public float healthAsPercentage
		{
			get { return currentHealthPoints / maxHealthPoints; }
		}

		public void TakeDamage ( float damage )
		{
			currentHealthPoints = Mathf.Clamp ( currentHealthPoints - damage, 0f, maxHealthPoints );
			audioSource.clip = damageSounds [UnityEngine.Random.Range ( 0, damageSounds.Length )];
			audioSource.Play ();

			if ( currentHealthPoints <= 0 )
			{
				StartCoroutine ( KillPlayer () );
			}
		}

		public void Heal ( float points )
		{
			currentHealthPoints = Mathf.Clamp ( currentHealthPoints + points, 0f, maxHealthPoints );
		}

		IEnumerator KillPlayer ()
		{
			animator.SetTrigger ( DEATH_TRIGGER );

			audioSource.clip = deathSounds [UnityEngine.Random.Range ( 0, deathSounds.Length )];
			audioSource.Play ();
			yield return new WaitForSecondsRealtime ( audioSource.clip.length );

			SceneManager.LoadScene( 0 );
		}

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
			audioSource = GetComponent<AudioSource> ();
			RegisterForMouseClick();
			SetCurrentMaxHealth();
			PutWeaponInHand(currentWeaponConfig);
			SetAttackAnimation();
			AttachInitialAbilities ();
		}

		void Update ()
		{
			if ( healthAsPercentage > Mathf.Epsilon )
			{
				ScanForAbilityKeyDown ();
			}
		}

		private void AttachInitialAbilities ()
		{
			for ( int abilityIndex = 0; abilityIndex < abilities.Length; abilityIndex++ )
			{
				abilities [abilityIndex].AttachAbilityTo ( gameObject );
			}
		}

		private void ScanForAbilityKeyDown ()
		{
			for ( int keyIndex = 1; keyIndex < abilities.Length; keyIndex++ )
			{
				if ( Input.GetKeyDown( keyIndex.ToString() ) )
				{
					AttemptSpecialAbility ( keyIndex );
				}
			}
		}

		private void SetCurrentMaxHealth ()
		{
			currentHealthPoints = maxHealthPoints;
		}

		private void SetAttackAnimation ()
		{
			animator = GetComponent<Animator> ();
			animator.runtimeAnimatorController = animatorOverrideController;
			animatorOverrideController [DEFAULT_ATTACK] = currentWeaponConfig.GetAttackAnimClip (); // TODO remove paramater
		}

		private void RegisterForMouseClick ()
		{
			cameraRaycaster = FindObjectOfType<CameraRaycaster> ();
			cameraRaycaster.OnMouseOverEnemyObservers += OnMouseOverEnemy;
		}

		void OnMouseOverEnemy ( Enemy enemy )
		{
			currentEnemy = enemy;
			if ( Input.GetMouseButton( 0 ) && IsTargetInRange( enemy.gameObject ) )
			{
				AttackTarget();
			}
			else if ( Input.GetMouseButtonDown( 1 ) ) //TODO check for ability range
			{
				AttemptSpecialAbility( 0 );
			}
		}

		private void AttemptSpecialAbility (int abilityIndex )
		{
			var energyComponent = GetComponent<Energy> ();
			var energyCost = abilities [abilityIndex].GetEnergyCost ();

			if (energyComponent.IsEnergyAvailable( energyCost ) )
			{
				energyComponent.ConsumeEnergy ( energyCost );

				var abilityParams = new AbilityUseParams ( currentEnemy, baseDamage );
				abilities [abilityIndex].Use ( abilityParams );				
			}
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
			return distanceToTarget <= currentWeaponConfig.GetMaxAttackRange();
		}

		private void AttackTarget ()
		{
			Vector3 faceTheEnemy = (currentEnemy.transform.position - transform.position);
			transform.rotation = Quaternion.Slerp ( transform.rotation, Quaternion.LookRotation ( faceTheEnemy ), 0.2f );

			IDamagable damagableComponent = currentEnemy.GetComponent<IDamagable> ();
			if ( damagableComponent != null && Time.time - lastHitTime > currentWeaponConfig.GetMinTimeBetweenHits() )
			{
				SetAttackAnimation ();
				animator.SetTrigger ( ATTACK_TRIGGER );
				damagableComponent.TakeDamage ( CalculateDamage () );
				lastHitTime = Time.time;
			}
		}

		private float CalculateDamage ()
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