using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace ProjectFound.CameraUI {


	[RequireComponent(typeof(GridLayoutGroup))]
	public class WidgetItemGridUI : PanelWidgetUI
	{
		[SerializeField] int m_maxSlots;
		[SerializeField] GameObject m_slotPrefab;

		GridLayoutGroup m_grid;
		List<GameObject> m_slots;

		public GameObject FirstEmptySlot
		{
			get
			{
				foreach ( GameObject slot in m_slots )
				{
					if ( slot.transform.childCount == 0 )
						return slot;
				}

				Debug.Log( "Inventory full!" );
				return null;
			}
		}

		new void Start( )
		{
			base.Start( );

			m_grid = GetComponent<GridLayoutGroup>( );
			m_slots = new List<GameObject>( );

			for ( int i = 0; i < m_maxSlots; ++i )
			{
				m_slots.Add( Instantiate( m_slotPrefab, this.transform ) );
			}
		}

		void LateUpdate( )
		{
			float parentX = m_parentPanelRect.sizeDelta.x;
			float parentY = m_parentPanelRect.sizeDelta.y;

			float cellX = m_grid.cellSize.x + m_grid.spacing.x;
			float cellY = m_grid.cellSize.y + m_grid.spacing.y;

			int numColumns = Mathf.FloorToInt( parentX / cellX );

			m_grid.constraintCount = numColumns;
		}
	}


}
