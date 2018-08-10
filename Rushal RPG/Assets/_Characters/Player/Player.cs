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
		[SerializeField] AudioClip [] damageSounds = null;
		[SerializeField] AudioClip [] deathSounds = null;

		[SerializeField] AbilityConfig [] abilities;

		const string ATTACK_TRIGGER = "Attack";
		const string DEATH_TRIGGER = "Death";

		Animator animator = null;
		AudioSource audioSource = null;
		CameraRaycaster cameraRaycaster = null;
		Enemy currentEnemy = null;
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

		void Start ()
		{
			audioSource = GetComponent<AudioSource> ();
			RegisterForMouseClick();
			SetCurrentMaxHealth();
			PutWeaponInHand();
			SetupRuntimeAnimator();
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
				abilities [abilityIndex].AttachComponentTo ( gameObject );
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

		private void AttackTarget ()
		{
			Vector3 faceTheEnemy = (currentEnemy.transform.position - transform.position);
			transform.rotation = Quaternion.Slerp ( transform.rotation, Quaternion.LookRotation ( faceTheEnemy ), 0.2f );

			IDamagable damagableComponent = currentEnemy.GetComponent<IDamagable> ();
			if ( damagableComponent != null && Time.time - lastHitTime > weaponInUse.GetMinTimeBetweenHits() )
			{
				animator.SetTrigger ( ATTACK_TRIGGER );
				damagableComponent.TakeDamage ( baseDamage );
				lastHitTime = Time.time;
			}
		}
	}
}