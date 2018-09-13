namespace ProjectFound.Core
{


	#region NAMESPACES
	using System.Collections.Generic;

	using UnityEngine;

	using ProjectFound.Core.Master;
	using ProjectFound.Environment;
	using ProjectFound.Environment.Surfaces;
	using ProjectFound.Interaction;
	#endregion

	public class GameContext
	{

		#region PROPERTIES
		public RaycastMaster RaycastMaster { get; } = new RaycastMaster( );
		public InputMaster InputMaster { get; } = new InputMaster( );
		public PlayerMaster PlayerMaster { get; } = new PlayerMaster( );
		public CameraMaster CameraMaster { get; } = new CameraMaster( );
		public UIMaster UIMaster { get; } = new UIMaster( );
		public ShaderMaster ShaderMaster { get; } = new ShaderMaster( );
		public CombatMaster CombatMaster { get; } = new CombatMaster( );
		#endregion

		#region SETUP
		public void Initialize( )
		{
			InputMaster.Initialize( );
			UIMaster.Initialize( );
		}


		public void Setup( )
		{
			//PlayerMaster.Setup( );

			//SetupConductBar( );
			SetupRaycasters( );
			SetInputTracking( );
			//SetCombatDelegates( );

			LoadInputMappings( );
		}
		#endregion

		#region INPUT MAPPING
		protected void LoadInputMappings( )
		{
			LoadMouseAndKeyboardMappings( );
			LoadGamepadMappings( );
		}

		protected void LoadMouseAndKeyboardMappings( )
		{
			InputMaster.AddNewDevice( InputMaster.InputDevice.MouseAndKeyboard );

			InputMaster.MapButton( "CursorSelect",
				OnSelectOneShot, OnSelectOneShotRelease,
				OnSelectWindow, OnSelectWindowRelease,
				OnSelectHolding, OnSelectHoldingRelease );
			InputMaster.MapAxis( "CameraZoom", OnCameraZoom );
			InputMaster.MapAxis( "CameraRoamHorizontal", OnCameraRoamHorizontal );
			InputMaster.MapAxis( "CameraRoamVertical", OnCameraRoamVertical );
			InputMaster.MapButton( "CameraAttach", 
				delegateOneShot: OnCameraAttach );
			InputMaster.MapButton( "CameraRotateMod", 
				delegateWindow: OnCameraRotateModEnable,
				delegateWindowRelease: OnCameraRotateModDisable );
			InputMaster.MapAxis( "CameraRotate", OnCameraRotate, 
				axisCondition: () => CameraMaster.Camera.IsRotateModEnabled );
			InputMaster.MapButton( "InventoryToggle", 
				delegateOneShot: OnInventoryToggle );
			InputMaster.MapButton( "StandardCancel",
				delegateOneShot: OnStandardCancel );
			InputMaster.MapButton( "BrowseRight", 
				delegateOneShot: OnBrowseRight );
			InputMaster.MapButton( "BrowseLeft", 
				delegateOneShot: OnBrowseLeft );
			InputMaster.MapButton( "BrowseUp",
				delegateOneShot: OnBrowseUp );
			InputMaster.MapButton( "BrowseDown",
				delegateOneShot: OnBrowseDown );
			InputMaster.MapButton( "ActionBar1",
				delegateOneShot: OnActionBar1 );
			InputMaster.MapButton( "ActionBar2",
				delegateOneShot: OnActionBar2 );

			//InputMaster.MapAxis( true, OnCameraRotation, "KeyboardCameraRotation" );
			//InputMaster.MapKey( true, OnInventoryToggle, KeyCode.I );
			//InputMaster.MapKey( true, OnPropCollectionToggle, KeyCode.P );
			//InputMaster.MapKey( false, OnCombatAttack, KeyCode.Alpha1 );
		}

		protected void LoadGamepadMappings( )
		{
			InputMaster.AddNewDevice( InputMaster.InputDevice.Gamepad );

			InputMaster.MapButton( "ContextualSelect", 
				delegateOneShot: OnContextualSelect,
				oneShotCondition: () => RaycastMaster.PropProximityRaycaster.HasDetection );
			InputMaster.MapAxis( "CameraRotateDirect", OnCameraRotate );
			InputMaster.MapDualAxis( "ProtagonistMoveX", "ProtagonistMoveY", OnProtagonistMove );
			InputMaster.MapButton( "PropDetectionRadius", 
				delegateHolding: OnPropDetectionRadiusEnable,
				delegateHoldingRelease: OnPropDetectionRadiusDisable );
			InputMaster.MapButton( "StandardCancel",
				delegateOneShot: OnStandardCancel );
			//InputMaster.MapAxis( "")

			/*InputMaster.MapAxis( false, OnCameraMoveHorizontal, "ControllerCameraHorizontal" );
			InputMaster.MapAxis( false, OnCameraMoveVertical, "ControllerCameraVertical" );
			InputMaster.MapAxis( true, OnCameraRotation, "ControllerCameraRotation" );
			InputMaster.MapAxis( true, OnCameraZoom, "ControllerCameraZoom" );
			InputMaster.MapAxii( true, OnPlayerMovement,
				"ControllerMovementHorizontal", "ControllerMovementVertical" );
			InputMaster.MapKey( true, OnCameraAttachToggle, KeyCode.Joystick1Button11 );
			InputMaster.MapKey( true, OnCursorSelect, KeyCode.Joystick1Button1 );
			InputMaster.MapKey( true, OnInventoryToggle, KeyCode.Joystick1Button9 );
			InputMaster.MapKey( true, OnDetectionRadiusToggle, KeyCode.Joystick1Button0 );
			InputMaster.MapKey( true, OnPropCollectionToggle, KeyCode.Joystick1Button2 );*/
		}
		#endregion

		#region CALLBACKS
		private void OnContextualSelect( InputMaster.ButtonMap map )
		{
			var raycaster = RaycastMaster.PropProximityRaycaster;
			var report = raycaster.Report;

			PlayerMaster.Protagonist.RunOneShotChain( 
				report.Component as Interactee );

			//raycaster.Clear( );
		}

		private void OnSelectOneShot( InputMaster.ButtonMap map )
		{
			if ( UIMaster.IsCursorOverUI( ) )
				return;

			var raycaster = RaycastMaster.CurrentInteracteeRaycaster;
			var report = raycaster.Report;

			PlayerMaster.Protagonist.RunOneShotChain( report.Component as Interactee );
		}

		private void OnSelectOneShotRelease( InputMaster.ButtonMap map )
		{
			if ( UIMaster.IsCursorOverUI( ) )
				return;

			var raycaster = RaycastMaster.CurrentInteracteeRaycaster;
			var report = raycaster.Report;

			if ( report.Mode == RaycastMaster.RaycastMode.CursorSelection )
			{
				if ( report.HitLayerID == LayerID.Item || report.HitLayerID == LayerID.Prop )
				{
					//PlayerMaster.Protagonist.KillOneShotChain( );

					PlayerMaster.Protagonist.RunOneShotReleaseChain( 
						report.Component as Interactee );
				}
			}
		}

		private void OnSelectWindow( InputMaster.ButtonMap map )
		{
			if ( UIMaster.IsCursorOverUI( ) )
				return;

			var raycaster = RaycastMaster.CurrentInteracteeRaycaster;
			var report = raycaster.Report;

			if ( report.Mode == RaycastMaster.RaycastMode.CursorSelection )
			{
				if ( report.HitLayerID == LayerID.Walkable )
				{
					PlayerMaster.Protagonist.RunWindowChain( 
						report.Component as Interactee );
				}
				else if ( report.HitLayerID == LayerID.Prop || report.HitLayerID == LayerID.Item )
				{
					PlayerMaster.Protagonist.RunWindowChain( 
						report.Component as Interactee );
				}
			}
		}

		private void OnSelectWindowRelease( InputMaster.ButtonMap map )
		{
			//PlayerMaster.KillOneShotChain( );
			//PlayerMaster.KillWindowChain( );

			if ( UIMaster.IsCursorOverUI( ) )
				return;

			var raycaster = RaycastMaster.CurrentInteracteeRaycaster;
			var report = raycaster.Report;

			if ( report.Mode == RaycastMaster.RaycastMode.HoldToMove )
			{
				PlayerMaster.Protagonist.RunWindowReleaseChain( 
					report.Component as Interactee );
			}
			else if ( report.Mode == RaycastMaster.RaycastMode.PropPlacement )
			{
				PlayerMaster.Protagonist.RunWindowReleaseChain( 
					PlayerMaster.Placeable.Interactee );
			}
		}

		private void OnSelectHolding( InputMaster.ButtonMap map )
		{
			if ( UIMaster.IsCursorOverUI( ) )
				return;

			var raycaster = RaycastMaster.CurrentInteracteeRaycaster;
			var report = raycaster.Report;

			if ( report.Mode == RaycastMaster.RaycastMode.HoldToMove )
			{
				if ( report.HitLayerID == LayerID.Walkable )
				{
					//PlayerMaster.KillOneShotChain( );

					PlayerMaster.Protagonist.RunHoldingChain( 
						report.Component as Interactee );
				}
			}

			else if ( report.Mode == RaycastMaster.RaycastMode.PropPlacement )
			{
				PlayerMaster.Protagonist.RunHoldingChain( 
					PlayerMaster.Placeable.Interactee );
			}
		}

		private void OnSelectHoldingRelease( InputMaster.ButtonMap map )
		{
			if ( UIMaster.IsCursorOverUI( ) )
				return;

			var raycaster = RaycastMaster.CurrentInteracteeRaycaster;
			var report = raycaster.Report;

			if ( report.Mode == RaycastMaster.RaycastMode.HoldToMove )
			{
				//PlayerMaster.KillHoldingChain( );

				PlayerMaster.Protagonist.RunHoldingReleaseChain( 
					report.Component as Interactee );
			}
			else if ( report.Mode == RaycastMaster.RaycastMode.PropPlacement )
			{
				PlayerMaster.Protagonist.RunHoldingReleaseChain( 
					PlayerMaster.Placeable.Interactee );
			}
		}

		public void OnCameraZoom( InputMaster.AxisMap map, float zoomAmount )
		{
			CameraMaster.Camera.HandleZoom( zoomAmount );
		}

		public void OnCameraRoamHorizontal( InputMaster.AxisMap map, float movement )
		{
			CameraMaster.Camera.HandleMovement( 0f, movement );
		}

		public void OnCameraRoamVertical( InputMaster.AxisMap map, float movement )
		{
			CameraMaster.Camera.HandleMovement( movement, 0f );
		}

		public void OnCameraAttach( InputMaster.ButtonMap map )
		{
			CameraMaster.Camera.HandleAttachment( );
		}

		public void OnCameraRotateModEnable( InputMaster.ButtonMap map )
		{
			CameraMaster.Camera.IsRotateModEnabled = true;
		}

		public void OnCameraRotateModDisable( InputMaster.ButtonMap map )
		{
			CameraMaster.Camera.IsRotateModEnabled = false;
		}

		public void OnCameraRotate( InputMaster.AxisMap map, float movement )
		{
			CameraMaster.Camera.HandleRotation( movement );
		}

		public void OnOcclusionEnable( OccludableSurface occludable )
		{
			occludable.EnableOcclusion( );
		}

		public void OnOcclusionDisable( OccludableSurface occludable )
		{
			occludable.DisableOcclusion( );
		}

		public void OnCursorFocusGained( Interactee ie )
		{
			PlayerMaster.Protagonist.RunFocusChain( ie );
		}

		public void OnCursorFocusLost( Interactee ie )
		{
			PlayerMaster.Protagonist.RunFocusReleaseChain( ie );
		}

		public void OnInventoryToggle( InputMaster.ButtonMap map )
		{
			UIMaster.InventoryUI.Toggle( );
		}


		public void OnProtagonistMove( InputMaster.DualAxisMap map, 
			float movementX, float movementY )
		{
			PlayerMaster.Protagonist.TranslateMoveTarget( 
				movementX * 1.6f, movementY * 1.6f, CameraMaster.UnityCamera.transform );
		}

		public void OnPropDetectionRadiusEnable( InputMaster.ButtonMap map )
		{
			UIMaster.DetectionRadius.Enable( );
		}

		public void OnPropDetectionRadiusDisable( InputMaster.ButtonMap map )
		{
			var radius = UIMaster.DetectionRadius;
			var ui = UIMaster.DetectionUI;

			radius.GatherDetections( );

			ui.AddDetections( radius.ObjectsWithin, radius.ObjectsWithinCount );
			ui.Show( );

			radius.Disable( );
		}

		public void OnStandardCancel( InputMaster.ButtonMap map )
		{
			UIMaster.CloseAllWindows( );
		}

		public void OnInputTracking( InputMaster.InputDevice device )
		{
			//RaycastMaster.CursorDevice = device;
		}

		public void OnCursorLost( )
		{
			//Debug.Log( "OnCursorLost()" );
			//RaycastMaster.Reset( );
			//InputMaster.GetKeyMap( OnCursorSelect ).ResetMode( );
			//PlayerMaster.AbortInteractionChains( );
		}

		public void OnCursorGained( )
		{
			//Debug.Log( "OnCursorGained()" );
			//RaycastMaster.SwitchTo( RaycastMaster.CursorSelectionRaycaster );
		}

		public void OnPropProximityHit( ref RaycastHit hit )
		{
			GameObject obj = hit.collider.gameObject;
			Interactee ie = obj.GetComponentInParent<Interactee>( );

			if ( ie == null )
				return ;

			PlayerMaster.Protagonist.RunFocusChain( ie );
		}

		public void OnBrowseRight( InputMaster.ButtonMap map )
		{
			if ( UIMaster.InventoryUI.IsHidden )
				return;

			UIMaster.InventoryUI.ItemGrid.MoveSlotHighlight( 1, 0 );
		}

		public void OnBrowseLeft( InputMaster.ButtonMap map )
		{
			if ( UIMaster.InventoryUI.IsHidden )
				return;

			UIMaster.InventoryUI.ItemGrid.MoveSlotHighlight( -1, 0 );
		}

		public void OnBrowseUp( InputMaster.ButtonMap map )
		{
			if ( UIMaster.InventoryUI.IsHidden )
				return;

			UIMaster.InventoryUI.ItemGrid.MoveSlotHighlight( 0, 1 );
		}

		public void OnBrowseDown( InputMaster.ButtonMap map )
		{
			if ( UIMaster.InventoryUI.IsHidden )
				return;

			UIMaster.InventoryUI.ItemGrid.MoveSlotHighlight( 0, -1 );
		}

		public void OnActionBar1( InputMaster.ButtonMap map )
		{
			PlayerMaster.Protagonist.SetCombatActionChain( 1 );
			//PlayerMaster.Protagonist.CombatActionChain = 
				//PlayerMaster.Protagonist.Skillbook["Punch"];
		}

		public void OnActionBar2( InputMaster.ButtonMap map )
		{
			PlayerMaster.Protagonist.SetCombatActionChain( 2 );
		}
		#endregion

		#region SETUP INTERNALS
		protected void SetupRaycasters( )
		{
			RaycastMaster.Setup( );

			SetupCursorSelectionRaycaster( );
			SetupHoldToMoveRaycaster( );
			SetupCameraOcclusionRaycaster( );
			SetupPropPlacementRaycaster( );
			SetupPropProximityRaycaster( );

			RaycastMaster.SwitchTo( RaycastMaster.CursorSelectionRaycaster );

#if false

			var combatCursorSelection = RaycastMaster.CombatCursorSelectionRaycaster;

			combatCursorSelection.AddPriority( LayerID.Walkable );

			combatCursorSelection.DelegateHitFound = OnCombatRaycastHit;

			combatCursorSelection.DelegateCasterAssignment = ( ref Ray ray ) =>
			{
				ray = Camera.main.ScreenPointToRay( InputMaster.MousePosition );
			};
#endif
		}

		protected void SetInputTracking( )
		{
			InputMaster.DelegateInputTracker = OnInputTracking;
			InputMaster.DelegateCursorLost = OnCursorLost;
			InputMaster.DelegateCursorGained = OnCursorGained;
		}

		private void SetupCursorSelectionRaycaster( )
		{
			var cursorSelection = RaycastMaster.CursorSelectionRaycaster;

			//cursorSelection.AddPriority( LayerID.UI );
			cursorSelection.AddPriority( LayerID.Walkable );
			cursorSelection.AddPriority( LayerID.Enemy );
			cursorSelection.AddPriority( LayerID.Item );
			cursorSelection.AddPriority( LayerID.Prop );
			cursorSelection.AddBlocker( LayerID.Default );
			cursorSelection.AddBlocker( LayerID.Occludable );
			cursorSelection.AddBlocker( LayerID.Usable );

			cursorSelection.DelegateLineTracking = ( out Vector3 start, out Vector3 end ) =>
			{
				start = InputMaster.PreviousMousePosition;
				end = InputMaster.MousePosition;
			};

			cursorSelection.DelegateCasterAssignments = ( out Ray ray, ref Vector3 screenPos ) =>
			{
				ray = CameraMaster.UnityCamera.ScreenPointToRay( screenPos );
			};

			cursorSelection.SetLayerDelegates(
				LayerID.Prop, OnCursorFocusGained, OnCursorFocusLost );
			cursorSelection.SetLayerDelegates(
				LayerID.Item, OnCursorFocusGained, OnCursorFocusLost );
			cursorSelection.SetLayerDelegates(
				LayerID.Enemy, OnCursorFocusGained, OnCursorFocusLost );
		}

		private void SetupHoldToMoveRaycaster( )
		{
			var holdToMove = RaycastMaster.HoldToMoveRaycaster;

			holdToMove.AddPriority( LayerID.Walkable );

			holdToMove.DelegateCasterAssignment = ( ref Ray ray ) =>
			{
				ray = CameraMaster.UnityCamera.ScreenPointToRay( InputMaster.MousePosition );
			};
		}

		private void SetupCameraOcclusionRaycaster( )
		{
			var cameraOcclusion = RaycastMaster.CameraOcclusionRaycaster;

			cameraOcclusion.AddPriority( LayerID.Occludable );
			cameraOcclusion.AddPriority( LayerID.OccludableHidden );
			cameraOcclusion.AddBlocker( LayerID.Protagonist );
			//cameraOcclusion.Target = PlayerMaster.Protagonist.transform;

			cameraOcclusion.DelegateOcclusionEnable = OnOcclusionEnable;
			cameraOcclusion.DelegateOcclusionDisable = OnOcclusionDisable;
			cameraOcclusion.DelegateRayAssignments = CameraOcclusionRayAssignments;
		}

		private void SetupPropPlacementRaycaster( )
		{
			var propPlacement = RaycastMaster.PropPlacementRaycaster;
		
			//propPlacement.AddPriority( LayerID.Item );
			propPlacement.AddPriority( LayerID.Prop );
			propPlacement.AddPriority( LayerID.Usable );
			propPlacement.AddPriority( LayerID.Walkable );
			//propPlacement.AddPriority( LayerID.Default );

			propPlacement.DelegateCasterAssignment = ( ref Ray ray ) =>
			{
				ray = CameraMaster.UnityCamera.ScreenPointToRay( InputMaster.MousePosition );
			};
		}

		private void SetupPropProximityRaycaster( )
		{
			var propProximity = RaycastMaster.PropProximityRaycaster;
			
			propProximity.TestRaycastCondition = InputMaster.IsUsingGamepad;

			propProximity.AddPriority( LayerID.Enemy );
			propProximity.AddPriority( LayerID.Item );
			propProximity.AddPriority( LayerID.Prop );

			propProximity.DelegateCasterAssignment = (out Ray ray) =>
			{
				Transform xform = PlayerMaster.Protagonist.transform;
				Vector3 direction = xform.forward;
				Vector3 origin = xform.position + new Vector3( 0f, 1f, 0f ) - (direction * 1.2f);
				
				ray = new Ray( origin, direction );
			};

			propProximity.SetLayerDelegates(
				LayerID.Prop, OnCursorFocusGained, OnCursorFocusLost );
			propProximity.SetLayerDelegates(
				LayerID.Item, OnCursorFocusGained, OnCursorFocusLost );

			//propProximity.DelegateHitFound = OnPropProximityHit;
			//propProximity.AddBlocker( LayerID.Default );
			//propProximity.AddBlocker( LayerID.Occludable );
			//propProximity.AddBlocker( LayerID.Usable );
		}

		private void CameraOcclusionRayAssignments( ref Ray ray, int i )
		{
			Vector3 protagonistPosition = PlayerMaster.Protagonist.transform.position
					+ new Vector3( 0f, 1.5f, 0f );

			int width = Screen.width;
			int height = Screen.height;

			float widthPortion = width / 8f;
			float heightPortion = height / 8f;

			float x = 0f;
			float y = 0f;
			switch ( i + 1 )
			{
				case 1: x = 0 + widthPortion; y = height - heightPortion; break;
				case 2: x = width / 2; y = height - heightPortion; break;
				case 3: x = width - widthPortion; y = height - heightPortion; break;
				case 4: x = 0 + widthPortion; y = height / 2; break;
				case 5: x = width / 2; y = height / 2; break;
				case 6: x = width - widthPortion; y = height / 2; break;
				case 7: x = 0 + widthPortion; y = heightPortion; break;
				case 8: x = width / 2; y = heightPortion; break;
				case 9: x = width - widthPortion; y = heightPortion; break;
			}

			Vector3 screenPos = new Vector3( x, y );
			ray = CameraMaster.UnityCamera.ScreenPointToRay( screenPos );
			ray.direction = (protagonistPosition - ray.origin).normalized;
			ray.origin -= ray.direction * 1.5f;

			//Debug.Log( "Ray #" + (i + 1) + " Origin: " + ray.origin + " Direction: " + ray.direction + " Screen: " + x + ", " + y );
		}
		#endregion

		#region OLD CODE
#if false

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



		private void SetCombatDelegates( )
		{
			CombatMaster.SetCombatBeginDelegate( OnCombatBegin );
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
#endif
		#endregion
	}


}
