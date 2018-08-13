using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPG.Characters
{
	[CreateAssetMenu ( menuName = ("RPG/Weapon") )]
	public class WeaponConfig : ScriptableObject
	{
		public Transform gripTransform;

		[SerializeField] float minTimeBetweeHits = 0.5f;
		[SerializeField] float maxAttackRange = 2f;
		[SerializeField] float additionalDamage = 10f;

		[SerializeField] GameObject weaponPrefab;
		[SerializeField] AnimationClip attackAnimation;

		public float GetMinTimeBetweenHits ()
		{
			//TODO consider whether we take animation time into account
			return minTimeBetweeHits;
		}

		public float GetAdditionalDamage ()
		{
			return additionalDamage;
		}

		public float GetMaxAttackRange ()
		{
			return maxAttackRange;
		}
		
		public GameObject GetWeaponPrefab ()
		{
			return weaponPrefab;
		}

		public AnimationClip GetAttackAnimClip ()
		{
			attackAnimation.events = new AnimationEvent [0]; // Remove animation events
			return attackAnimation;
		}
	}
}