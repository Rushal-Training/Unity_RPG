using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
	public float projectileSpeed;

	float damageCaused;

	public void SetDamage ( float damage )
	{
		damageCaused = damage;
	}

	private void OnCollisionEnter( Collision collision )
	{
		IDamagable damagableComponent = collision.gameObject.GetComponent<IDamagable>();
		if ( damagableComponent != null )
		{
			damagableComponent.TakeDamage( damageCaused );
		}

		Destroy( gameObject );
	}		
}