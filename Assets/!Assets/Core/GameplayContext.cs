using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using ProjectFound.Master;
using ProjectFound.Environment;
using ProjectFound.Environment.Items;

namespace ProjectFound.Core {


	public abstract class GameplayContext : GameContext
	{
		public GameplayContext( PlayerMaster playerMaster )
			: base( playerMaster )
		{}

		protected override void SetRaycastPriority( )
		{
			RaycastMaster.AddPriority( LayerID.UI ); // UI
			RaycastMaster.AddPriority( LayerID.Enemy ); // Enemy
			RaycastMaster.AddPriority( LayerID.Item ); // Item
			RaycastMaster.AddPriority( LayerID.Prop ); // Prop
			RaycastMaster.AddPriority( LayerID.Walkable ); // Walkable

			RaycastMaster.SetLayerDelegates(
				LayerID.Item, OnCursorFocusGained, OnCursorFocusLost );

			RaycastMaster.SetLayerDelegates(
				LayerID.Prop, OnCursorFocusGained, OnCursorFocusLost );
		}

		protected override void SetInputTracker( )
		{
			InputMaster.DelegateInputTracker = OnInputTracking;
		}

		protected override void LoadMouseAndKeyboardMappings( )
		{
			InputMaster.CurrentDeviceMapped = InputMaster.InputDevice.MouseAndKeyboard;

			InputMaster.MapKey( true, OnCursorSelect, KeyCode.Mouse0 );
			InputMaster.MapAxis( true, OnCameraMoveHorizontal, "KeyboardCameraHorizontal" );
			InputMaster.MapAxis( true, OnCameraMoveVertical, "KeyboardCameraVertical" );
			InputMaster.MapAxis( true, OnCameraRotation, "KeyboardCameraRotation" );
			InputMaster.MapAxis( true, OnCameraZoom, "Mouse ScrollWheel" );
			InputMaster.MapKey( true, OnCameraAttach, KeyCode.Home );
			InputMaster.MapKey( true, OnCameraRotationMod, KeyCode.Mouse2 );
			InputMaster.MapKey( true, OnInventoryToggle, KeyCode.I );
			InputMaster.MapKey( true, OnPropCollectionToggle, KeyCode.P );
		}

		protected override void LoadGamepadMappings( )
		{
			InputMaster.CurrentDeviceMapped = InputMaster.InputDevice.Gamepad;

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
			Device = device;
		}

		public virtual void OnCursorSelect( InputMaster.KeyMap map )
		{
			Debug.Log( "OnCursorSelect: " + map.Mode );

			if ( RaycastMaster.PriorityHitCheck == null )
				return ;

			RaycastHit hit = RaycastMaster.PriorityHitCheck.Value;
			GameObject obj = hit.collider.gameObject;
			LayerID layer = (LayerID)obj.layer;

			if ( map.Mode == InputMaster.KeyMode.OneShot )
			{
				switch ( layer )
				{
					case LayerID.Walkable:
						map.HoldingWindow = 0.35f;
						PlayerMaster.CharacterMovement.SetMoveTarget( hit.point );
						break;

					case LayerID.Item:
						Item item = obj.GetComponent<Item>( );
						PlayerMaster.PickUp( item, () =>
						{
							RemoveFocusDirectly( item as Prop );
							AddToInventory( item );
						} );
						break;

					case LayerID.Prop:
						Prop prop = obj.GetComponent<Prop>( );
						PlayerMaster.Activate( prop, () =>
						{
							RemoveFocusDirectly( prop );
						} );
						break;
				}
			}
			else if ( map.Mode == InputMaster.KeyMode.Holding )
			{
				if ( layer == LayerID.Walkable )
				{
					PlayerMaster.CharacterMovement.SetMoveTarget( hit.point );
				}
			}
			else if ( map.Mode == InputMaster.KeyMode.HoldingRelease )
			{
				PlayerMaster.CharacterMovement.ResetMoveTarget( );
			}
		}

		public void OnCursorFocusGained( GameObject obj )
		{
			Debug.Log( "Cursor Gained" );

			Prop prop = obj.GetComponentInParent<Prop>( );
			AddFocus( prop );
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

					InputMaster.AxiiMaps[moveAxii].Disable( );
					InputMaster.AxisMaps[cameraH].Enable( );
					InputMaster.AxisMaps[cameraV].Enable( );
				}
				else
				{
					CameraMaster.FixedTiltZoomableCamera.HandleAttachment( );

					InputMaster.AxiiMaps[moveAxii].Enable( );
					InputMaster.AxisMaps[cameraH].Disable( );
					InputMaster.AxisMaps[cameraV].Disable( );
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
			// Lambda statement delegates...love this
			UIMaster.AddInventoryButton( item ).onClick.AddListener( () =>
			{
				// Automatically caches item reference until called! Very powerful.
				PlayerMaster.Use( item, () =>
				{
					// Layering of lamba expressions even more powerful!
					UIMaster.RemoveInventoryButton( item );
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

		protected void AddFocus( Prop prop )
		{
			if ( prop.IsReceptive == true && prop.IsFocused == false )
			{
				prop.IsFocused = true;

				KeyCode key = InputMaster.ActionToKey[OnCursorSelect];
				UIMaster.DisplayPrompt( prop, key );
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
			// Nullify Raycast Hit Check so RemoveFocus isn't called twice
			RaycastMaster.PriorityHitCheck = null;
			RemoveFocus( prop );
		}
	}


}
