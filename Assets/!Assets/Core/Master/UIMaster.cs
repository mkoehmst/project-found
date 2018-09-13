namespace ProjectFound.Core.Master 
{


	using UnityEngine;
	using UnityEngine.Assertions;
	using UnityEngine.EventSystems;

	using ProjectFound.CameraUI;
	using ProjectFound.Environment.Items;

	public class UIMaster
	{
		public EventSystem EventSystem { get; private set; }
		public InventoryUI InventoryUI { get; private set; }
		//public OnScreenUI OnScreenUI { get; set; }
		//public PropCollectionUI PropCollectionUI { get; set; }
		public DetectionRadius DetectionRadius { get; private set; }
		public DetectionUI DetectionUI { get; private set; } 
		//public ConductBarUI ConductBarUI { get; set; }
		//public PauseMenuUI PauseMenuUI { get; set; }

		public Rect ScreenRect { get; private set; }

		public void Initialize( )
		{
			InventoryUI = GameObject.FindObjectOfType<InventoryUI>( );
			Assert.IsNotNull( InventoryUI );

			EventSystem = EventSystem.current;
			Assert.IsNotNull( EventSystem );

			InventoryUI.Hide( );

			ScreenRect = new Rect( 0, 0, Screen.width, Screen.height );

			//OnScreenUI = GameObject.FindObjectOfType<OnScreenUI>( );

			//PropCollectionUI = GameObject.FindObjectOfType<PropCollectionUI>( );
			//PropCollectionUI.gameObject.SetActive( false );

			DetectionRadius = GameObject.FindObjectOfType<DetectionRadius>( );
			Assert.IsNotNull( DetectionRadius );

			DetectionUI = GameObject.FindObjectOfType<DetectionUI>( );
			Assert.IsNotNull( DetectionUI );

			//ConductBarUI = GameObject.FindObjectOfType<ConductBarUI>( );
		}

		public void Loop( )
		{
			if ( (Screen.width != ScreenRect.width) || (Screen.height != ScreenRect.height) )
			{
				ScreenRect = new Rect( 0, 0, Screen.width, Screen.height );
			}
		}

		public bool IsCursorOverUI( )
		{
			return EventSystem.IsPointerOverGameObject( );
		}

		//public void DisplayComment( Interactee i, string comment )
		//{
		//	InGameCommentUI igcUI = i.GetComponentInChildren<InGameCommentUI>( );

		//	igcUI.DisplayComment( comment );
		//}

		//public void DisplayPrompt( Prop prop, KeyCode key, Vector3 worldPos )
		//{
		//	prop.Prompt = OnScreenUI.CreatePrompt( key, prop.PromptText, prop.IngameName );
		//
		//	float offsX = Screen.width / 12f;
		//	float offsY = Screen.width / -12f;
		//
		//	Vector3 screenPos =
		//	Camera.main.WorldToScreenPoint( worldPos ) + new Vector3( offsX, offsY, 0f );
		//	
		//	prop.Prompt.transform.position = screenPos;
		//}

		//public void RemovePrompt( Prop prop )
		//{
		//	GameObject.Destroy( prop.Prompt );
		//
		//	prop.Prompt = null;
		//}


		public UnityEngine.UI.Button AddInventoryButton( Item item )
		{
			return InventoryUI.AddItem( item );
		}

		public void RemoveInventoryButton( Item item )
		{
			InventoryUI.RemoveItem( item );
		}

		public void CloseAllWindows( )
		{
			// TODO: Generic, automated solution. Keep track of open windows?
			if ( !InventoryUI.IsHidden ) InventoryUI.Hide( );
			if ( !DetectionUI.IsHidden ) DetectionUI.Hide( );
		}

		//public UnityEngine.UI.Button AddSkillToConductBar( SkillSpec skill )
		//{
			//for ( int i = 0; i < skills.Length; ++i )
			//{
				//return ConductBarUI.AddConduct( skill.Icon );
			//}
		//}
	}


}
