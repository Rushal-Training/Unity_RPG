using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPG.Characters
{
	[CreateAssetMenu ( menuName = ("RPG/Weapon") )]
	public class WeaponConfig : ScriptableObject
	{
		public Transform gripTransform;

		[SerializeField] float timeBetweenAnimationCycles = 0.5f;
		[SerializeField] float maxAttackRange = 2f;
		[SerializeField] float additionalDamage = 10f;
		[SerializeField] float damageDelay = .5f;

		[SerializeField] GameObject weaponPrefab;
		[SerializeField] AnimationClip attackAnimation;

		public float GetDamageDelay()
		{
			return damageDelay;
		}

		public float GetTimeBetweenAnimationCycles ()
		{
			return timeBetweenAnimationCycles;
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