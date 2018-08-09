using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace RPG.Characters
{
	public class Energy : MonoBehaviour
	{
		[SerializeField] Image energyOrb = null;
		[SerializeField] float maxEnergyPoints = 100f;
		[SerializeField] float regenPointsPerSecond = 10f;

		float currentEnergyPoints;

		public bool IsEnergyAvailable ( float amount )
		{
			return amount <= currentEnergyPoints;
		}

		public void ConsumeEnergy ( float amount )
		{
			float newEnergyPoints = currentEnergyPoints - amount;
			currentEnergyPoints = Mathf.Clamp ( newEnergyPoints, 0, maxEnergyPoints );
		}

		void Start ()
		{
			currentEnergyPoints = maxEnergyPoints;
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

		private void RegenerateEnergy ()
		{
			float newEnergyPoints = currentEnergyPoints + regenPointsPerSecond * Time.deltaTime;
			currentEnergyPoints = Mathf.Clamp ( newEnergyPoints, 0, maxEnergyPoints );
		}

		private void UpdateEnergyBar ()
		{
			energyOrb.fillAmount = EnergyAsPercent ();
		}

		float EnergyAsPercent ()
		{
			return currentEnergyPoints / maxEnergyPoints;
		}
	}
}