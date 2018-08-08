using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RPG.Core;

namespace RPG.Characters
{
	public struct AbilityUseParams
	{
		public IDamagable target;
		public float baseDamage;

		public AbilityUseParams ( IDamagable target, float baseDamage )
		{
			this.target = target;
			this.baseDamage = baseDamage;
		}
	}

	public interface ISpecialAbility
	{
		void Use ( AbilityUseParams useParams );
	}

	public abstract class SpecialAbility : ScriptableObject
	{
		[Header ( "Special Ability General" )]
		[SerializeField] float energyCost = 15f;

		protected ISpecialAbility behaviour;

		abstract public void AttachComponentTo ( GameObject gameObjectToAttachTo );

		public float GetEnergyCost ()
		{
			return energyCost;
		}

		public void Use ( AbilityUseParams useParams )
		{
			behaviour.Use ( useParams );
		}
	}
}