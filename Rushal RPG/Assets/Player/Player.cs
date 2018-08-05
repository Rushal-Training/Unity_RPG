using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour, IDamagable
{
	[SerializeField] int enemyLayer = 10;
	[SerializeField] float maxHealthPoints = 100f;
	[SerializeField] float meleeDamagePerClick = 10f;
	[SerializeField] float minTimeBetweeHits = 0.5f;
	[SerializeField] float maxAttackRange = 2f;

	CameraRaycaster cameraRaycaster;
	GameObject currentTarget;
	float currentHealthPoints;
	float lastHitTime = 0f;

	void Start () {
		cameraRaycaster = FindObjectOfType<CameraRaycaster>();
		cameraRaycaster.notifyMouseClickObservers += OnMouseClick;

		currentHealthPoints = maxHealthPoints;
	}
	
	void Update () {
		
	}

	public float healthAsPercentage
	{
		get
		{
			return currentHealthPoints / maxHealthPoints;
		}
	}

	void OnMouseClick ( RaycastHit raycastHit, int layerHit )
	{
		if ( layerHit == enemyLayer )
		{
			var enemy = raycastHit.collider.gameObject;
			
			if ( (enemy.transform.position - transform.position).magnitude > maxAttackRange ) { return; }

			currentTarget = enemy;

			Vector3 faceTheEnemy = ( enemy.transform.position - transform.position );
			transform.rotation = Quaternion.Slerp( transform.rotation, Quaternion.LookRotation( faceTheEnemy ), 0.2f );

			IDamagable damagableComponent = enemy.GetComponent<IDamagable>();
			if ( damagableComponent != null && Time.time - lastHitTime > minTimeBetweeHits )
			{
				damagableComponent.TakeDamage( meleeDamagePerClick );
				lastHitTime = Time.time;
			}

		}
	}

	public void TakeDamage( float damage )
	{
		currentHealthPoints = Mathf.Clamp( currentHealthPoints - damage, 0f, maxHealthPoints );
		//if ( currentHealthPoints <= 0 ) { Destroy( gameObject );  }
	}
}
