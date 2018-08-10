using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPG.Characters
{
	public class SelfHealBehaviour : MonoBehaviour, ISpecialAbility
	{
		AudioSource audioSource = null;
		Player player = null;
		SelfHealConfig config = null;

		public void SetConfig ( SelfHealConfig configToSet )
		{
			config = configToSet;
		}

		public void Use ( AbilityUseParams useParams )
		{
			HealTarget ( useParams );
			PlayParticleEffect ();
		}

		private void Start ()
		{
			audioSource = GetComponent<AudioSource> ();
			player = GetComponent<Player> ();
		}

		private void HealTarget ( AbilityUseParams useParams )
		{
			player.Heal ( config.GetHealAmount () );
			audioSource.clip = config.GetAudioClip ();
			audioSource.Play ();
		}

		private void PlayParticleEffect ()
		{
			var prefab = Instantiate ( config.GetParticlePrefab (), transform.position, Quaternion.identity );
			prefab.transform.parent = transform;
			ParticleSystem myParticleSystem = prefab.GetComponent<ParticleSystem> ();
			myParticleSystem.Play ();
			Destroy ( prefab, myParticleSystem.main.duration + 4 );
		}
	}
}