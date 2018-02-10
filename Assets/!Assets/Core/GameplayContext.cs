using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using ProjectFound.Master;
using ProjectFound.Environment.Props;
using ProjectFound.Environment.Characters;

namespace ProjectFound.Core {


	public abstract class GameplayContext : GameContext
	{
		public GameplayContext( PlayerMaster playerMaster )
			: base( playerMaster )
		{}

		protected override void Setup( )
		{
			ContextHandler.AssignMasters( RaycastMaster, InputMaster, PlayerMaster, CameraMaster, UIMaster, ShaderMaster, CombatMaster );

			SetupSkillBook( );
			SetupConductBar( );
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

			button.onClick.AddListener( () =>
			{
				PlayerMaster.Player.DelegateCombatHandler = skill.Handle;
			} );
		}

		protected override void SetupRaycasters( )
		{
			var cursorSelection =
				RaycastMaster.AddLineRaycaster(
					RaycastMaster.RaycastMode.CursorSelection, 30f, 3 );

			cursorSelection.AddPriority( LayerID.UI );
			cursorSelection.AddPriority( LayerID.Enemy );
			cursorSelection.AddPriority( LayerID.Item );
			cursorSelection.AddPriority( LayerID.Prop );
			cursorSelection.AddPriority( LayerID.Walkable );
			//cursorSelection.AddPriority( LayerID.Default ); // Temporary debug

			cursorSelection.DelegateLineTracking = (ref Vector3 start, ref Vector3 end) =>
			{
				start = InputMaster.PreviousMousePosition;
				end = InputMaster.MousePosition;
			};

			cursorSelection.DelegateCasterAssignments = (ref Ray ray, ref Vector3 screenPos) =>
			{
				ray = Camera.main.ScreenPointToRay( screenPos );
			};

			cursorSelection.SetLayerDelegates
				( LayerID.Prop, OnCursorFocusGained, OnCursorFocusLost );

			cursorSelection.SetLayerDelegates
				( LayerID.Item, OnCursorFocusGained, OnCursorFocusLost );

			var combatCursorSelection =
				RaycastMaster.AddPointRaycaster(
					RaycastMaster.RaycastMode.CombatCursorSelection, 30f, false );

			combatCursorSelection.AddPriority( LayerID.Walkable );

			combatCursorSelection.DelegateHitFound = OnCombatRaycastHit;

			combatCursorSelection.DelegateCasterAssignment = (ref Ray ray) =>
			{
				ray = Camera.main.ScreenPointToRay( InputMaster.MousePosition );
			};

			var holdToMove =
				RaycastMaster.AddPointRaycaster(
					RaycastMaster.RaycastMode.HoldToMove, 30f, false );

			holdToMove.AddPriority( LayerID.Walkable );

			holdToMove.DelegateCasterAssignment = (ref Ray ray) =>
			{
				ray = Camera.main.ScreenPointToRay( InputMaster.MousePosition );
			};

			var propPlacement =
				RaycastMaster.AddPointRaycaster(
					RaycastMaster.RaycastMode.PropPlacement, 30f, false );

			propPlacement.AddPriority( LayerID.Item );
			propPlacement.AddPriority( LayerID.Prop );
			propPlacement.AddPriority( LayerID.Walkable );
			propPlacement.AddPriority( LayerID.Default );

			propPlacement.DelegateCasterAssignment = (ref Ray ray) =>
			{
				ray = Camera.main.ScreenPointToRay( InputMaster.MousePosition );
			};

			var cameraOcclusion =
				RaycastMaster.AddOcclusionRaycaster(
					RaycastMaster.RaycastMode.CameraOcclusion, 70f, PlayerMaster.Player.transform );

			cameraOcclusion.AddPriority( LayerID.Roof );

			cameraOcclusion.DelegateHitsFound = OnRaycastHits;
			cameraOcclusion.DelegateHitsNotFound = OnRaycastNoHits;

			cameraOcclusion.DelegateCasterAssignment = (ref Ray ray) =>
			{
				ray.origin = Camera.main.transform.position;
				ray.direction = (cameraOcclusion.Target.position - ray.origin).normalized;
			};


			RaycastMaster.CurrentRaycaster = cursorSelection;
		}

		protected override void SetInputTracker( )
		{
			InputMaster.DelegateInputTracker = OnInputTracking;
			InputMaster.DelegateCursorLost = OnCursorLost;
			InputMaster.DelegateCursorGained = OnCursorGained;
		}

		protected override void SetCombatDelegates( )
		{
			CombatMaster.SetCombatBeginDelegate( OnCombatBegin );
		}

		protected override void LoadMouseAndKeyboardMappings( )
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

		protected override void LoadGamepadMappings( )
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

		public void OnInputTracking( InputMaster.InputDevice device )
		{
			RaycastMaster.CursorDevice = device;
		}

		public void OnCombatBegin( List<Combatant> combatants )
		{
			RaycastMaster.CurrentRaycaster.IsEnabled = false;

			RaycastMaster.CurrentRaycaster =
				RaycastMaster.Raycasters[RaycastMaster.RaycastMode.CombatCursorSelection];

			RaycastMaster.CurrentRaycaster.IsEnabled = true;

			//InputMaster.DisableMap( InputMaster.GetKeyFromAction( )

			//InputMaster.KeyMaps[InputMaster.GetKeyFromAction( OnCombatA)]
		}

		public void OnCombatRaycastHit( ref RaycastHit hit )
		{
			if ( PlayerMaster.CanMoveTo( hit.point ) == false )
			{
				PlayerMaster.CombatMovementFeedback( hit.point, false );
				return ;
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

		public void OnRaycastHits( ICollection<GameObject> hits )
		{
			PlayerMaster.OccludedFromCamera = true;

			for ( var e = hits.GetEnumerator( ); e.MoveNext( ); )
			{
				e.Current.layer = (int)LayerID.RoofHidden;
			}

			RaycastMaster.Raycaster raycaster =
				RaycastMaster.Raycasters[RaycastMaster.RaycastMode.CameraOcclusion];

			raycaster.RemovePriority( LayerID.Roof );
			raycaster.AddPriority( LayerID.RoofHidden );
		}

		public void OnRaycastNoHits( ICollection<GameObject> hits )
		{
			PlayerMaster.OccludedFromCamera = false;

			for ( var e = hits.GetEnumerator( ); e.MoveNext( ); )
			{
				e.Current.layer = (int)LayerID.Roof;
			}

			RaycastMaster.Raycaster raycaster =
				RaycastMaster.Raycasters[RaycastMaster.RaycastMode.CameraOcclusion];

			raycaster.RemovePriority( LayerID.RoofHidden );
			raycaster.AddPriority( LayerID.Roof );
		}

		public void OnCursorLost( )
		{
			RaycastMaster.Raycaster raycaster = RaycastMaster.CurrentRaycaster;

			switch ( raycaster.Mode )
			{
				case RaycastMaster.RaycastMode.CursorSelection:
					var objs = RaycastMaster.CurrentRaycaster.GetPreviousHitObjects( );
					for ( var e = objs.GetEnumerator( ); e.MoveNext( ); )
					{
						RemoveFocusDirectly( e.Current.GetComponent<Prop>( ) );
					}
					break;
				case RaycastMaster.RaycastMode.HoldToMove:
					InputMaster.ResetKeyMap( OnCursorSelect );
					PlayerMaster.CharacterMovement.ResetMoveTarget( );
					break;
				case RaycastMaster.RaycastMode.PropPlacement:
					raycaster.ClearBlacklist( );
					PlayerMaster.EndPropPlacement( );
					InputMaster.ResetKeyMap( OnCursorSelect );
					break;
			}

			raycaster.IsEnabled = false;

			RaycastMaster.CurrentRaycaster =
				RaycastMaster.Raycasters[RaycastMaster.RaycastMode.CursorSelection];

			RaycastMaster.CurrentRaycaster.IsEnabled = false;
		}

		public void OnCursorGained( )
		{
			RaycastMaster.CurrentRaycaster.IsEnabled = true;
		}

		public virtual void OnCursorSelect( InputMaster.KeyMap map )
		{
			RaycastMaster.Raycaster raycaster = RaycastMaster.CurrentRaycaster;

			if ( raycaster.PreviousPriorityHitCheck.Count == 0 )
			{
				return ;
			}

			if ( UIMaster.IsOverUIElement( ) )
			{
				return ;
			}

			KeyValuePair<GameObject,RaycastHit> pair = raycaster.GetLastHit( );
			RaycastHit hit = pair.Value;
			GameObject obj = pair.Key;
			LayerID layer = (LayerID)obj.layer;

			if ( map.Mode == InputMaster.KeyMode.OneShot )
			{
				Debug.Log( "Cursor OneShot: " + map.Key );

				switch ( layer )
				{
					case LayerID.Walkable:
						map.HoldingWindow = 0.35f;
						PlayerMaster.CharacterMovement.SetMoveTarget( hit.point );
						break;
					case LayerID.Item:
					case LayerID.Prop:
						map.HoldingWindow = 0.25f;
						break;
				}
			}
			else if ( map.Mode == InputMaster.KeyMode.OneShotRelease )
			{
				Debug.Log( "Cursor OneShotRelease: " + map.Key );

				switch ( layer )
				{
					case LayerID.Item:
						obj.GetComponent<Item>( ).PickUp( );
						//Item item = obj.GetComponent<Item>( );
						//RemoveFocusDirectly( item as Prop );
						//item.PickUp( );
						//PlayerMaster.PickUp( item, () =>
						//{
						//	RemoveFocusDirectly( item as Prop );
						//	AddToInventory( item );
						//} );
						break;
					case LayerID.Prop:
						obj.GetComponentInParent<Prop>( ).Activate( );
						//Prop prop = obj.GetComponent<Prop>( );
						//PlayerMaster.Activate( prop, () =>
						//{
						//	RemoveFocusDirectly( prop );
						//} );
						break;
				}
			}
			else if ( map.Mode == InputMaster.KeyMode.Holding )
			{
				Debug.Log( "Holding: " + map.Key );

				if ( map.HoldingCount == 1 )
				{
					switch ( layer )
					{
						case LayerID.Walkable:
							// PlayerMaster.CharacterMovement.StartHoldToMove( hit.point );
							PlayerMaster.CharacterMovement.SetMoveTarget( hit.point );
							raycaster.IsEnabled = false;
							raycaster = RaycastMaster.CurrentRaycaster =
								RaycastMaster.Raycasters[RaycastMaster.RaycastMode.HoldToMove];
							raycaster.IsEnabled = true;
							break;
						case LayerID.Item:
						case LayerID.Prop:
							obj.GetComponent<Prop>( ).StartDragAndDrop( ref hit );
							/*Prop prop = obj.GetComponent<Prop>( );
							RemoveFocus( prop );
							raycaster.IsEnabled = false;
							raycaster = RaycastMaster.CurrentRaycaster =
								RaycastMaster.Raycasters[RaycastMaster.RaycastMode.PropPlacement];
							raycaster.IsEnabled = true;
							raycaster.AddBlacklistee( obj );
							PlayerMaster.StartPropPlacement( prop, obj, ref hit );*/
							break;
					}
				}
				else
				{
					switch ( raycaster.Mode )
					{
						case RaycastMaster.RaycastMode.HoldToMove:
							// PlayerMaster.CharacterMovement.SetMoveTarget( hit.point );
							PlayerMaster.CharacterMovement.SetMoveTarget( hit.point );
							break;
						case RaycastMaster.RaycastMode.PropPlacement:
							PlayerMaster.PropPlacement( ref hit );
							break;
					}
				}
			}
			else if ( map.Mode == InputMaster.KeyMode.HoldingRelease )
			{
				Debug.Log( "HoldingRelease: " + map.Key );

				switch ( raycaster.Mode )
				{
					case RaycastMaster.RaycastMode.HoldToMove:
						PlayerMaster.CharacterMovement.ResetMoveTarget( );
						break;
					case RaycastMaster.RaycastMode.PropPlacement:
						raycaster.RemoveBlacklistee( PlayerMaster.PropBeingPlaced.gameObject );
						PlayerMaster.EndPropPlacement( ref hit );
						break;
				}

				raycaster.IsEnabled = false;

				raycaster = RaycastMaster.CurrentRaycaster =
					RaycastMaster.Raycasters[RaycastMaster.RaycastMode.CursorSelection];

				raycaster.IsEnabled = true;
			}
		}

		public void OnCursorFocusGained( KeyValuePair<GameObject,RaycastHit> pair )
		{
			Debug.Log( "Cursor Gained" );

			//Prop prop = pair.Key.GetComponentInParent<Prop>( );
			AddFocus( pair );
		}

		public void OnCursorFocusLost( GameObject obj )
		{
			Debug.Log( "Cursor Lost" );

			Prop prop = obj.GetComponentInParent<Prop>( );
			RemoveFocus( prop );
		}

		public virtual void OnCameraMoveHorizontal( InputMaster.AxisMap map, float movement )
		{
			CameraMaster.FixedTiltZoomableCamera.HandleMovement( 0f, movement );
		}

		public virtual void OnCameraMoveVertical( InputMaster.AxisMap map, float movement )
		{
			CameraMaster.FixedTiltZoomableCamera.HandleMovement( movement, 0f );
		}

		public virtual void OnCameraRotation( InputMaster.AxisMap map, float movement )
		{
			CameraMaster.FixedTiltZoomableCamera.HandleRotation( movement );
		}

		public virtual void OnCameraZoom( InputMaster.AxisMap map, float movement )
		{
			CameraMaster.FixedTiltZoomableCamera.HandleZoom( movement );
		}

		public virtual void OnPlayerMovement( InputMaster.AxiiMap map, float[] movements )
		{
			if ( movements.Length != 2 )
				return ;

			PlayerMaster.CharacterMovement.TranslateMoveTarget( movements[0], movements[1] );
		}

		public virtual void OnCameraAttachToggle( InputMaster.KeyMap map )
		{
			if ( map.Mode == InputMaster.KeyMode.OneShot )
			{
				string[] moveAxii = { "ControllerMovementHorizontal", "ControllerMovementVertical" };
				string cameraH = "ControllerCameraHorizontal";
				string cameraV = "ControllerCameraVertical";

				if ( CameraMaster.FixedTiltZoomableCamera.IsAttached )
				{
					CameraMaster.FixedTiltZoomableCamera.HandleDetachment( );

					InputMaster.DisableMap( moveAxii );
					InputMaster.EnableMap( cameraH );
					InputMaster.EnableMap( cameraV );
				}
				else
				{
					CameraMaster.FixedTiltZoomableCamera.HandleAttachment( );

					InputMaster.EnableMap( moveAxii );
					InputMaster.DisableMap( cameraH );
					InputMaster.DisableMap( cameraV );
				}
			}
		}

		public virtual void OnCameraAttach( InputMaster.KeyMap map )
		{
			if ( map.Mode == InputMaster.KeyMode.OneShot )
			{
				CameraMaster.FixedTiltZoomableCamera.HandleAttachment( );
			}
		}

		public virtual void OnCameraRotationMod( InputMaster.KeyMap map )
		{
			if ( map.Mode == InputMaster.KeyMode.OneShot )
			{
				map.HoldingWindow = 0f;
			}
			else if ( map.Mode == InputMaster.KeyMode.Holding )
			{
				float movement = InputMaster.CheckAxis( "MouseCameraRotation" );

				if ( movement != 0f )
				{
					InputMaster.AxisHasMoved( "KeyboardCameraRotation", movement );
					//OnCameraRotation( movement );
				}
			}
		}

		public virtual void OnInventoryToggle( InputMaster.KeyMap map )
		{
			if ( map.Mode == InputMaster.KeyMode.OneShot )
			{
				UIMaster.ToggleInventoryWindow( );
			}
		}

		public virtual void OnPropCollectionToggle( InputMaster.KeyMap map )
		{
			if ( map.Mode == InputMaster.KeyMode.OneShot )
			{
				UIMaster.TogglePropCollectionWindow( );
			}
		}

		public virtual void OnDetectionRadiusToggle( InputMaster.KeyMap map )
		{
			if ( map.Mode == InputMaster.KeyMode.OneShot )
			{
				map.HoldingWindow = 0.35f;
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

		protected void AddToInventory( Item item )
		{
			PlayerMaster.AddInventoryItem( item );

			// Lambda statement delegates...love this
			UIMaster.AddInventoryButton( item ).onClick.AddListener( () =>
			{
				// Automatically caches item reference until called! Very powerful.
				PlayerMaster.Use( item, () =>
				{
					// Layering of lamba expressions even more powerful!
					UIMaster.RemoveInventoryButton( item );
					PlayerMaster.DropItem( item );
				} );
			} );
		}

		protected void AddItemToPropCollection( Item item )
		{
			UIMaster.AddPropCollectionButton( item as Prop ).onClick.AddListener( () =>
			{
				PlayerMaster.PickUp( item, () =>
				{
					AddToInventory( item );
					UIMaster.RemovePropCollectionProp( item as Prop );
				} );
			} );
		}

		protected void AddPropToPropCollection( Prop prop )
		{
			UIMaster.AddPropCollectionButton( prop ).onClick.AddListener( () =>
			{
				PlayerMaster.Activate( prop, () =>
				{
					UIMaster.RemovePropCollectionProp( prop );
				} );
			} );
		}

		protected void AddFocus( KeyValuePair<GameObject,RaycastHit> pair )
		{
			Prop prop = pair.Key.GetComponentInParent<Prop>( );

			if ( prop.IsReceptive == true && prop.IsFocused == false )
			{
				prop.IsFocused = true;

				//RaycastHit hit = RaycastMaster.CurrentRaycaster.PriorityHitCheck.Value;
				KeyCode key = InputMaster.GetKeyFromAction( OnCursorSelect );
				UIMaster.DisplayPrompt( prop, key, pair.Value.point );
				ShaderMaster.ToggleSelectionOutline( prop.gameObject );
			}
		}

		protected void RemoveFocus( Prop prop )
		{
			if ( prop.IsFocused == true )
			{
				prop.IsFocused = false;

				UIMaster.RemovePrompt( prop );
				ShaderMaster.ToggleSelectionOutline( prop.gameObject );
			}
		}

		protected void RemoveFocusDirectly( Prop prop )
		{
			if ( prop != null )
			{
				// Nullify Raycast Hit Check so RemoveFocus isn't called twice
				RaycastMaster.CurrentRaycaster.PriorityHitCheck.Remove( prop.gameObject );
				//RaycastMaster.CurrentRaycaster.PreviousPriorityHitCheck.Remove( prop.gameObject );
				RemoveFocus( prop );
			}
		}
	}


}
