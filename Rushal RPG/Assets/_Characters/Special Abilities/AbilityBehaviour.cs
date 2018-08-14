using System.Collections;
using UnityEngine;

namespace RPG.Characters
{
	public abstract class AbilityBehaviour : MonoBehaviour
	{
		protected AbilityConfig config;

		const string ATTACK_TRIGGER = "Attack";
		const string DEFAULT_ATTACK_STATE = "DEFAULT_ATTACK";
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

		protected  void PlayAbilityAnimation()
		{
			var animatorOverrideController = GetComponent<Character>().getOverrideController;
			var animator = GetComponent<Animator>();
			animator.runtimeAnimatorController = animatorOverrideController;
			animatorOverrideController[DEFAULT_ATTACK_STATE] = config.GetAbilityAnimation();
			animator.SetTrigger( ATTACK_TRIGGER );

		}

		protected void PlayAbilitySound()
		{
			var abilitySound = config.GetRandomAbilitySound();
			var audioSource = GetComponent<AudioSource>();
			audioSource.PlayOneShot( abilitySound );
		}
	}
}