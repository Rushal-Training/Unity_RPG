﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPG.Characters
{
	[CreateAssetMenu ( menuName = "RPG/Special Ability/Area Effect" )]
	public class AreaEffectConfig : SpecialAbility
	{
		[Header ( "Area Effect Specific" )]
		[SerializeField] float radius = 5f;
		[SerializeField] float damageToEachTarget = 15f;

		public override void AttachComponentTo ( GameObject gameObjectToAttatchTo )
		{
			var behaviourComponent = gameObjectToAttatchTo.AddComponent<AreaEffectBehaviour> ();
			behaviourComponent.SetConfig ( this );
			behaviour = behaviourComponent;
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