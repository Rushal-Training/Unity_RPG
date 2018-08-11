using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPG.Characters
{
	[CreateAssetMenu ( menuName = "RPG/Special Ability/Area Effect" )]
	public class AreaEffectConfig : AbilityConfig
	{
		[Header ( "Area Effect Specific" )]
		[SerializeField] float radius = 5f;
		[SerializeField] float damageToEachTarget = 15f;

		public override AbilityBehaviour GetBehaviourComponent( GameObject objectToAttatchTo )
		{
			return objectToAttatchTo.AddComponent<AreaEffectBehaviour>();
		}

		public float GetRaduis ()
		{
			return radius;
		}

		public float GetDamageToEnemies ()
		{
			return damageToEachTarget;
		}
	}
}