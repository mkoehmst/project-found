using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class RaycastMaster
{
	public List<int> Priority { get; private set; }

	public RaycastHit? PriorityHitCheck { get; set; }

	public EventSystem EventSystem { get; private set; }

	public RaycastMaster( )
	{
		Priority = new List<int>( );

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
		foreach ( int layer in Priority )
		{
			foreach ( RaycastHit hit in hits )
			{
				if ( hit.collider.gameObject.layer == layer )
				{
					PriorityHitCheck = hit;
					return ;
				}
			}
		}

		PriorityHitCheck = null;
	}
}
