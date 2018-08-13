using System;
using UnityEngine;
using UnityEngine.UI;

namespace RPG.Characters
{
	public class SpecialAbilities : MonoBehaviour
	{
		[SerializeField] AbilityConfig [] abilities;
		[SerializeField] Image energyBar;
		[SerializeField] float maxEnergyPoints = 100f;
		[SerializeField] float regenPointsPerSecond = 10f;
		// todo out of energy sound

		AudioSource audioSource;
		float currentEnergyPoints;

		float energyAsPercent { get { return currentEnergyPoints / maxEnergyPoints; } }

		public void ConsumeEnergy ( float amount )
		{
			float newEnergyPoints = currentEnergyPoints - amount;
			currentEnergyPoints = Mathf.Clamp ( newEnergyPoints, 0, maxEnergyPoints );
		}

		public int GetNumberOfAbilities ()
		{
			return abilities.Length;
		}

		public void AttemptSpecialAbility ( int abilityIndex )
		{
			var energyComponent = GetComponent<SpecialAbilities> ();
			var energyCost = abilities [abilityIndex].GetEnergyCost ();

			if ( energyCost <= currentEnergyPoints )
			{
				energyComponent.ConsumeEnergy ( energyCost );

				print ( "using special ability " + abilityIndex );
				/*var abilityParams = new AbilityUseParams ( currentEnemy, baseDamage );
				abilities [abilityIndex].Use ( abilityParams );*/
			}
			else
			{
				// todo play out of energy sound
			}
		}

		void Start ()
		{
			audioSource = GetComponent<AudioSource> ();
			currentEnergyPoints = maxEnergyPoints;
			AttachInitialAbilities ();
			UpdateEnergyBar ();
		}

		void Update ()
		{
			if ( currentEnergyPoints < maxEnergyPoints )
			{
				RegenerateEnergy ();
				UpdateEnergyBar ();
			}
		}

		void RegenerateEnergy ()
		{
			float newEnergyPoints = currentEnergyPoints + regenPointsPerSecond * Time.deltaTime;
			currentEnergyPoints = Mathf.Clamp ( newEnergyPoints, 0, maxEnergyPoints );
		}

		void UpdateEnergyBar ()
		{
			energyBar.fillAmount = energyAsPercent;
		}

		void AttachInitialAbilities ()
		{
			for ( int abilityIndex = 0; abilityIndex < abilities.Length; abilityIndex++ )
			{
				abilities [abilityIndex].AttachAbilityTo ( gameObject );
			}
		}
	}
}