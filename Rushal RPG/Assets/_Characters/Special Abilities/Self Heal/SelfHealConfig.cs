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

		public override void AttachComponentTo (GameObject gameObjectToAttatchTo)
		{
			var behaviourComponent = gameObjectToAttatchTo.AddComponent<SelfHealBehaviour> ();
			behaviourComponent.SetConfig ( this );
			behaviour = behaviourComponent;
		}

		public float GetHealAmount ()
		{
			return healAmount;
		}
	}
}