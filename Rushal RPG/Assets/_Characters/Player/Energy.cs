using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using RPG.CameraUI;

namespace RPG.Characters
{
	public class Energy : MonoBehaviour
	{
		[SerializeField] RawImage energyBar;
		[SerializeField] float maxEnergyPoints = 100f;
		[SerializeField] float pointsPerHit = 10f;

		float currentEnergyPoints;
		CameraRaycaster cameraRaycaster;

		void Start ()
		{
			cameraRaycaster = FindObjectOfType<CameraRaycaster> ();
			cameraRaycaster.OnMouseOverEnemyObservers += OnMouseOverEnemy;
			currentEnergyPoints = maxEnergyPoints;
		}

		void Update ()
		{

		}

		void OnMouseOverEnemy ( Enemy enemy )
		{
			if ( Input.GetMouseButtonDown ( 1 ) )
			{
				float newEnergyPoints = currentEnergyPoints - pointsPerHit;
				currentEnergyPoints = Mathf.Clamp ( newEnergyPoints, 0, maxEnergyPoints );

				float xValue = -((currentEnergyPoints / maxEnergyPoints) / 2f) - 0.5f;
				energyBar.uvRect = new Rect ( xValue, 0f, 0.5f, 1f );
			}			
		}
	}
}