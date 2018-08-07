using System;
using UnityEngine;
using UnityEngine.AI;
using UnityStandardAssets.Characters.ThirdPerson;
using RPG.CameraUI; // TODO consider rewiring

namespace RPG.Characters
{
	[RequireComponent ( typeof ( NavMeshAgent ) )]
	[RequireComponent ( typeof ( AICharacterControl ) )]
	[RequireComponent ( typeof ( ThirdPersonCharacter ) )]
	public class PlayerMovement : MonoBehaviour
	{
		[SerializeField] const int walkableLayerNumber = 9;
		[SerializeField] const int enemyLayerNumber = 10;

		ThirdPersonCharacter thirdPersonCharacter;   // A reference to the ThirdPersonCharacter on the object
		AICharacterControl aiCharacterControl = null;
		CameraRaycaster cameraRaycaster;
		Vector3 clickPoint;
		GameObject walkTarget;

		bool isInDirectMode = false; // TODO consider making static

		private void Start ()
		{
			cameraRaycaster = Camera.main.GetComponent<CameraRaycaster> ();
			cameraRaycaster.notifyMouseClickObservers += ProcessMouseClick;

			aiCharacterControl = GetComponent<AICharacterControl> ();
			thirdPersonCharacter = GetComponent<ThirdPersonCharacter> ();
			walkTarget = new GameObject ( "WalkTarget" );
		}

		void ProcessMouseClick ( RaycastHit raycastHit, int layerHit )
		{
			switch ( layerHit )
			{
				case enemyLayerNumber:
					GameObject enemy = raycastHit.collider.gameObject;
					aiCharacterControl.SetTarget ( enemy.transform );
					break;
				case walkableLayerNumber:
					walkTarget.transform.position = raycastHit.point;
					aiCharacterControl.SetTarget ( walkTarget.transform );
					break;
				default:
					Debug.LogWarning ( "Don't know how to handle mouse click for player" );
					return;
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