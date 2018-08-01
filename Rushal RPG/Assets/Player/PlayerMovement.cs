using System;
using UnityEngine;
using UnityStandardAssets.Characters.ThirdPerson;

[RequireComponent(typeof (ThirdPersonCharacter))]
public class PlayerMovement : MonoBehaviour
{
	[SerializeField] float walkMoveStopRadius = 0.2f;

    ThirdPersonCharacter thirdPersonCharacter;   // A reference to the ThirdPersonCharacter on the object
    CameraRaycaster cameraRaycaster;
    Vector3 currentClickTarget;

	bool isInDirectMode = false; // TODO consider making static

	private void Start()
    {
        cameraRaycaster = Camera.main.GetComponent<CameraRaycaster>();
        thirdPersonCharacter = GetComponent<ThirdPersonCharacter>();
        currentClickTarget = transform.position;
    }

    private void FixedUpdate()
	{
		if(Input.GetKeyDown(KeyCode.G)) // TODO allow player to remap or add to menus
		{
			isInDirectMode = !isInDirectMode;
			currentClickTarget = transform.position;
		}

		if (isInDirectMode)
		{
			ProcessDirectMovement();
		}
		else
		{
			ProcessMouseMovement();
		}
	}

	private void ProcessDirectMovement()
	{
		float h = Input.GetAxis( "Horizontal" );
		float v = Input.GetAxis( "Vertical" );

		Vector3 cameraForward = Vector3.Scale( Camera.main.transform.forward, new Vector3( 1, 0, 1 ) ).normalized;
		Vector3 movement = v * cameraForward + h * Camera.main.transform.right;

		thirdPersonCharacter.Move( movement, false, false );
	}

	private void ProcessMouseMovement()
	{
		if ( Input.GetMouseButton( 0 ) )
		{
			switch ( cameraRaycaster.currentLayerHit )
			{
				case Layer.Walkable:
					currentClickTarget = cameraRaycaster.hit.point;
					break;
				case Layer.Enemy:
					print( "not moving to enemy" );
					break;
				default:
					print( "Shouldn't be here" );
					break;
			}
		}

		var playerToClickPoint = currentClickTarget - transform.position;
		if ( playerToClickPoint.magnitude >= walkMoveStopRadius )
		{
			thirdPersonCharacter.Move( playerToClickPoint, false, false );
		}
		else
		{
			thirdPersonCharacter.Move( Vector3.zero, false, false );
		}
	}
}