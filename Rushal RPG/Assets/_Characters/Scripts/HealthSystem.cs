﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

namespace RPG.Characters
{
	public class HealthSystem : MonoBehaviour
	{
		[SerializeField] float deathVanishSeconds = 2f;
		[SerializeField] float maxHealthPoints = 100f;
		[SerializeField] Image healthBar;
		[SerializeField] AudioClip [] damageSounds = null;
		[SerializeField] AudioClip [] deathSounds = null;

		const string DEATH_TRIGGER = "Death";

		Animator animator;
		AudioSource audioSource;
		Character characterMovement;

		float currentHealthPoints;

		public float healthAsPercentage
		{
			get { return currentHealthPoints / maxHealthPoints; }
		}

		public void TakeDamage ( float damage )
		{
			bool characterDies = (currentHealthPoints - damage <= 0);
			currentHealthPoints = Mathf.Clamp ( currentHealthPoints - damage, 0f, maxHealthPoints );
			var clip = damageSounds [UnityEngine.Random.Range ( 0, damageSounds.Length )];
			audioSource.PlayOneShot ( clip );

			if ( characterDies )
			{
				StartCoroutine ( KillCharacter () );
			}
		}

		IEnumerator KillCharacter ()
		{
			characterMovement.Kill ();
			animator.SetTrigger ( DEATH_TRIGGER );

			audioSource.clip = deathSounds [UnityEngine.Random.Range ( 0, deathSounds.Length )];
			audioSource.Play ();
			yield return new WaitForSecondsRealtime ( audioSource.clip.length );

			var playerComponent = GetComponent<PlayerControl> ();
			if ( playerComponent && playerComponent.isActiveAndEnabled )
			{
				SceneManager.LoadScene ( 0 );
			}
			else
			{
				Destroy ( gameObject, deathVanishSeconds );
			}
		}

		public void Heal ( float points )
		{
			// todo only if not dead
			currentHealthPoints = Mathf.Clamp ( currentHealthPoints + points, 0f, maxHealthPoints );
		}

		void Start ()
		{
			animator = GetComponent<Animator> ();
			audioSource = GetComponent<AudioSource> ();
			characterMovement = GetComponent<Character> ();

			currentHealthPoints = maxHealthPoints;
		}

		void Update ()
		{
			UpdateHealthBar ();
		}

		void UpdateHealthBar()
		{
			if ( healthBar )
			{
				healthBar.fillAmount = healthAsPercentage;
			}
		}
	}
}