using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;
using RPG.CameraUI; // TODO consider rewiring

namespace RPG.Characters
{
	public class Player : MonoBehaviour, IDamagable
	{
		[SerializeField] int enemyLayer = 10;
		[SerializeField] float maxHealthPoints = 100f;
		[SerializeField] float meleeDamagePerClick = 10f;
		[SerializeField] float minTimeBetweeHits = 0.5f;
		[SerializeField] float maxAttackRange = 2f;

		[SerializeField] Weapon weaponInUse;

		CameraRaycaster cameraRaycaster;
		float currentHealthPoints;
		float lastHitTime = 0f;

		void Start ()
		{
			RegisterForMouseClick ();

			currentHealthPoints = maxHealthPoints;

			PutWeaponInHand ();
		}

		private void RegisterForMouseClick ()
		{
			cameraRaycaster = FindObjectOfType<CameraRaycaster> ();
			cameraRaycaster.notifyMouseClickObservers += OnMouseClick;
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

		void Update ()
		{

		}

		public float healthAsPercentage
		{
			get
			{
				return currentHealthPoints / maxHealthPoints;
			}
		}

		void OnMouseClick ( RaycastHit raycastHit, int layerHit )
		{
			if ( layerHit == enemyLayer )
			{
				var enemy = raycastHit.collider.gameObject;

				if ( (enemy.transform.position - transform.position).magnitude > maxAttackRange ) { return; }

				Vector3 faceTheEnemy = (enemy.transform.position - transform.position);
				transform.rotation = Quaternion.Slerp ( transform.rotation, Quaternion.LookRotation ( faceTheEnemy ), 0.2f );

				IDamagable damagableComponent = enemy.GetComponent<IDamagable> ();
				if ( damagableComponent != null && Time.time - lastHitTime > minTimeBetweeHits )
				{
					damagableComponent.TakeDamage ( meleeDamagePerClick );
					lastHitTime = Time.time;
				}

			}
		}

		public void TakeDamage ( float damage )
		{
			currentHealthPoints = Mathf.Clamp ( currentHealthPoints - damage, 0f, maxHealthPoints );
			//if ( currentHealthPoints <= 0 ) { Destroy( gameObject );  }
		}
	}
}