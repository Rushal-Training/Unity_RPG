using System;
using UnityEngine;
using UnityEngine.AI;
using RPG.CameraUI; // TODO consider rewiring

namespace RPG.Characters
{
	[RequireComponent ( typeof ( NavMeshAgent ) )]
	[RequireComponent ( typeof ( AICharacterControl ) )]
	[RequireComponent ( typeof ( ThirdPersonCharacter ) )]
	public class PlayerMovement : MonoBehaviour
	{
		ThirdPersonCharacter thirdPersonCharacter;   // A reference to the ThirdPersonCharacter on the object
		AICharacterControl aiCharacterControl = null;
		CameraRaycaster cameraRaycaster;
		Vector3 clickPoint;
		GameObject walkTarget;
		Player player;

		bool isInDirectMode = false; // TODO consider making static

		private void Start ()
		{
			cameraRaycaster = Camera.main.GetComponent<CameraRaycaster> ();
			cameraRaycaster.OnMouseOverPotentiallyWalkable += OnMouseOverPotentiallyWalkable;
			cameraRaycaster.OnMouseOverEnemyObservers += OnMouseOverEnemy;

			aiCharacterControl = GetComponent<AICharacterControl> ();
			thirdPersonCharacter = GetComponent<ThirdPersonCharacter> ();
			walkTarget = new GameObject ( "WalkTarget" );
			player = GetComponent<Player>();
		}

		void OnMouseOverPotentiallyWalkable ( Vector3 destination )
		{
			if ( Input.GetMouseButton ( 0 ) && !player.GetIsDead())
			{
				walkTarget.transform.position = destination;
				aiCharacterControl.SetTarget ( walkTarget.transform );
			}
		}

		void OnMouseOverEnemy ( Enemy enemy )
		{
			if ( !player.GetIsDead() )
			{
				if ( Input.GetMouseButton( 0 ) || Input.GetMouseButtonDown( 1 ) )
				{
					aiCharacterControl.SetTarget( enemy.transform );
				}
			}
		}

		// TODO make this get called again
		private void ProcessDirectMovement ()
		{
			float h = Input.GetAxis ( "Horizontal" );
			float v = Input.GetAxis ( "Vertical" );

			Vector3 cameraForward = Vector3.Scale ( Camera.main.transform.forward, new Vector3 ( 1, 0, 1 ) ).normalized;
			Vector3 movement = v * cameraForward + h * Camera.main.transform.right;

			thirdPersonCharacter.Move ( movement, false, false );
		}
	}
}