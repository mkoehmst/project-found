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

		public OrderedDictionary<Core.LayerID,LayerDetails> Priority { get; private set; }
		public RaycastHit? PriorityHitCheck { get; set; }
		public EventSystem EventSystem { get; private set; }

		public RaycastMaster( )
		{
			Priority = new OrderedDictionary<Core.LayerID,LayerDetails>( );
			EventSystem = GameObject.FindObjectOfType<EventSystem>( );
		}

		public void Loop( )
		{
			if ( IsOverUIElement( ) )
				return ;

			// Raycast to max depth, every frame as things can move under mouse
			Ray ray = Camera.main.ScreenPointToRay( Input.mousePosition );
			RaycastHit[] raycastHits = Physics.RaycastAll( ray, 100f );

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
				foreach ( RaycastHit hit in hits )
				{
					GameObject hitObj = hit.collider.gameObject;

					if ( hitObj.layer == (int)layer )
					{
						if ( PriorityHitCheck.HasValue == false ||
							PriorityHitCheck.Value.collider != hit.collider )
						{
							LayerDetails layerDetails = Priority.GetValue( layer );

							if ( layerDetails != null &&
								layerDetails.DelegateCursorFocusGained != null )
							{
								layerDetails.DelegateCursorFocusGained( hitObj );
							}
						}

						if ( PriorityHitCheck.HasValue == true &&
							PriorityHitCheck.Value.collider.gameObject != hit.collider.gameObject )
						{
							GameObject prevObj = PriorityHitCheck.Value.collider.gameObject;

							LayerDetails prevLayerDetails =
								Priority.GetValue( (Core.LayerID)prevObj.layer );

							if ( prevLayerDetails != null &&
								prevLayerDetails.DelegateCursorFocusLost != null )
							{
								prevLayerDetails.DelegateCursorFocusLost( prevObj );
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