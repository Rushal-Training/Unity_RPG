using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RPG.Core;

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
			var hits = Physics.SphereCastAll ( transform.position, config.GetRaduis (), Vector3.forward, config.GetRaduis());

			foreach (var hit in hits)
			{
				var damagable = hit.collider.gameObject.GetComponent<IDamagable> ();
				if ( damagable != null )
				{
					float damageToDeal = useParams.baseDamage + config.GetDamageToEnemies ();
					damagable.TakeDamage ( damageToDeal );
				}
			}
		}
	}
}