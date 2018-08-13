using System;
using UnityEngine;
using UnityEngine.AI;
using RPG.CameraUI; // TODO consider rewiring

namespace RPG.Characters
{
	[RequireComponent ( typeof ( NavMeshAgent ) )]
	public class CharacterMovement : MonoBehaviour
	{
		[SerializeField] float stoppingDistance = 1.4f;
		[SerializeField] float animationSpeedMultiplier = 1.5f;
		[SerializeField] float moveSpeedMultiplier = 1.2f;
		[SerializeField] float movingTurnSpeed = 1800;
		[SerializeField] float stationaryTurnSpeed = 900;

		Animator animator;
		NavMeshAgent agent;
		Rigidbody myRigidbody;
		Vector3 clickPoint;

		float forwardAmount;
		float turnAmount;
		float moveThreshold = 1f;
		bool isInDirectMode = false; // TODO consider making static

		public void Move ( Vector3 movement )
		{
			SetForwardAndTurn ( movement );
			ApplyExtraTurnRotation ();
			UpdateAnimator ();
		}

		public void Kill ()
		{
			// todo allow death signal
		}

		void Start ()
		{
			animator = GetComponent<Animator> ();

			CameraRaycaster cameraRaycaster = Camera.main.GetComponent<CameraRaycaster> ();
			cameraRaycaster.OnMouseOverPotentiallyWalkable += OnMouseOverPotentiallyWalkable;
			cameraRaycaster.OnMouseOverEnemyObservers += OnMouseOverEnemy;

			agent = GetComponent<NavMeshAgent> ();
			agent.updateRotation = false;
			agent.updatePosition = true;
			agent.stoppingDistance = stoppingDistance;

			myRigidbody = GetComponent<Rigidbody> ();
			myRigidbody.constraints = RigidbodyConstraints.FreezeRotation;
		}

		void Update ()
		{
			if ( agent.remainingDistance > agent.stoppingDistance )
			{
				Move ( agent.desiredVelocity );
			}
			else
			{
				Move ( Vector3.zero );
			}
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

		void OnMouseOverPotentiallyWalkable ( Vector3 destination )
		{
			if ( Input.GetMouseButton ( 0 ) )
			{
				agent.SetDestination ( destination );
			}
		}

		void OnMouseOverEnemy ( Enemy enemy )
		{
			if ( Input.GetMouseButton( 0 ) || Input.GetMouseButtonDown( 1 ) )
			{
				agent.SetDestination ( enemy.transform.position );
			}
		}

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