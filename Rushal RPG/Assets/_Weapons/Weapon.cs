﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPG.Weapons
{
	[CreateAssetMenu ( menuName = ("RPG/Weapon") )]
	public class Weapon : ScriptableObject
	{
		public Transform gripTransform;

		[SerializeField] float minTimeBetweeHits = 0.5f;
		[SerializeField] float maxAttackRange = 2f;

		[SerializeField] GameObject weaponPrefab;
		[SerializeField] AnimationClip attackAnimation;

		public float GetMinTimeBetweenHits ()
		{
			//TODO consider whether we take animation time into account
			return minTimeBetweeHits;
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