namespace ProjectFound.Core
{

	using System.Collections.Generic;

	using UnityEngine;

	using ProjectFound.Master;
	using ProjectFound.Environment;
	using ProjectFound.Environment.Props;
	using ProjectFound.Environment.Characters;
	using ProjectFound.Environment.Occlusion;

	public class GameContext
	{
		#region Properties
		public RaycastMaster RaycastMaster { get; } = new RaycastMaster( );
		public InputMaster InputMaster { get; } = new InputMaster( );
		public PlayerMaster PlayerMaster { get; } = new PlayerMaster( );
		public CameraMaster CameraMaster { get; } = new CameraMaster( );
		public UIMaster UIMaster { get; } = new UIMaster( );
		public ShaderMaster ShaderMaster { get; } = new ShaderMaster( );
		public CombatMaster CombatMaster { get; } = new CombatMaster( );
		public InteractionMaster InteractionMaster { get; } = new InteractionMaster( );
		#endregion

		#region Constructor
		public GameContext( )
		{
			ContextHandler.AssignContext( this );
		}
		#endregion

		#region Setup and Loop
		public void Setup( )
		{
			//PlayerMaster.Setup( );

			//SetupConductBar( );
			SetupRaycasters( );
			SetInputTracker( );
			SetCombatDelegates( );

			LoadInputMappings( );
		}

		public void Loop( )
		{
			InputMaster.TrackingLoop( );
			RaycastMaster.Loop( );
			InputMaster.MappingLoop( );
			PlayerMaster.Loop( );
			CameraMaster.Loop( );
			CombatMaster.Loop( );
		}

		private void SetupSkillBook( )
		{
			/*Skill[] skills = PlayerMaster.SkillBook.Skills;
			for ( int i = 0; i < skills.Length; ++i )
			{
				Skill skill = skills[i];


			}*/
		}

		private void SetupConductBar( )
		{
			Skill[] skills = PlayerMaster.ConductBarSkills;
			for ( int i = 0; i < skills.Length; ++i )
			{
				Skill skill = skills[i];

				if ( skill != null )
				{
					AddSkillToConductBar( skill );
				}
			}
		}

		private void AddSkillToConductBar( Skill skill )
		{
			UnityEngine.UI.Button button =
					UIMaster.AddSkillToConductBar( skill.Specification );

			button.onClick.AddListener( ( ) =>
			{
				PlayerMaster.Player.DelegateCombatHandler = skill.Handle;
			} );
		}

		protected void SetupRaycasters( )
		{
			RaycastMaster.Setup( );

			var cursorSelection = RaycastMaster.CursorSelectionRaycaster;

			cursorSelection.AddPriority( LayerID.UI );
			cursorSelection.AddPriority( LayerID.Walkable );
			cursorSelection.AddPriority( LayerID.Enemy );
			cursorSelection.AddPriority( LayerID.Item );
			cursorSelection.AddPriority( LayerID.Prop );
			cursorSelection.AddBlocker( LayerID.Default );
			cursorSelection.AddBlocker( LayerID.Roof );
			cursorSelection.AddBlocker( LayerID.Usable );

			cursorSelection.DelegateLineTracking = ( ref Vector3 start, ref Vector3 end ) =>
			{
				start = InputMaster.PreviousMousePosition;
				end = InputMaster.MousePosition;
			};

			cursorSelection.DelegateCasterAssignments = ( ref Ray ray, ref Vector3 screenPos ) =>
			{
				ray = Camera.main.ScreenPointToRay( screenPos );
			};

			cursorSelection.SetLayerDelegates(
				LayerID.Prop, OnCursorFocusGained, OnCursorFocusLost );

			cursorSelection.SetLayerDelegates(
				LayerID.Item, OnCursorFocusGained, OnCursorFocusLost );

			var combatCursorSelection = RaycastMaster.CombatCursorSelectionRaycaster;

			combatCursorSelection.AddPriority( LayerID.Walkable );

			combatCursorSelection.DelegateHitFound = OnCombatRaycastHit;

			combatCursorSelection.DelegateCasterAssignment = ( ref Ray ray ) =>
			{
				ray = Camera.main.ScreenPointToRay( InputMaster.MousePosition );
			};

			var holdToMove = RaycastMaster.HoldToMoveRaycaster;

			holdToMove.AddPriority( LayerID.Walkable );

			holdToMove.DelegateCasterAssignment = ( ref Ray ray ) =>
			{
				ray = Camera.main.ScreenPointToRay( InputMaster.MousePosition );
			};

			var propPlacement = RaycastMaster.PropPlacementRaycaster;

			//propPlacement.AddPriority( LayerID.Item );
			propPlacement.AddPriority( LayerID.Prop );
			propPlacement.AddPriority( LayerID.Usable );
			propPlacement.AddPriority( LayerID.Walkable );
			//propPlacement.AddPriority( LayerID.Default );

			propPlacement.DelegateCasterAssignment = ( ref Ray ray ) =>
			{
				ray = Camera.main.ScreenPointToRay( InputMaster.MousePosition );
			};

			var cameraOcclusion = RaycastMaster.CameraOcclusionRaycaster;

			cameraOcclusion.AddPriority( LayerID.Roof );
			cameraOcclusion.Target = PlayerMaster.Player.transform;

			cameraOcclusion.DelegateOcclusionEnable = OnPlayerOccluded;
			cameraOcclusion.DelegateOcclusionDisable = OnPlayerNotOccluded;

			cameraOcclusion.DelegateCasterAssignment = ( ref Ray ray ) =>
			{
				ray.origin = Camera.main.transform.position;
				ray.direction = (cameraOcclusion.Target.position - ray.origin).normalized;
			};

			RaycastMaster.SwitchTo( cursorSelection );
		}

		private void SetInputTracker( )
		{
			InputMaster.DelegateInputTracker = OnInputTracking;
			InputMaster.DelegateCursorLost = OnCursorLost;
			InputMaster.DelegateCursorGained = OnCursorGained;
		}

		private void SetCombatDelegates( )
		{
			CombatMaster.SetCombatBeginDelegate( OnCombatBegin );
		}
		#endregion

		#region Input Mappings
		protected void LoadInputMappings( )
		{
			LoadMouseAndKeyboardMappings( );
			LoadGamepadMappings( );
		}

		protected void LoadMouseAndKeyboardMappings( )
		{
			InputMaster.AddNewDevice( InputMaster.InputDevice.MouseAndKeyboard );

			InputMaster.MapKey( true, OnCursorSelect, KeyCode.Mouse0 );
			InputMaster.MapAxis( true, OnCameraMoveHorizontal, "KeyboardCameraHorizontal" );
			InputMaster.MapAxis( true, OnCameraMoveVertical, "KeyboardCameraVertical" );
			InputMaster.MapAxis( true, OnCameraRotation, "KeyboardCameraRotation" );
			InputMaster.MapAxis( true, OnCameraZoom, "Mouse ScrollWheel" );
			InputMaster.MapKey( true, OnCameraAttach, KeyCode.Home );
			InputMaster.MapKey( true, OnCameraRotationMod, KeyCode.Mouse2 );
			InputMaster.MapKey( true, OnInventoryToggle, KeyCode.I );
			InputMaster.MapKey( true, OnPropCollectionToggle, KeyCode.P );

			InputMaster.MapKey( false, OnCombatAttack, KeyCode.Alpha1 );
		}

		protected void LoadGamepadMappings( )
		{
			InputMaster.AddNewDevice( InputMaster.InputDevice.Gamepad );

			InputMaster.MapAxis( false, OnCameraMoveHorizontal, "ControllerCameraHorizontal" );
			InputMaster.MapAxis( false, OnCameraMoveVertical, "ControllerCameraVertical" );
			InputMaster.MapAxis( true, OnCameraRotation, "ControllerCameraRotation" );
			InputMaster.MapAxis( true, OnCameraZoom, "ControllerCameraZoom" );
			InputMaster.MapAxii( true, OnPlayerMovement,
				"ControllerMovementHorizontal", "ControllerMovementVertical" );
			InputMaster.MapKey( true, OnCameraAttachToggle, KeyCode.Joystick1Button11 );
			InputMaster.MapKey( true, OnCursorSelect, KeyCode.Joystick1Button1 );
			InputMaster.MapKey( true, OnInventoryToggle, KeyCode.Joystick1Button9 );
			InputMaster.MapKey( true, OnDetectionRadiusToggle, KeyCode.Joystick1Button0 );
			InputMaster.MapKey( true, OnPropCollectionToggle, KeyCode.Joystick1Button2 );
		}
		#endregion

		#region Master Callbacks
		public void OnInputTracking( InputMaster.InputDevice device )
		{
			//RaycastMaster.CursorDevice = device;
		}

		public void OnCombatBegin( List<Combatant> combatants )
		{
			RaycastMaster.CurrentInteracteeRaycaster.SetEnabled( false );

			RaycastMaster.CurrentInteracteeRaycaster =
				RaycastMaster.CombatCursorSelectionRaycaster;

			RaycastMaster.CurrentInteracteeRaycaster.SetEnabled( true );

			//InputMaster.DisableMap( InputMaster.GetKeyFromAction( )

			//InputMaster.KeyMaps[InputMaster.GetKeyFromAction( OnCombatA)]
		}

		public void OnCombatRaycastHit( ref RaycastHit hit )
		{
			if ( PlayerMaster.CanMoveTo( hit.point ) == false )
			{
				PlayerMaster.CombatMovementFeedback( hit.point, false );
				return;
			}

			float distance = PlayerMaster.NavMeshDistanceTo( );
			int actionPointCost =
				CombatMaster.CalculateMovementCost( PlayerMaster.Player as Combatant, distance );

			if ( CombatMaster.HasEnoughActionPoints(
				PlayerMaster.Player as Combatant, actionPointCost ) )
			{
				PlayerMaster.CombatMovementFeedback( hit.point, true );
			}
			else
			{
				PlayerMaster.CombatMovementFeedback( hit.point, false );
			}
		}

		public void OnCombatAttack( InputMaster.KeyMap map )
		{
			if ( map.Mode == InputMaster.KeyMode.OneShotRelease )
			{
				//CombatMaster.CalculateMovementCost( PlayerMaster.DistanceToTarget, PlayerMaster.MovementScore );
				//CombatMaster.MoveCombatant( (ref Vector3 destination, int actionPointsLeft) =>
				//{
				//     PlayerMaster.MoveTo( ref destination );
				//	   UIMaster.UpdateActionPoints( actionPointsLeft );
				//} );
			}
		}

		public void OnPlayerOccluded( Occludable occludable )
		{
			//Debug.Log( "OnPlayerOccluded()" );
			PlayerMaster.RunFocusChain( occludable );
		}

		public void OnPlayerNotOccluded( Occludable occludable )
		{
			//Debug.Log( "OnPlayerNotOccluded()" );
			PlayerMaster.RunFocusReleaseChain( occludable );
		}

		public void OnCursorLost( )
		{
			//Debug.Log( "OnCursorLost()" );
			RaycastMaster.Reset( );
			InputMaster.GetKeyMap( OnCursorSelect ).ResetMode( );
			PlayerMaster.AbortInteractionChains( );
		}

		public void OnCursorGained( )
		{
			//Debug.Log( "OnCursorGained()" );
			RaycastMaster.SwitchTo( RaycastMaster.CursorSelectionRaycaster );
		}

		public void OnCursorFocusGained( KeyValuePair<Interactee, RaycastHit> pair )
		{
			PlayerMaster.RunFocusChain( pair.Key );
		}

		public void OnCursorFocusLost( Interactee interactee )
		{
			PlayerMaster.KillFocusChain( );
			PlayerMaster.RunFocusReleaseChain( interactee );
		}

		private void OnCursorSelectOneShot( InputMaster.KeyMap map )
		{
			var raycaster = RaycastMaster.CurrentInteracteeRaycaster;
			var report = raycaster.Report;

			if ( raycaster.Mode == RaycastMaster.RaycastMode.CursorSelection )
			{
				if ( report.layerID == LayerID.Walkable )
				{
					map.OpenHoldingWindow( 0.40f );

					PlayerMaster.RunOneShotChain( report.component as Interactee );
				}
				else if ( report.layerID == LayerID.Item || report.layerID == LayerID.Prop )
				{
					map.OpenHoldingWindow( 0.40f );
				}
			}
		}

		private void OnCursorSelectOneShotRelease( InputMaster.KeyMap map )
		{
			var raycaster = RaycastMaster.CurrentInteracteeRaycaster;
			var report = raycaster.Report;

			if ( raycaster.Mode == RaycastMaster.RaycastMode.CursorSelection )
			{
				if ( report.layerID == LayerID.Item || report.layerID == LayerID.Prop )
				{
					PlayerMaster.RunOneShotReleaseChain( report.component as Interactee );
				}
			}
		}

		private void OnCursorSelectWindow( InputMaster.KeyMap map )
		{
			var raycaster = RaycastMaster.CurrentInteracteeRaycaster;
			var report = raycaster.Report;

			if ( raycaster.Mode == RaycastMaster.RaycastMode.CursorSelection )
			{
				if ( report.layerID == LayerID.Walkable )
				{
					PlayerMaster.RunWindowChain( report.component as Interactee );
				}
				else if ( report.layerID == LayerID.Prop || report.layerID == LayerID.Item )
				{
					PlayerMaster.RunWindowChain( report.component as Interactee );
				}
			}
		}

		private void OnCursorSelectWindowRelease( InputMaster.KeyMap map )
		{
			var raycaster = RaycastMaster.CurrentInteracteeRaycaster;
			var report = raycaster.Report;

			if ( raycaster.Mode == RaycastMaster.RaycastMode.HoldToMove )
			{
				PlayerMaster.RunWindowReleaseChain( report.component as Interactee );
			}
			else if ( raycaster.Mode == RaycastMaster.RaycastMode.PropPlacement )
			{
				PlayerMaster.RunWindowReleaseChain( PlayerMaster.PropBeingPlaced );
			}
		}

		private void OnCursorSelectHolding( InputMaster.KeyMap map )
		{
			var raycaster = RaycastMaster.CurrentInteracteeRaycaster;
			var report = raycaster.Report;

			if ( raycaster.Mode == RaycastMaster.RaycastMode.HoldToMove )
			{
				if ( report.layerID == LayerID.Walkable )
				{
					PlayerMaster.RunHoldingChain( report.component as Interactee );
				}
			}

			else if ( raycaster.Mode == RaycastMaster.RaycastMode.PropPlacement )
			{
				PlayerMaster.RunHoldingChain( PlayerMaster.PropBeingPlaced );
			}
		}

		private void OnCursorSelectHoldingRelease( InputMaster.KeyMap map )
		{
			var raycaster = RaycastMaster.CurrentInteracteeRaycaster;
			var report = raycaster.Report;

			if ( raycaster.Mode == RaycastMaster.RaycastMode.HoldToMove )
			{
				PlayerMaster.RunHoldingReleaseChain( report.component as Interactee );
			}
			else if ( raycaster.Mode == RaycastMaster.RaycastMode.PropPlacement )
			{
				PlayerMaster.RunHoldingReleaseChain( PlayerMaster.PropBeingPlaced );
			}
		}

		public void OnCursorSelect( InputMaster.KeyMap map )
		{
			if ( UIMaster.IsCursorOverUI( ) )
			{
				// The UGUI system has its own input handling and we want the UI to block
				return;
			}

			//Debug.Log("OnCursorSelect()");

			switch ( map.Mode )
			{
				case InputMaster.KeyMode.OneShot:
					PlayerMaster.KillOneShotReleaseChain( );
					PlayerMaster.KillWindowReleaseChain( );
					PlayerMaster.KillHoldingReleaseChain( );
					OnCursorSelectOneShot( map );
					break;

				case InputMaster.KeyMode.OneShotRelease:
					PlayerMaster.KillOneShotChain( );
					if ( map.HoldingWindow.HasValue )
					{
						PlayerMaster.KillWindowChain( );
						OnCursorSelectWindowRelease( map );
					}
					OnCursorSelectOneShotRelease( map );
					break;

				case InputMaster.KeyMode.HoldingWindow:
					OnCursorSelectWindow( map );
					break;

				case InputMaster.KeyMode.Holding:
					PlayerMaster.KillOneShotChain( );
					PlayerMaster.KillWindowChain( );
					OnCursorSelectHolding( map );
					break;

				case InputMaster.KeyMode.HoldingRelease:
					PlayerMaster.KillHoldingChain( );
					OnCursorSelectHoldingRelease( map );
					break;
			}
		}

		public void OnCameraMoveHorizontal( InputMaster.AxisMap map, float movement )
		{
			CameraMaster.FixedTiltZoomableCamera.HandleMovement( 0f, movement );
		}

		public void OnCameraMoveVertical( InputMaster.AxisMap map, float movement )
		{
			CameraMaster.FixedTiltZoomableCamera.HandleMovement( movement, 0f );
		}

		public void OnCameraRotation( InputMaster.AxisMap map, float movement )
		{
			CameraMaster.FixedTiltZoomableCamera.HandleRotation( movement );
		}

		public void OnCameraZoom( InputMaster.AxisMap map, float movement )
		{
			CameraMaster.FixedTiltZoomableCamera.HandleZoom( movement );
		}

		public void OnPlayerMovement( InputMaster.AxiiMap map, float[] movements )
		{
			if ( movements.Length != 2 )
				return;

			//PlayerMaster.CharacterMovement.TranslateMoveTarget( movements[0], movements[1] );
		}

		public void OnCameraAttachToggle( InputMaster.KeyMap map )
		{
			if ( map.Mode == InputMaster.KeyMode.OneShotRelease )
			{
				PlayerMaster.RunOneShotChain( CameraMaster.FixedTiltZoomableCamera );
			}
		}

		public void OnCameraAttach( InputMaster.KeyMap map )
		{
			if ( map.Mode == InputMaster.KeyMode.OneShot )
			{
				CameraMaster.FixedTiltZoomableCamera.HandleAttachment( );
			}
		}

		public void OnCameraRotationMod( InputMaster.KeyMap map )
		{
			//Debug.Log("OnCameraRotationMod()");

			switch ( map.Mode )
			{
				case InputMaster.KeyMode.OneShot:
					map.OpenHoldingWindow( 0f );
					break;
				case InputMaster.KeyMode.Holding:
					PlayerMaster.RunHoldingChain( CameraMaster.FixedTiltZoomableCamera );
					break;
				case InputMaster.KeyMode.HoldingRelease:
					PlayerMaster.KillHoldingChain( );
					break;
			}
		}

		public void OnInventoryToggle( InputMaster.KeyMap map )
		{
			if ( map.Mode == InputMaster.KeyMode.OneShot )
			{
				UIMaster.ToggleInventoryWindow( );
			}
		}

		public void OnPropCollectionToggle( InputMaster.KeyMap map )
		{
			if ( map.Mode == InputMaster.KeyMode.OneShot )
			{
				UIMaster.TogglePropCollectionWindow( );
			}
		}

		public void OnDetectionRadiusToggle( InputMaster.KeyMap map )
		{
			if ( map.Mode == InputMaster.KeyMode.OneShot )
			{
				map.OpenHoldingWindow( 0.40f );
			}
			else if ( map.Mode == InputMaster.KeyMode.Holding )
			{
				Debug.Log( "DetectionRadiusBegin" );

				UIMaster.DisplayDetectionRadius( );
			}
			else if ( map.Mode == InputMaster.KeyMode.HoldingRelease )
			{
				UIMaster.DetectionRadius.GatherDetections( );

				foreach ( Collider coll in UIMaster.DetectionRadius.ObjectsWithin )
				{
					GameObject obj = coll.gameObject;
					LayerID layer = (LayerID)obj.layer;

					switch ( layer )
					{
						case LayerID.Item:
							Item item = obj.GetComponent<Item>( );
							AddItemToPropCollection( item );
							break;
						case LayerID.Prop:
							Prop prop = obj.GetComponent<Prop>( );
							AddPropToPropCollection( prop );
							break;
					}
				}

				UIMaster.ClearDetectionRadius( );

				if ( UIMaster.PropCollectionUI.Buttons.Count > 0 )
				{
					UIMaster.TogglePropCollectionWindow( );
				}
			}
		}
		#endregion

		#region Misc Methods

		protected void AddItemToPropCollection( Item item )
		{
			/*UIMaster.AddPropCollectionButton( item as Prop ).onClick.AddListener( () =>
			{
				PlayerMaster.PickUp( item, () =>
				{
					AddToInventory( item );
					UIMaster.RemovePropCollectionProp( item as Prop );
				} );
			} );*/
		}

		protected void AddPropToPropCollection( Prop prop )
		{
			/*UIMaster.AddPropCollectionButton( prop ).onClick.AddListener( () =>
			{
				PlayerMaster.Activate( prop, () =>
				{
					UIMaster.RemovePropCollectionProp( prop );
				} );
			} );*/
		}
		#endregion
	}

}
