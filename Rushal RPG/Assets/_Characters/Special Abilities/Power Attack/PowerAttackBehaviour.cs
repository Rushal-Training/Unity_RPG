using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace RPG.Characters
{
	public class PowerAttackBehaviour : MonoBehaviour, ISpecialAbility
	{

		PowerAttackConfig config;

		public void SetConfig( PowerAttackConfig configToSet )
		{
			config = configToSet;
		}

		void Start ()
		{

		}

		void Update ()
		{

		}

		public void Use ()
		{

		}
	}
}