using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPG.Characters
{
	[ExecuteInEditMode]
	public class WeaponPickupPoint : MonoBehaviour
	{
		[SerializeField] Weapon weaponConfig;
		[SerializeField] AudioClip weaponPickupSfx;

		void Start ()
		{

		}

		void Update ()
		{
			if ( !Application.isPlaying )
			{
				DestroyChildren ();
				InstantiateWeapon ();
			}
		}

		private void DestroyChildren ()
		{
			foreach ( Transform child in transform )
			{
				DestroyImmediate ( child.gameObject );
			}
		}

		private void InstantiateWeapon ()
		{
			var weapon = weaponConfig.GetWeaponPrefab ();
			weapon.transform.position = Vector3.zero;
			Instantiate ( weapon, gameObject.transform );
		}

		private void OnTriggerEnter ( Collider collider )
		{
			FindObjectOfType<PlayerControl> ().PutWeaponInHand ( weaponConfig );
			AudioSource.PlayClipAtPoint ( weaponPickupSfx, transform.position );
		}
	}
}
