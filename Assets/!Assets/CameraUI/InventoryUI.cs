using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Assertions;

using ProjectFound.Environment.Items;

namespace ProjectFound.CameraUI {

public class InventoryUI : MonoBehaviour
{
	[SerializeField] GameObject _InventoryButtonPrefab = null;

	public Dictionary<Item,Button> Buttons { get; private set; }

	void Start( )
	{
		Assert.IsNotNull( _InventoryButtonPrefab );

		Buttons = new Dictionary<Item,Button>( );
	}

	public Button AddItem( Item item )
	{
		GameObject obj	= Instantiate( _InventoryButtonPrefab, gameObject.transform );
		Button button	= obj.GetComponent<Button>( );

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