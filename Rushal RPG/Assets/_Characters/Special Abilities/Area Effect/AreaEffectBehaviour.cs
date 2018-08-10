using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RPG.Core;
using System;

namespace RPG.Characters
{
	public class AreaEffectBehaviour : MonoBehaviour, ISpecialAbility
	{
		AreaEffectConfig config;

		public void SetConfig ( AreaEffectConfig configToSet )
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
			DealRadialDamage ( useParams );
			PlayParticleEffect ();
		}

		private void PlayParticleEffect ()
		{
			var prefab = Instantiate ( config.GetParticlePrefab (), transform.position, config.GetParticlePrefab ().transform.rotation );
			//prefab.transform.parent = transform;
			ParticleSystem myParticleSystem = prefab.GetComponent<ParticleSystem> ();
			myParticleSystem.Play ();
			Destroy ( prefab, myParticleSystem.main.duration );
		}

		private void DealRadialDamage ( AbilityUseParams useParams )
		{
			var hits = Physics.SphereCastAll ( transform.position, config.GetRaduis (), Vector3.forward, config.GetRaduis () );

			foreach ( var hit in hits )
			{
				var damagable = hit.collider.gameObject.GetComponent<IDamagable> ();
				bool hitPlayer = hit.collider.gameObject.GetComponent<Player> ();
				if ( damagable != null && !hitPlayer )
				{
					float damageToDeal = useParams.baseDamage + config.GetDamageToEnemies ();
					damagable.TakeDamage ( damageToDeal );
				}
			}
		}
	}
}