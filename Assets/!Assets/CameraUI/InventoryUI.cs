using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Assertions;

using ProjectFound.Environment.Props;

namespace ProjectFound.CameraUI {


	public class InventoryUI : MonoBehaviour
	{
		[SerializeField] GameObject m_inventoryButtonPrefab;

		public Dictionary<Item,Button> Buttons { get; private set; }
		public WidgetItemGridUI ItemGrid { get; private set; }

		void Awake( )
		{
			ItemGrid = GetComponentInChildren<WidgetItemGridUI>( );
			Assert.IsNotNull( ItemGrid );

			Buttons = new Dictionary<Item,Button>( );
		}

		void Start( )
		{
			Assert.IsNotNull( m_inventoryButtonPrefab );
		}

		public Button AddItem( Item item )
		{
			GameObject slot = ItemGrid.FirstEmptySlot;

			if ( slot == null )
				return null;

			GameObject obj = Instantiate( m_inventoryButtonPrefab, slot.transform );
			Button button = obj.GetComponent<Button>( );
			Image img = obj.GetComponent<Image>( );
			img.sprite = item.Icon;

			Buttons.Add( item, button );

			return button;
		}

		public void RemoveItem( Item item )
		{
			Destroy( Buttons[item].gameObject );

			Buttons.Remove( item );
		}
	}


}