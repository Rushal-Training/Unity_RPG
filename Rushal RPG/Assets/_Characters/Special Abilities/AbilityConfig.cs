﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RPG.Core;

namespace RPG.Characters
{
	public abstract class AbilityConfig : ScriptableObject
	{
		[Header ( "Special Ability General" )]
		[SerializeField] float energyCost = 15f;
		[SerializeField] GameObject particlePrefab;
		[SerializeField] AudioClip[] audioClips;
		[SerializeField] AnimationClip abilityAnimation;

		protected AbilityBehaviour behaviour;

		public abstract AbilityBehaviour GetBehaviourComponent( GameObject objectToAttachTo );

		public void AttachAbilityTo ( GameObject objectToAttachTo )
		{
			AbilityBehaviour behaviourComponent = GetBehaviourComponent( objectToAttachTo );
			behaviourComponent.SetConfig( this );
			behaviour = behaviourComponent;
		}

		public float GetEnergyCost ()
		{
			return energyCost;
		}

		public GameObject GetParticlePrefab ()
		{
			return particlePrefab;
		}

		public AudioClip GetRandomAbilitySound ()
		{
			return audioClips[Random.Range( 0, audioClips.Length )];
		}

		public void Use ( GameObject target )
		{
			behaviour.Use ( target );
		}

		public AnimationClip GetAbilityAnimation()
		{
			return abilityAnimation;
		}
	}
}