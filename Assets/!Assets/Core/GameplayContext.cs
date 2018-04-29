using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using ProjectFound.Master;
using ProjectFound.Environment;
using ProjectFound.Environment.Handlers;
using ProjectFound.Environment.Props;
using ProjectFound.Environment.Characters;
using ProjectFound.Environment.Occlusion;

namespace ProjectFound.Core {


	public abstract class GameplayContext : GameContext
	{
		public GameplayContext( PlayerMaster playerMaster )
			: base( playerMaster )
		{}

		protected override void Setup( )
		{
			ContextHandler.AssignMasters( RaycastMaster, InputMaster, PlayerMaster, CameraMaster,
				UIMaster, ShaderMaster, CombatMaster, InteractionMaster );

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
			RaycastMaster.Initialize( );

			var cursorSelection = RaycastMaster.CursorSelectionRaycaster;

			cursorSelection.AddPriority( LayerID.UI );
			cursorSelection.AddPriority( LayerID.Walkable );
			cursorSelection.AddPriority( LayerID.Enemy );
			cursorSelection.AddPriority( LayerID.Item );
			cursorSelection.AddPriority( LayerID.Prop );
			cursorSelection.AddBlocker( LayerID.Default );
			cursorSelection.AddBlocker( LayerID.Roof );
			cursorSelection.AddBlocker( LayerID.Usable );

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

			var combatCursorSelection = RaycastMaster.CombatCursorSelectionRaycaster;

			combatCursorSelection.AddPriority( LayerID.Walkable );

			combatCursorSelection.DelegateHitFound = OnCombatRaycastHit;

			combatCursorSelection.DelegateCasterAssignment = (ref Ray ray) =>
			{
				ray = Camera.main.ScreenPointToRay( InputMaster.MousePosition );
			};

			var holdToMove = RaycastMaster.HoldToMoveRaycaster;

			holdToMove.AddPriority( LayerID.Walkable );

			holdToMove.DelegateCasterAssignment = (ref Ray ray) =>
			{
				ray = Camera.main.ScreenPointToRay( InputMaster.MousePosition );
			};

			var propPlacement = RaycastMaster.PropPlacementRaycaster;

			//propPlacement.AddPriority( LayerID.Item );
			propPlacement.AddPriority( LayerID.Prop );
			propPlacement.AddPriority( LayerID.Usable );
			propPlacement.AddPriority( LayerID.Walkable );
			//propPlacement.AddPriority( LayerID.Default );

			propPlacement.DelegateCasterAssignment = (ref Ray ray) =>
			{
				ray = Camera.main.ScreenPointToRay( InputMaster.MousePosition );
			};

			var cameraOcclusion = RaycastMaster.CameraOcclusionRaycaster;

			cameraOcclusion.AddPriority( LayerID.Roof );
			cameraOcclusion.Target = PlayerMaster.Player.transform;

			cameraOcclusion.DelegateOcclusionEnable = OnPlayerOccluded;
			cameraOcclusion.DelegateOcclusionDisable = OnPlayerNotOccluded;

			cameraOcclusion.DelegateCasterAssignment = (ref Ray ray) =>
			{
				ray.origin = Camera.main.transform.position;
				ray.direction = (cameraOcclusion.Target.position - ray.origin).normalized;
			};


			RaycastMaster.CurrentInteracteeRaycaster = cursorSelection;
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
			RaycastMaster.CurrentInteracteeRaycaster.IsEnabled = false;

			RaycastMaster.CurrentInteracteeRaycaster =
				RaycastMaster.CombatCursorSelectionRaycaster;

			RaycastMaster.CurrentInteracteeRaycaster.IsEnabled = true;

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

		public void OnPlayerOccluded( Occludable occludable )
		{
			PlayerMaster.OccludedFromCamera = true;

			occludable.Hide( );
			//for ( var e = hits.GetEnumerator( ); e.MoveNext( ); )
			//{
				//e.Current.gameObject.layer = (int)LayerID.RoofHidden;
			//}

			var raycaster = RaycastMaster.CameraOcclusionRaycaster;

			raycaster.RemovePriority( LayerID.Roof );
			raycaster.AddPriority( LayerID.RoofHidden );
		}

		public void OnPlayerNotOccluded( Occludable occludable )
		{
			PlayerMaster.OccludedFromCamera = false;

			//for ( var e = hits.GetEnumerator( ); e.MoveNext( ); )
		//	{
			//	e.Current.gameObject.layer = (int)LayerID.Roof;
			//}
			occludable.Show( );

			var raycaster = RaycastMaster.CameraOcclusionRaycaster;

			raycaster.RemovePriority( LayerID.RoofHidden );
			raycaster.AddPriority( LayerID.Roof );
		}

		public void OnCursorLost( )
		{
			var raycasters = RaycastMaster.Raycasters;
			int count = raycasters.Count;

			for ( int i = 0; i < count; ++i )
			{
				RaycastMaster.Raycaster raycaster = raycasters[i];

				if ( raycaster.IsEnabled == false ||
					raycaster.Mode == RaycastMaster.RaycastMode.CameraOcclusion )
				{
					continue;
				}

				switch ( raycaster.Mode )
				{
					case RaycastMaster.RaycastMode.CursorSelection:
						var csr = RaycastMaster.CursorSelectionRaycaster;
						var components = csr.GetPreviousHitComponents( );
						for ( var e = components.GetEnumerator( ); e.MoveNext( ); )
						{
							RemoveFocusDirectly( e.Current );
						}
						break;
					case RaycastMaster.RaycastMode.HoldToMove:
						InputMaster.ResetKeyMap( OnCursorSelect );
						PlayerMaster.CharacterMovement.ResetMoveTarget( );
						break;
					case RaycastMaster.RaycastMode.PropPlacement:
						var ppr = RaycastMaster.PropPlacementRaycaster;
						ppr.ClearBlacklist( );
						PlayerMaster.EndPropPlacement( );
						InputMaster.ResetKeyMap( OnCursorSelect );
						break;
				}

				raycaster.IsEnabled = false;
			}

			RaycastMaster.CurrentInteracteeRaycaster = RaycastMaster.CursorSelectionRaycaster;
		}

		public void OnCursorGained( )
		{
			RaycastMaster.CursorSelectionRaycaster.IsEnabled = true;
		}

		public virtual void OnCursorSelect2( InputMaster.KeyMap map )
		{
			var raycaster = RaycastMaster.CurrentInteracteeRaycaster;

			if ( raycaster.Mode == RaycastMaster.RaycastMode.PropPlacement )
			{

			}
		}

		public virtual void OnCursorSelect( InputMaster.KeyMap map )
		{
			var raycaster = RaycastMaster.CurrentInteracteeRaycaster;
			var player = PlayerMaster.Player;

			if ( raycaster.PreviousPriorityHitCheck.Count == 0 )
			{
				return ;
			}

			if ( UIMaster.IsOverUIElement( ) )
			{
				return ;
			}

			KeyValuePair<Interactee,RaycastHit> pair = raycaster.GetLastHit( );
			Interactee interactee = pair.Key;
			RaycastHit hit = pair.Value;
			GameObject obj = interactee.gameObject;
			LayerID layer = (LayerID)obj.layer;

			SelectionSpec spec = player.SelectionSpec;
			spec.hit = pair.Value;
			spec.gameObj = pair.Key.gameObject;
			spec.layer = (LayerID)spec.gameObj.layer;

			if ( map.Mode == InputMaster.KeyMode.OneShot )
			{
				switch ( layer )
				{
					case LayerID.Walkable:
						if ( raycaster.Mode == RaycastMaster.RaycastMode.CombatCursorSelection
							&& PlayerMaster.MovementFeedback.IsFeedbackGood == false )
						{
							return ;
						}

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
				switch ( layer )
				{
					case LayerID.Item:
					case LayerID.Prop:
						interactee.ExecuteSelectionChain( player );
						break;
				}
			}
			else if ( map.Mode == InputMaster.KeyMode.Holding )
			{
				if ( raycaster.Mode == RaycastMaster.RaycastMode.CombatCursorSelection )
				{
					return ;
				}

				if ( map.HoldingCount == 1 )
				{
					switch ( layer )
					{
						case LayerID.Walkable:
							// InteractionMaster.StartHoldToMove( interactee as WalkableSurface,
							PlayerMaster.CharacterMovement.SetMoveTarget( hit.point );
							raycaster.IsEnabled = false;
							raycaster = RaycastMaster.CurrentInteracteeRaycaster =
								RaycastMaster.HoldToMoveRaycaster;
							raycaster.IsEnabled = true;
							break;
						case LayerID.Item:
						case LayerID.Prop:
							(interactee as Prop).ExecuteDragAndDropChain( player );
							break;
					}
				}
				else
				{
					switch ( raycaster.Mode )
					{
						case RaycastMaster.RaycastMode.HoldToMove:
							PlayerMaster.CharacterMovement.SetMoveTarget( hit.point );
							break;
					}
				}
			}
			else if ( map.Mode == InputMaster.KeyMode.HoldingRelease )
			{
				if ( raycaster.Mode == RaycastMaster.RaycastMode.CombatCursorSelection )
				{
					return ;
				}

				switch ( raycaster.Mode )
				{
					case RaycastMaster.RaycastMode.HoldToMove:
						PlayerMaster.CharacterMovement.ResetMoveTarget( );
						raycaster.IsEnabled = false;
						raycaster = RaycastMaster.CurrentInteracteeRaycaster =
							RaycastMaster.CursorSelectionRaycaster;
						raycaster.IsEnabled = true;
						break;
					case RaycastMaster.RaycastMode.PropPlacement:
						PlayerMaster.PropBeingPlaced.StopDragAndDropChain( );
						break;
				}

			}
		}

		public void OnCursorFocusGained( KeyValuePair<Interactee,RaycastHit> pair )
		{
			//Prop prop = pair.Key.GetComponentInParent<Prop>( );
			AddFocus( pair );
		}

		public void OnCursorFocusLost( Interactee interactee )
		{
			//Prop prop = obj.GetComponentInParent<Prop>( );
			RemoveFocus( interactee );
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
			/*UIMaster.AddInventoryButton( item ).onClick.AddListener( () =>
			{
				// Automatically caches item reference until called! Very powerful.
				PlayerMaster.Use( item, () =>
				{
					// Layering of lamba expressions even more powerful!
					UIMaster.RemoveInventoryButton( item );
					PlayerMaster.DropItem( item );
				} );
			} );*/
		}

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

		protected void AddFocus( KeyValuePair<Interactee,RaycastHit> pair )
		{
			Interactee interactee = pair.Key;//pair.Key.GetComponentInParent<Prop>( );

			if ( interactee.IsReceptive == true && interactee.IsFocused == false )
			{
				interactee.IsFocused = true;

				//RaycastHit hit = RaycastMaster.CurrentRaycaster.PriorityHitCheck.Value;
				KeyCode key = InputMaster.GetKeyFromAction( OnCursorSelect );
				UIMaster.DisplayPrompt( interactee as Prop, key, pair.Value.point );
				ShaderMaster.ToggleSelectionOutline( interactee.gameObject );
			}
		}

		protected void RemoveFocus( Interactee interactee )
		{
			if ( interactee.IsFocused == true )
			{
				interactee.IsFocused = false;

				UIMaster.RemovePrompt( interactee as Prop );
				ShaderMaster.ToggleSelectionOutline( interactee.gameObject );
			}
		}

		protected void RemoveFocusDirectly( Interactee interactee )
		{
			if ( interactee != null )
			{
				// Nullify Raycast Hit Check so RemoveFocus isn't called twice
				RaycastMaster.CursorSelectionRaycaster.PriorityHitCheck.Remove( interactee );
				//RaycastMaster.CurrentRaycaster.PreviousPriorityHitCheck.Remove( prop.gameObject );
				RemoveFocus( interactee );
			}
		}
	}


}
