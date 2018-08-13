﻿using System.Collections;
using UnityEngine;

namespace RPG.Characters
{
	public abstract class AbilityBehaviour : MonoBehaviour
	{
		protected AbilityConfig config;

		const float PARTICLE_CLEAN_UP_DELAY = 20f;

		public abstract void Use( GameObject target );

		public void SetConfig( AbilityConfig configToSet )
		{
			config = configToSet;
		}

		protected void PlayParticleEffect()
		{
			var particleObject = Instantiate( config.GetParticlePrefab(), transform.position, config.GetParticlePrefab().transform.rotation );
			particleObject.transform.parent = transform;
			particleObject.GetComponent<ParticleSystem>().Play();
			StartCoroutine( DestroyParticleWhenFinished( particleObject ) );
		}

		IEnumerator DestroyParticleWhenFinished( GameObject particlePrefab )
		{
			while ( particlePrefab.GetComponent<ParticleSystem>().isPlaying )
			{
				yield return new WaitForSecondsRealtime( PARTICLE_CLEAN_UP_DELAY );
			}
			Destroy( particlePrefab );
			yield return new WaitForEndOfFrame();
		}

		protected void PlayAbilitySound()
		{
			var abilitySound = config.GetRandomAbilitySound();
			var audioSource = GetComponent<AudioSource>();
			audioSource.PlayOneShot( abilitySound );
		}
	}
}