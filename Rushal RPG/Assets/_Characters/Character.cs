using System;
using UnityEngine;
using UnityEngine.AI;
using RPG.CameraUI; // TODO consider rewiring

namespace RPG.Characters
{
	[SelectionBase]
	public class Character : MonoBehaviour
	{
		[Header("Animator")]
		[SerializeField] RuntimeAnimatorController runtimeAnimatorController;
		[SerializeField] AnimatorOverrideController animatorOverrideController;
		[SerializeField] Avatar avatar;

		[Header("Audio")]
		[SerializeField] float audioSourceSpatialBlend = 0.5f;

		[Header ( "Capsule Collider" )]
		[SerializeField] Vector3 colliderCenter = new Vector3 ( 0, 1f, 0 );
		[SerializeField] float colliderRadius = 0.2f;
		[SerializeField] float colliderHeight = 1.85f;

		[Header ( "Movement" )]
		[SerializeField] float animationSpeedMultiplier = 1.2f;
		[SerializeField] float moveSpeedMultiplier = 1f;
		[SerializeField] float movingTurnSpeed = 1800f;
		[SerializeField] float stationaryTurnSpeed = 900f;

		[Header ( "NavMesh Agent" )]
		[SerializeField] float navMeshAgentSteeringSpeed = 1.0f;
		[SerializeField] float stoppingDistance = 1.4f;

		Animator animator;
		NavMeshAgent navMeshAgent;
		Rigidbody myRigidbody;

		float forwardAmount;
		float turnAmount;
		float moveThreshold = 1f;

		bool isAlive = true;
		bool isInDirectMode = false; // TODO consider making static

		public void SetDestination ( Vector3 worldPos )
		{
			navMeshAgent.destination = worldPos;
		}

		public void Kill ()
		{
			isAlive = false;
		}

		void Awake ()
		{
			AddRequiredComponents ();
		}

		void AddRequiredComponents ()
		{
			var capsuleCollider = gameObject.AddComponent<CapsuleCollider> ();
			capsuleCollider.center = colliderCenter;
			capsuleCollider.radius = colliderRadius;
			capsuleCollider.height = colliderHeight;

			myRigidbody = gameObject.AddComponent<Rigidbody> ();
			myRigidbody.constraints = RigidbodyConstraints.FreezeRotation;

			var audioSource = gameObject.AddComponent<AudioSource> ();
			audioSource.spatialBlend = audioSourceSpatialBlend;

			animator = gameObject.AddComponent<Animator> ();
			animator.runtimeAnimatorController = runtimeAnimatorController;
			animator.avatar = avatar;

			navMeshAgent = gameObject.AddComponent<NavMeshAgent> ();
			navMeshAgent.speed = navMeshAgentSteeringSpeed;
			navMeshAgent.stoppingDistance = stoppingDistance;
			navMeshAgent.autoBraking = false;
			navMeshAgent.updateRotation = false;
			navMeshAgent.updatePosition = true;
		}

		void Update ()
		{
			if ( navMeshAgent.remainingDistance > navMeshAgent.stoppingDistance && isAlive )
			{
				Move ( navMeshAgent.desiredVelocity );
			}
			else
			{
				Move ( Vector3.zero );
			}
		}

		void Move ( Vector3 movement )
		{
			SetForwardAndTurn ( movement );
			ApplyExtraTurnRotation ();
			UpdateAnimator ();
		}

		void SetForwardAndTurn ( Vector3 movement )
		{
			if ( movement.magnitude > moveThreshold )
			{
				movement.Normalize ();
			}
			var localMovement = transform.InverseTransformDirection ( movement );
			turnAmount = Mathf.Atan2 ( localMovement.x, localMovement.z );
			forwardAmount = localMovement.z;
		}

		void UpdateAnimator ()
		{
			animator.SetFloat ( "Forward", forwardAmount, 0.1f, Time.deltaTime );
			animator.SetFloat ( "Turn", turnAmount, 0.1f, Time.deltaTime );
			animator.speed = animationSpeedMultiplier;
		}

		void ApplyExtraTurnRotation ()
		{
			float turnSpeed = Mathf.Lerp ( stationaryTurnSpeed, movingTurnSpeed, forwardAmount );
			transform.Rotate ( 0, turnAmount * turnSpeed * Time.deltaTime, 0 );
		}

		/*void OnMouseOverPotentiallyWalkable ( Vector3 destination )
		{
			if ( Input.GetMouseButton ( 0 ) )
			{
				navMeshAgent.SetDestination ( destination );
			}
		}

		void OnMouseOverEnemy ( Enemy enemy )
		{
			if ( Input.GetMouseButton( 0 ) || Input.GetMouseButtonDown( 1 ) )
			{
				navMeshAgent.SetDestination ( enemy.transform.position );
			}
		}*/

		// TODO make this get called again
		void ProcessDirectMovement ()
		{
			float h = Input.GetAxis ( "Horizontal" );
			float v = Input.GetAxis ( "Vertical" );

			Vector3 cameraForward = Vector3.Scale ( Camera.main.transform.forward, new Vector3 ( 1, 0, 1 ) ).normalized;
			Vector3 movement = v * cameraForward + h * Camera.main.transform.right;

			Move ( movement );
		}

		void OnAnimatorMove ()
		{
			if ( Time.deltaTime > 0 )
			{
				Vector3 velocity = (animator.deltaPosition * moveSpeedMultiplier) / Time.deltaTime;

				// we preserve the existing y part of the current velocity.
				velocity.y = myRigidbody.velocity.y;
				myRigidbody.velocity = velocity;
			}
		}
	}
}