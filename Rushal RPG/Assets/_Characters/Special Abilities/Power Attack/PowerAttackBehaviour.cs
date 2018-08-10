using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPG.Characters
{
	public class PowerAttackBehaviour : MonoBehaviour, ISpecialAbility
	{
		PowerAttackConfig config;

		public void SetConfig( PowerAttackConfig configToSet )
		{
			config = configToSet;
		}

		void Start ()
		{

		}

		void Update ()
		{

		}

		public void Use ( AbilityUseParams useParams )
		{
			DealDamage ( useParams );
			PlayParticleEffect ();
		}

		private void DealDamage ( AbilityUseParams useParams )
		{
			float damageToDeal = useParams.baseDamage + config.GetExtraDamage ();
			useParams.target.TakeDamage ( damageToDeal );
		}

		private void PlayParticleEffect ()
		{
			var prefab = Instantiate ( config.GetParticlePrefab (), transform.position, Quaternion.identity );
			//prefab.transform.parent = transform;
			ParticleSystem myParticleSystem = prefab.GetComponent<ParticleSystem> ();
			myParticleSystem.Play ();
			Destroy ( prefab, myParticleSystem.main.duration );
		}
	}
}