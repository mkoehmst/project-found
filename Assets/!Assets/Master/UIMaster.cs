using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

using ProjectFound.CameraUI;
using ProjectFound.Environment;
using ProjectFound.Environment.Props;
using ProjectFound.Environment.Characters;
using System;

namespace ProjectFound.Master {


	public class UIMaster
	{
		public EventSystem EventSystem { get; private set; }
		public InventoryUI InventoryUI { get; set; }
		public OnScreenUI OnScreenUI { get; set; }
		public PropCollectionUI PropCollectionUI { get; set; }
		public DetectionRadius DetectionRadius { get; set; }
		public ConductBarUI ConductBarUI { get; set; }
		//public PauseMenuUI PauseMenuUI { get; set; }

		public Rect ScreenRect { get; private set; }

		public UIMaster( )
		{
			EventSystem = EventSystem.current;

			InventoryUI = GameObject.FindObjectOfType<InventoryUI>( );
			InventoryUI.gameObject.SetActive( false );

			OnScreenUI = GameObject.FindObjectOfType<OnScreenUI>( );

			PropCollectionUI = GameObject.FindObjectOfType<PropCollectionUI>( );
			PropCollectionUI.gameObject.SetActive( false );

			DetectionRadius = GameObject.FindObjectOfType<DetectionRadius>( );
			DetectionRadius.gameObject.SetActive( false );

			ConductBarUI = GameObject.FindObjectOfType<ConductBarUI>( );

			ScreenRect = new Rect( 0, 0, Screen.width, Screen.height );
		}

		public void Loop( )
		{
			if ( (Screen.width != ScreenRect.width) || (Screen.height != ScreenRect.height) )
			{
				ScreenRect = new Rect( 0, 0, Screen.width, Screen.height );
			}
		}

		public void DisplayComment( Interactee i, string comment )
		{
			InGameCommentUI igcUI = i.GetComponentInChildren<InGameCommentUI>( );

			igcUI.DisplayComment( comment );
		}

		public void DisplayPrompt( Prop prop, KeyCode key, Vector3 worldPos )
		{
			prop.Prompt = OnScreenUI.CreatePrompt( key, prop.PromptText, prop.IngameName );

			float offsX = Screen.width / 12f;
			float offsY = Screen.width / -12f;

			Vector3 screenPos =
				Camera.main.WorldToScreenPoint( worldPos ) + new Vector3( offsX, offsY, 0f );

			prop.Prompt.transform.position = screenPos;
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

		//public void CombatMovementInRange( Vector3 loc )
		//{

		//}

		public void RemovePropCollectionProp( Prop prop )
		{
			PropCollectionUI.RemoveProp( prop );
		}

		public UnityEngine.UI.Button AddSkillToConductBar( SkillSpec skill )
		{
			//for ( int i = 0; i < skills.Length; ++i )
			//{
				return ConductBarUI.AddConduct( skill.Icon );
			//}
		}

		public bool IsOverUIElement( )
		{
			return EventSystem.IsPointerOverGameObject( );
		}
	}


}
