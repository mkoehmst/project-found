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
			InputMaster.MapKey( true, OnCameraAttachToggle, KeyCode.Joystick1Button9 );
			InputMaster.MapKey( true, OnCursorSelect, KeyCode.Joystick1Button0 );
			InputMaster.MapKey( true, OnInventoryToggle, KeyCode.Joystick1Button7 );
			InputMaster.MapKey( true, OnDetectionRadiusToggle, KeyCode.Joystick1Button2 );
			InputMaster.MapKey( true, OnPropCollectionToggle, KeyCode.Joystick1Button1 );
		}

		public virtual void OnCursorSelect( InputMaster.KeyMap map )
		{
			Debug.Log( "OnCursorSelect: " + map.Mode );

			Device = map.Device;

			if ( RaycastMaster.PriorityHitCheck == null )
				return ;

			RaycastHit hit = RaycastMaster.PriorityHitCheck.Value;
			GameObject obj = hit.collider.gameObject;
			LayerID layer = (LayerID)obj.layer;

			if ( map.Mode == InputMaster.KeyMode.OneShot )
			{
				if ( layer == LayerID.Walkable )
				{
					map.OpenHoldingWindow( 0.35f );
					PlayerMaster.CharacterMovement.SetMoveTarget( hit.point );
				}
				else if ( layer == LayerID.Item )
				{
					Item item = obj.GetComponent<Item>( );

					if ( PlayerMaster.PickUp( item ) == true )
					{
						AddToInventory( item );
					}
				}
				else if ( layer == LayerID.Prop )
				{
					Prop prop = obj.GetComponent<Prop>( );

					PlayerMaster.Use( prop as Interactee );
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

			Prop prop = obj.GetComponent<Prop>( );
			KeyCode key = InputMaster.ActionToKey[OnCursorSelect];
			UIMaster.DisplayPrompt( prop, key );
			ShaderMaster.ToggleSelectionOutline( obj );
		}

		public void OnCursorFocusLost( GameObject obj )
		{
			Debug.Log( "Cursor Lost" );

			Prop prop = obj.GetComponent<Prop>( );
			UIMaster.RemovePrompt( prop );
			ShaderMaster.ToggleSelectionOutline( obj );
		}

		public virtual void OnCameraMoveHorizontal( InputMaster.AxisMap map, float movement )
		{
			Device = map.Device;

			CameraMaster.FixedTiltZoomableCamera.HandleMovement( 0f, movement );
		}

		public virtual void OnCameraMoveVertical( InputMaster.AxisMap map, float movement )
		{
			Device = map.Device;

			CameraMaster.FixedTiltZoomableCamera.HandleMovement( movement, 0f );
		}

		public virtual void OnCameraRotation( InputMaster.AxisMap map, float movement )
		{
			Device = map.Device;

			CameraMaster.FixedTiltZoomableCamera.HandleRotation( movement );
		}

		public virtual void OnCameraZoom( InputMaster.AxisMap map, float movement )
		{
			Device = map.Device;

			CameraMaster.FixedTiltZoomableCamera.HandleZoom( movement );
		}

		public virtual void OnPlayerMovement( InputMaster.AxiiMap map, float[] movements )
		{
			Device = map.Device;

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
				map.OpenHoldingWindow( 0f );
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
				map.OpenHoldingWindow( 0.35f );
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
				PlayerMaster.Use( item as Interactee );
				UIMaster.RemoveInventoryButton( item );
			} );
		}

		protected void AddItemToPropCollection( Item item )
		{
			UIMaster.AddPropCollectionButton( item as Prop ).onClick.AddListener( () =>
			{
				if ( PlayerMaster.PickUp( item ) == true )
				{
					AddToInventory( item );
					UIMaster.RemovePropCollectionProp( item as Prop );
				}
			} );
		}

		protected void AddPropToPropCollection( Prop prop )
		{
			UIMaster.AddPropCollectionButton( prop ).onClick.AddListener( () =>
			{
				PlayerMaster.Use( prop as Interactee );
				UIMaster.RemovePropCollectionProp( prop );
			} );
		}
	}


}
