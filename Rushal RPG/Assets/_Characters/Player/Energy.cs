using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace RPG.Characters
{
	public class Energy : MonoBehaviour
	{
		[SerializeField] RawImage energyBar = null;
		[SerializeField] float maxEnergyPoints = 100f;

		float currentEnergyPoints;

		void Start ()
		{
			currentEnergyPoints = maxEnergyPoints;
		}

		void Update ()
		{

		}

		public bool IsEnergyAvailable ( float amount )
		{
			return amount <= currentEnergyPoints;
		}

		public void ConsumeEnergy ( float amount )
		{
			float newEnergyPoints = currentEnergyPoints - amount;
			currentEnergyPoints = Mathf.Clamp ( newEnergyPoints, 0, maxEnergyPoints );
			UpdateEnergyBar ();
		}

		private void UpdateEnergyBar ()
		{
			float xValue = -((currentEnergyPoints / maxEnergyPoints) / 2f) - 0.5f;
			energyBar.uvRect = new Rect ( xValue, 0f, 0.5f, 1f );
		}
	}
}