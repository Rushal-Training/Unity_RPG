using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPG.Characters
{
	public class SelfHealBehaviour : AbilityBehaviour
	{
		Player player ;

		public override void Use ( AbilityUseParams useParams )
		{
			var playerHealth = player.GetComponent<HealthSystem> ();

			PlayAbilitySound();
			playerHealth.Heal ( (config as SelfHealConfig).GetHealAmount () );
			PlayParticleEffect ();
		}

		void Start ()
		{
			player = GetComponent<Player> ();
		}
	}
}