using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPG.Characters
{
	public class WaypointContainer : MonoBehaviour
	{
		void OnDrawGizmos()
		{
			Vector3 firstPos = transform.GetChild( 0 ).position;
			Vector3 previousPos = firstPos;

			foreach ( Transform child in transform )
			{
				Gizmos.color = new Color( 255f, 0f, 0f, .5f );
				Gizmos.DrawSphere( child.position, .2f );

				Gizmos.color = new Color( 255f, 255f, 255f, .5f );
				Gizmos.DrawLine( previousPos, child.position );

				previousPos = child.position;
			}

			Gizmos.DrawLine( previousPos, firstPos );
		}
	}
}