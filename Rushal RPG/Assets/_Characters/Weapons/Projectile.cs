using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// TODO consider rewiring
using RPG.Core;

namespace RPG.Characters
{
	public class Projectile : MonoBehaviour
	{
		[SerializeField] float projectileSpeed;
		int parentLayer;
		float damageCaused;

		public float GetDefaultLaunchSpeed ()
		{
			return projectileSpeed;
		}

		public void SetParentLayer ( GameObject parent )
		{
			parentLayer = parent.layer;
		}

		public void SetDamage ( float damage )
		{
			damageCaused = damage;
		}

		private void OnCollisionEnter ( Collision collision )
		{
			IDamagable damagableComponent = collision.gameObject.GetComponent<IDamagable> ();
			var collisionLayer = collision.gameObject.layer;
			if ( damagableComponent != null && parentLayer != collisionLayer )
			{
				damagableComponent.TakeDamage ( damageCaused );
			}

			Destroy ( gameObject );
		}
	}
}