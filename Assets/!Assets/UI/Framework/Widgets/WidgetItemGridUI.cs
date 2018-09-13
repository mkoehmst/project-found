namespace ProjectFound.CameraUI 
{


	using System.Collections.Generic;

	using UnityEngine;
	using UnityEngine.UI;
	using Autelia.Serialization;

	[RequireComponent(typeof(GridLayoutGroup))]
	public class WidgetItemGridUI : PanelWidgetUI
	{
		[SerializeField] int _maxSlots;
		[SerializeField] GameObject _slotPrefab;
		[SerializeField] Sprite _selectedSlotSprite;
		[SerializeField] Sprite _unselectedSlotSprite;

		private GridLayoutGroup _grid;
		private GameObject _currentSlotObject;
		private int _currentSlotNumber;
		
		public List<GameObject> Slots { get; private set; }
		
		public GameObject FirstEmptySlot
		{
			get
			{
				int count = Slots.Count;
				for ( int i = 0; i < count; ++i )
				{
					GameObject obj = Slots[i];
					if ( obj.transform.childCount == 0 )
					{
						return obj;
					}
				}

				Debug.Log( "Grid is full!" );
				return null;
			}
		}

		void Awake( )
		{
			if (Serializer.IsLoading) return;

			Slots = new List<GameObject>( );
			_grid = GetComponent<GridLayoutGroup>( );

			for ( int i = 0; i < _maxSlots; ++i )
			{
				Slots.Add( Instantiate( _slotPrefab, this.transform ) );
			}

			HighlightSlot( 1 );
		}

		void LateUpdate( )
		{
			if ( Serializer.IsLoading ) return;

			float parentX = m_parentPanelRect.sizeDelta.x;
			//float parentY = m_parentPanelRect.sizeDelta.y;

			float cellX = _grid.cellSize.x + _grid.spacing.x;
			//float cellY = m_grid.cellSize.y + m_grid.spacing.y;

			int numColumns = Mathf.FloorToInt( parentX / cellX );

			_grid.constraintCount = numColumns;
		}

		public void Clear( )
		{
			Slots.Clear( );
		}

		public void HighlightSlot( int slotNumber )
		{
			if ( _currentSlotObject != null )
			{ 
				_currentSlotObject.GetComponent<Image>( ).sprite = _unselectedSlotSprite;
			}

			_currentSlotNumber = slotNumber;
			_currentSlotObject = Slots[slotNumber];
			_currentSlotObject.GetComponent<Image>( ).sprite = _selectedSlotSprite;
		}

		public void MoveSlotHighlight( int x, int y )
		{
			int numColumns = _grid.constraintCount;
			int numRows = Mathf.CeilToInt( _maxSlots / numColumns );

			int currentRow = _currentSlotNumber / numColumns;
			int currentCol = _currentSlotNumber % numColumns;

			int finalRow = currentRow - y;
			int finalCol = currentCol + x;

			if ( finalRow >= 0 )
				finalRow = finalRow % numRows;
			else
				finalRow = numRows + finalRow;

			if ( finalCol >= 0 )
				finalCol = finalCol % numColumns;
			else
				finalCol = numColumns + finalCol;

			//finalCol = Mathf.Clamp( finalCol, 0, numColumns - 1 );

			int finalSlotNumber = finalRow * numColumns + finalCol;
			//finalSlotNumber = Mathf.Clamp( finalSlotNumber, 0, _maxSlots - 1 );

			HighlightSlot( finalSlotNumber );
		}
	}


}
