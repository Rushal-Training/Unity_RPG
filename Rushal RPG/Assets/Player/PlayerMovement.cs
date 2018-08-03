using System;
using UnityEngine;
using UnityStandardAssets.Characters.ThirdPerson;

[RequireComponent(typeof (ThirdPersonCharacter))]
public class PlayerMovement : MonoBehaviour
{
	[SerializeField] float walkMoveStopRadius = 0.2f;
	[SerializeField] float attackMoveStopRadius = 5f;

    ThirdPersonCharacter thirdPersonCharacter;   // A reference to the ThirdPersonCharacter on the object
    CameraRaycaster cameraRaycaster;
    Vector3 currentDestination, clickPoint;

	bool isInDirectMode = false; // TODO consider making static

	private void Start()
    {
		cameraRaycaster = Camera.main.GetComponent<CameraRaycaster>();


		thirdPersonCharacter = GetComponent<ThirdPersonCharacter>();
        currentDestination = transform.position;
    }

    private void FixedUpdate()
	{
		if(Input.GetKeyDown(KeyCode.G)) // TODO allow player to remap or add to menus
		{
			isInDirectMode = !isInDirectMode;
			currentDestination = transform.position;
		}

		if (isInDirectMode)
		{
			ProcessDirectMovement();
		}
		else
		{
			//ProcessMouseMovement();
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

	/*private void ProcessMouseMovement( RaycastHit raycastHit, int layerHit )
	{
		if ( Input.GetMouseButton ( 0 ) )
		{
			clickPoint = cameraRaycaster.hit.point;
			switch ( cameraRaycaster.currentLayerHit )
			{
				case Layer.Walkable:
					currentDestination = ShortDistance ( clickPoint, walkMoveStopRadius );
					break;
				case Layer.Enemy:
					currentDestination = ShortDistance ( clickPoint, attackMoveStopRadius );
					break;
				default:
					print ( "Shouldn't be here" );
					break;
			}
		}

		WalkToDestination ();
	}*/

	private void WalkToDestination ()
	{
		var playerToClickPoint = currentDestination - transform.position;
		if ( playerToClickPoint.magnitude >= 0 )
		{
			thirdPersonCharacter.Move ( playerToClickPoint, false, false );
		}
		else
		{
			thirdPersonCharacter.Move ( Vector3.zero, false, false );
		}
	}

	Vector3 ShortDistance ( Vector3 destination, float shortening )
	{
		Vector3 reductionVector = (destination - transform.position).normalized * shortening;
		return destination - reductionVector;
	}

	private void OnDrawGizmos ()
	{
		// draw movement
		Gizmos.color = Color.black;
		Gizmos.DrawLine ( transform.position, currentDestination );
		Gizmos.DrawSphere ( currentDestination, 0.1f );
		Gizmos.DrawSphere ( clickPoint, 0.15f );

		// draw attack
		Gizmos.color = new Color ( 255f, 0f, 0f, .5f );
		Gizmos.DrawWireSphere ( transform.position, attackMoveStopRadius );
	}
}