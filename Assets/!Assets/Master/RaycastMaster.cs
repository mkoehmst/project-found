using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using UnityEngine;
using UnityEngine.EventSystems;

using mattmc3.dotmore.Collections.Generic;

namespace ProjectFound.Master {

	public class RaycastMaster
	{
		public class RaycastLayer
		{
			public int Layer { get; private set; }

			public delegate void CursorFocus( GameObject obj );
			public CursorFocus DelegateCursorFocusGained;
			public CursorFocus DelegateCursorFocusLost;

			public RaycastLayer( int layer )
			{
				Layer = layer;
			}
		}

		public OrderedDictionary<int,RaycastLayer> Priority { get; private set; }

		public RaycastHit? PriorityHitCheck { get; set; }

		public EventSystem EventSystem { get; private set; }

		public RaycastMaster( )
		{
			Priority = new OrderedDictionary<int,RaycastLayer>( );

			EventSystem = GameObject.FindObjectOfType<EventSystem>( );
		}

		public void Loop( )
		{
			if ( IsOverUIElement( ) )
				return ;

			// Raycast to max depth, every frame as things can move under mouse
			Ray ray = Camera.main.ScreenPointToRay( Input.mousePosition );
			RaycastHit[] raycastHits = Physics.RaycastAll( ray, 100f );

			if ( raycastHits.Length != 0 )
			{
				FindTopPriorityCursorHit( raycastHits );
			}
			else
			{
				PriorityHitCheck = null;
			}
		}

		public void AddPriority( int layer )
		{
			RaycastLayer raycastLayer = new RaycastLayer( layer );

			Priority.Add( layer, raycastLayer );
		}

		public void SetLayerDelegates(
			int layer, RaycastLayer.CursorFocus gained, RaycastLayer.CursorFocus lost )
		{
			RaycastLayer raycastLayer = Priority.GetValue( layer );

			if ( raycastLayer != null )
			{
				raycastLayer.DelegateCursorFocusGained = gained;
				raycastLayer.DelegateCursorFocusLost = lost;
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
			foreach ( int layer in Priority.Keys )
			{
				foreach ( RaycastHit hit in hits )
				{
					GameObject hitObj = hit.collider.gameObject;

					if ( hitObj.layer == layer )
					{
						if ( PriorityHitCheck.HasValue == false ||
							PriorityHitCheck.Value.collider != hit.collider )
						{
							RaycastLayer raycastLayer = Priority.GetValue( layer );

							if ( raycastLayer != null &&
								raycastLayer.DelegateCursorFocusGained != null )
							{
								raycastLayer.DelegateCursorFocusGained( hitObj );
							}
						}

						if ( PriorityHitCheck.HasValue == true &&
							PriorityHitCheck.Value.collider.gameObject != hit.collider.gameObject )
						{
							GameObject prevObj = PriorityHitCheck.Value.collider.gameObject;

							RaycastLayer prevRaycastLayer = Priority.GetValue( prevObj.layer );

							if ( prevRaycastLayer != null &&
								prevRaycastLayer.DelegateCursorFocusLost != null )
							{
								prevRaycastLayer.DelegateCursorFocusLost( prevObj );
							}
						}

						PriorityHitCheck = hit;

						return ;
					}
				}
			}

			PriorityHitCheck = null;
		}
	}

}