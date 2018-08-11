using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPG.Characters
{
	public class SelfHealBehaviour : AbilityBehaviour
	{
		Player player = null;

		public override void Use ( AbilityUseParams useParams )
		{
			PlayAbilitySound();
			HealTarget ( useParams );
			PlayParticleEffect ();
		}

		private void Start ()
		{
			player = GetComponent<Player> ();
		}

		private void HealTarget ( AbilityUseParams useParams )
		{
			player.Heal ( ( config as SelfHealConfig ).GetHealAmount () );
		}
	}
}