using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPG.Characters
{
	[CreateAssetMenu ( menuName = "RPG/Special Ability/Self Heal" )]
	public class SelfHealConfig : AbilityConfig
	{
		[Header ( "Self Heal Specific" )]
		[SerializeField] float healAmount = 200f;

		public override AbilityBehaviour GetBehaviourComponent( GameObject objectToAttatchTo )
		{
			return objectToAttatchTo.AddComponent<SelfHealBehaviour>();
		}

		public float GetHealAmount ()
		{
			return healAmount;
		}
	}
}