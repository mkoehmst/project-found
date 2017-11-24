using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using ProjectFound.CameraUI;
using ProjectFound.Environment.Items;
using System;

namespace ProjectFound.Master {


	public class UIMaster
	{
		public InventoryUI InventoryUI { get; set; }
		public OnScreenUI OnScreenUI { get; set; }
		public PropCollectionUI PropCollectionUI { get; set; }
		public DetectionRadius DetectionRadius { get; set; }
		//public ActionBarUI ActionBarUI { get; set; }
		//public PauseMenuUI PauseMenuUI { get; set; }

		public UIMaster( )
		{
			InventoryUI = GameObject.FindObjectOfType<InventoryUI>( );
			InventoryUI.gameObject.SetActive( false );

			OnScreenUI = GameObject.FindObjectOfType<OnScreenUI>( );

			PropCollectionUI = GameObject.FindObjectOfType<PropCollectionUI>( );
			PropCollectionUI.gameObject.SetActive( false );

			DetectionRadius = GameObject.FindObjectOfType<DetectionRadius>( );
			DetectionRadius.gameObject.SetActive( false );
		}

		public void Loop( )
		{

		}

		public void DisplayPrompt( Prop prop, KeyCode key )
		{
			prop.Prompt = OnScreenUI.CreatePrompt( key, prop.PromptText, prop.IngameName );
		}

		public void RemovePrompt( Prop prop )
		{
			GameObject.Destroy( prop.Prompt );

			prop.Prompt = null;
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

		public void TogglePropCollectionWindow( )
		{
			bool isEnabled = PropCollectionUI.gameObject.activeInHierarchy;

			if ( isEnabled )
			{
				PropCollectionUI.ClearCollection( );
				PropCollectionUI.gameObject.SetActive( false );
			}
			else
			{
				PropCollectionUI.gameObject.SetActive( true );
			}
		}

		public UnityEngine.UI.Button AddPropCollectionButton( Prop prop )
		{
			return PropCollectionUI.AddProp( prop );
		}

		public void DisplayDetectionRadius( )
		{
			if ( DetectionRadius.gameObject.activeInHierarchy == false )
			{
				DetectionRadius.gameObject.SetActive( true );
			}
		}

		public void ClearDetectionRadius( )
		{
			if ( DetectionRadius.gameObject.activeInHierarchy == true )
			{
				DetectionRadius.gameObject.SetActive( false );
			}
		}

		public void RemovePropCollectionProp( Prop prop )
		{
			PropCollectionUI.RemoveProp( prop );
		}
	}


}
