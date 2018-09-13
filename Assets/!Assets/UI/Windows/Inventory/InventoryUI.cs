namespace ProjectFound.CameraUI 
{


	using System.Collections.Generic;
	
	using UnityEngine;
	using UnityEngine.UI;
	using UnityEngine.Assertions;
	using Autelia.Serialization;

	using ProjectFound.Environment.Items;

	public class InventoryUI : WindowUI
	{
		[SerializeField] GameObject m_inventoryButtonPrefab;

		public Dictionary<Item,Button> Buttons { get; private set; }
		public WidgetItemGridUI ItemGrid { get; private set; }
		public DollCamera DollCamera { get; private set; }

		new void Awake( )
		{
			base.Awake( );

			if (Serializer.IsLoading) return;

			ItemGrid = GetComponentInChildren<WidgetItemGridUI>( );
			Assert.IsNotNull( ItemGrid );

			Buttons = new Dictionary<Item,Button>( );

			DollCamera = FindObjectOfType<DollCamera>( );
			Assert.IsNotNull( DollCamera );

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

		new public void Toggle( )
		{
			base.Toggle( );

			if ( IsHidden )
			{ 
				DollCamera.Disable( );
			}
			else
			{
				DollCamera.Enable( );
			}
		}
	}


}