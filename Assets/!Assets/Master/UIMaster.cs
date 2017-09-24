using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using ProjectFound.CameraUI;
using ProjectFound.Environment.Items;

namespace ProjectFound.Master {


	public class UIMaster
	{
		public InventoryUI InventoryUI { get; set; }
		public OnScreenUI OnScreenUI {  get; set; }
		//public ActionBarUI ActionBarUI { get; set; }
		//public PauseMenuUI PauseMenuUI { get; set; }

		public UIMaster( )
		{
			InventoryUI = GameObject.FindObjectOfType<InventoryUI>( );
			OnScreenUI = GameObject.FindObjectOfType<OnScreenUI>( );
		}

		public void Loop( )
		{

		}

		public void DisplayPrompt( Item item, KeyCode key )
		{
			item.Prompt = OnScreenUI.CreatePrompt( key, item.PromptText );
		}

		public void RemovePrompt( Item item )
		{
			GameObject.Destroy( item.Prompt );

			item.Prompt = null;
		}

		public void ToggleInventoryWindow( )
		{
			InventoryUI.gameObject.SetActive( !InventoryUI.gameObject.activeInHierarchy );
		}

		public UnityEngine.UI.Button AddInventoryButton( Item item )
		{
			return InventoryUI.AddItem( item );
		}

		public void RemoveInventoryButton( Item item )
		{
			InventoryUI.RemoveItem( item );
		}
	}


}
