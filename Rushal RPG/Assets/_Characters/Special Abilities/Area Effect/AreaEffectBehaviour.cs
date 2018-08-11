using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RPG.Core;

namespace RPG.Characters
{
	public class AreaEffectBehaviour : AbilityBehaviour
	{
		public override void Use ( AbilityUseParams useParams )
		{
			PlayAbilitySound();
			DealRadialDamage ( useParams );
			PlayParticleEffect ();
		}

		private void DealRadialDamage ( AbilityUseParams useParams )
		{
			var hits = Physics.SphereCastAll ( transform.position, (config as AreaEffectConfig).GetRaduis (), Vector3.forward, ( config as AreaEffectConfig ).GetRaduis () );

			foreach ( var hit in hits )
			{
				var damagable = hit.collider.gameObject.GetComponent<IDamagable> ();
				bool hitPlayer = hit.collider.gameObject.GetComponent<Player> ();
				if ( damagable != null && !hitPlayer )
				{
					float damageToDeal = useParams.baseDamage + ( config as AreaEffectConfig ).GetDamageToEnemies ();
					damagable.TakeDamage ( damageToDeal );
				}
			}
		}
	}
}