using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using RPG.Characters;

namespace RPG.CameraUI
{
	public class CameraRaycaster : MonoBehaviour
	{
		[SerializeField] Texture2D walkCursor = null;
		[SerializeField] Texture2D targetCursor = null;
		[SerializeField] Texture2D unknownCursor = null;
		[SerializeField] Vector2 cursorHotspot = new Vector2 ( 0, 0 );

		const int POTENTIALLY_WALKABLE_LAYER = 9;
		float maxRaycastDepth = 100f;

		public delegate void OnMouseOverEnemy ( Enemy enemy );
		public event OnMouseOverEnemy OnMouseOverEnemyObservers;

		public delegate void OnMouseOverTerrain ( Vector3 destination );
		public event OnMouseOverTerrain OnMouseOverPotentiallyWalkable;

		void Update ()
		{
			// Check if pointer is over an interactable UI element
			if ( EventSystem.current.IsPointerOverGameObject () )
			{
				// Implement ui interaction
			}
			else
			{
				PerformRaycasts ();
			}
		}

		void PerformRaycasts ()
		{
			Ray ray = Camera.main.ScreenPointToRay ( Input.mousePosition );
			if ( RaycastForUnknown ( ray ) ) { return; }
			if ( RaycastForEnemy ( ray ) ) { return; }
			if ( RaycastForPotentiallyWalkable ( ray ) ) { return; }

		}

		private bool RaycastForUnknown ( Ray ray )
		{
			RaycastHit hitInfo;
			bool unreachableRaycast = Physics.Raycast ( ray, out hitInfo, maxRaycastDepth );

			if ( !unreachableRaycast )
			{
				Cursor.SetCursor ( unknownCursor, cursorHotspot, CursorMode.Auto );
				return true;
			}
			return false;
		}

		private bool RaycastForEnemy ( Ray ray )
		{
			RaycastHit hitInfo;
			Physics.Raycast ( ray, out hitInfo, maxRaycastDepth );
			var gameObjectHit = hitInfo.collider.gameObject;
			var enemyHit = gameObjectHit.GetComponent<Enemy> ();

			if ( enemyHit )
			{
				Cursor.SetCursor ( targetCursor, cursorHotspot, CursorMode.Auto );
				OnMouseOverEnemyObservers ( enemyHit );
				return true;
			}
			return false;
		}

		private bool RaycastForPotentiallyWalkable (Ray ray)
		{
			RaycastHit hitInfo;
			LayerMask potentiallyWalkableLayer = 1 << POTENTIALLY_WALKABLE_LAYER;
			bool potentiallyWalkableHit = Physics.Raycast ( ray, out hitInfo, maxRaycastDepth, potentiallyWalkableLayer );

			if ( potentiallyWalkableHit )
			{
				Cursor.SetCursor ( walkCursor, cursorHotspot, CursorMode.Auto );
				OnMouseOverPotentiallyWalkable ( hitInfo.point );
				return true;
			}
			return false;
		}
	}
}