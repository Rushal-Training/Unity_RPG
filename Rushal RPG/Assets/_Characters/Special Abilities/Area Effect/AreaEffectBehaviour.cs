using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RPG.Core;

namespace RPG.Characters
{
	public class AreaEffectBehaviour : AbilityBehaviour
	{
		public override void Use ( GameObject target = null )
		{
			PlayAbilitySound();
			DealRadialDamage ( target );
			PlayParticleEffect ();
		}

		private void DealRadialDamage ( GameObject target )
		{
			var hits = Physics.SphereCastAll ( transform.position, (config as AreaEffectConfig).GetRaduis (), Vector3.forward, ( config as AreaEffectConfig ).GetRaduis () );

			foreach ( var hit in hits )
			{
				var enemy = hit.collider.gameObject.GetComponent<Enemy> ();
				if ( enemy )
				{
					float damageToDeal = ( config as AreaEffectConfig ).GetDamageToEnemies ();
					enemy.TakeDamage ( damageToDeal );
				}
			}
		}
	}
}