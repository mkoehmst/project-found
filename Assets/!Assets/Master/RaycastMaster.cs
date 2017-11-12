using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using UnityEngine;
using UnityEngine.EventSystems;

using mattmc3.dotmore.Collections.Generic;

namespace ProjectFound.Master {


	public class RaycastMaster
	{
		public class LayerDetails
		{
			public delegate void CursorFocus( GameObject obj );
			public CursorFocus DelegateCursorFocusGained;
			public CursorFocus DelegateCursorFocusLost;
		}

		private Ray m_ray;

		public OrderedDictionary<Core.LayerID,LayerDetails> Priority { get; private set; }
		public RaycastHit? PriorityHitCheck { get; private set; }
		public RaycastHit? PreviousPriorityHitCheck { get; private set; }
		public EventSystem EventSystem { get; private set; }
		public Rect ScreenRect { get; private set; }
		public Vector3 ScreenCenter { get; private set; }
		public InputMaster.InputDevice CursorDevice { get; set; }

		public RaycastMaster( )
		{
			Priority = new OrderedDictionary<Core.LayerID,LayerDetails>( );
			EventSystem = GameObject.FindObjectOfType<EventSystem>( );
			ScreenRect = new Rect( 0, 0, Screen.width, Screen.height );
			ScreenCenter = new Vector3( Screen.width / 2, Screen.height / 2, 0f );
		}

		public void Loop( )
		{
			if ( CursorDevice == InputMaster.InputDevice.MouseAndKeyboard )
			{
				if ( IsOverUIElement( ) )
					return ;

				if ( ScreenRect.Contains( Input.mousePosition ) == false )
					return ;

				// Raycast to max depth, every frame as things can move under mouse
				m_ray = Camera.main.ScreenPointToRay( Input.mousePosition );
			}
			else if ( CursorDevice == InputMaster.InputDevice.Gamepad )
			{
				m_ray = Camera.main.ScreenPointToRay( ScreenCenter );
			}

			RaycastHit[] raycastHits = Physics.RaycastAll( m_ray, 100f );
			FindTopPriorityCursorHit( raycastHits );
		}

		public void AddPriority( Core.LayerID layer )
		{
			LayerDetails layerDetails = new LayerDetails( );

			Priority.Add( layer, layerDetails );
		}

		public void SetLayerDelegates(
			Core.LayerID layer, LayerDetails.CursorFocus gained, LayerDetails.CursorFocus lost )
		{
			LayerDetails layerDetails = Priority.GetValue( layer );

			if ( layerDetails != null )
			{
				layerDetails.DelegateCursorFocusGained = gained;
				layerDetails.DelegateCursorFocusLost = lost;
			}
		}

		public bool DidFindPriorityHit( )
		{
			return PriorityHitCheck.HasValue;
		}

		public bool IsOverUIElement( )
		{
			return EventSystem.current.IsPointerOverGameObject( );
		}

		private void FindTopPriorityCursorHit( RaycastHit[] hits )
		{
			if ( hits.Length == 0 || Priority.Count == 0 )
			{
				PriorityHitCheck = null;
				return ;
			}

			// TODO More elegant way to check top priority hit
			foreach ( Core.LayerID layer in Priority.Keys )
			{
				var matchedHits = new List<RaycastHit>( );

				foreach ( RaycastHit hit in hits )
				{
					GameObject hitObj = hit.collider.gameObject;

					if ( hitObj.layer == (int)layer )
					{
						matchedHits.Add( hit );
					}
				}

				int matchCount = matchedHits.Count;

				if ( matchCount > 0 )
				{
					Vector3 cameraPos = Camera.main.transform.position;
					float closestDistance = 100000000.0f;
					int closestIndex = default( int );

					for ( int i = 0; i < matchCount; ++i )
					{
						float distance = (cameraPos - matchedHits[i].transform.position).magnitude;

						if ( distance < closestDistance )
						{
							closestDistance = distance;
							closestIndex = i;
						}
					}

					UpdateHitCheck( matchedHits[closestIndex] );

					return ;
				}
			}

			UpdateHitCheck( null );
		}

		private void UpdateHitCheck( RaycastHit? hit )
		{
			PreviousPriorityHitCheck = PriorityHitCheck;
			PriorityHitCheck = hit;

			CheckForFocusChange( );
		}

		private void CheckForFocusChange( )
		{
			if ( PreviousPriorityHitCheck.HasValue == true && PriorityHitCheck.HasValue == true )
			{
				GameObject prevObj = PreviousPriorityHitCheck.Value.collider.gameObject;
				GameObject curObj = PriorityHitCheck.Value.collider.gameObject;

				if ( prevObj != curObj )
				{
					ObjectFocusLost( prevObj );
					ObjectFocusGained( curObj );
				}
			}
			else if (
				PreviousPriorityHitCheck.HasValue == false && PriorityHitCheck.HasValue == true )
			{
				ObjectFocusGained( PriorityHitCheck.Value.collider.gameObject );
			}
			else if (
				PreviousPriorityHitCheck.HasValue == true && PriorityHitCheck.HasValue == false )
			{
				ObjectFocusLost( PreviousPriorityHitCheck.Value.collider.gameObject );
			}
		}

		private void ObjectFocusGained( GameObject obj )
		{
			LayerDetails layerDetails = Priority.GetValue( (Core.LayerID)obj.layer );

			if ( layerDetails.DelegateCursorFocusGained != null )
				layerDetails.DelegateCursorFocusGained( obj );
		}

		private void ObjectFocusLost( GameObject obj )
		{
			LayerDetails layerDetails = Priority.GetValue( (Core.LayerID)obj.layer );

			if ( layerDetails.DelegateCursorFocusLost != null )
				layerDetails.DelegateCursorFocusLost( obj );
		}
	}


}