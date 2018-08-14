using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPG.Characters
{
	public class SelfHealBehaviour : AbilityBehaviour
	{
		PlayerControl player ;

		public override void Use ( GameObject target )
		{
			var playerHealth = player.GetComponent<HealthSystem> ();

			PlayAbilitySound();
			playerHealth.Heal ( (config as SelfHealConfig).GetHealAmount () );
			PlayParticleEffect ();
			PlayAbilityAnimation();
		}

		void Start ()
		{
			player = GetComponent<PlayerControl> ();
		}
	}
}