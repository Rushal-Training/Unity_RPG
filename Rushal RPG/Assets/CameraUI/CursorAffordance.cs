using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPG.CameraUI
{
	[RequireComponent ( typeof ( CameraRaycaster ) )]
	public class CursorAffordance : MonoBehaviour
	{
		[SerializeField] const int walkableLayerNumber = 9;
		[SerializeField] const int enemyLayerNumber = 10;

		[SerializeField] Texture2D walkCursor = null;
		[SerializeField] Texture2D targetCursor = null;
		[SerializeField] Texture2D unknownCursor = null;

		[SerializeField] Vector2 cursorHotspot = new Vector2 ( 0, 0 );

		CameraRaycaster cameraRaycaster;

		void Start () // TODO Consider deregistering OnLayerChanged when leaving all game scenes
		{
			cameraRaycaster = GetComponent<CameraRaycaster> ();
			cameraRaycaster.notifyLayerChangeObservers += OnLayerChange;
		}

		void OnLayerChange ( int newLayer )
		{
			switch ( newLayer )
			{
				case walkableLayerNumber:
					Cursor.SetCursor ( walkCursor, cursorHotspot, CursorMode.Auto );
					break;
				case enemyLayerNumber:
					Cursor.SetCursor ( targetCursor, cursorHotspot, CursorMode.Auto );
					break;
				default:
					Cursor.SetCursor ( unknownCursor, cursorHotspot, CursorMode.Auto );
					break;
			}
		}
	}
}