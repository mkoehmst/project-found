using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using ProjectFound.Master;
using ProjectFound.Environment.Items;

namespace ProjectFound.Core {


	public abstract class GameplayContext : GameContext
	{
		public GameplayContext( PlayerMaster playerMaster )
			: base( playerMaster )
		{}

		protected override void SetRaycastPriority( )
		{
			RaycastMaster.AddPriority( 5 ); // UI
			RaycastMaster.AddPriority( 9 ); // Enemy
			RaycastMaster.AddPriority( 10 ); // Item
			RaycastMaster.AddPriority( 8 ); // Walkable

			RaycastMaster.SetLayerDelegates( 10, OnCursorFocusGained, OnCursorFocusLost );
		}

		protected override void LoadMouseAndKeyboardMappings( )
		{
			InputMaster.MapKey( true, OnCursorSelect, KeyCode.Mouse0 );
			InputMaster.MapAxis( true, OnCameraMoveHorizontal, "KeyboardCameraHorizontal" );
			InputMaster.MapAxis( true, OnCameraMoveVertical, "KeyboardCameraVertical" );
			InputMaster.MapAxis( true, OnCameraRotation, "KeyboardCameraRotation" );
			InputMaster.MapAxis( true, OnCameraZoom, "Mouse ScrollWheel" );
			InputMaster.MapKey( true, OnCameraAttach, KeyCode.Home );
			InputMaster.MapKey( true, OnCameraRotationMod, KeyCode.Mouse2 );
			InputMaster.MapKey( true, OnInventoryToggle, KeyCode.I );
		}

		protected override void LoadGamepadMappings( )
		{
			InputMaster.MapAxis( false, OnCameraMoveHorizontal, "ControllerCameraHorizontal" );
			InputMaster.MapAxis( false, OnCameraMoveVertical, "ControllerCameraVertical" );
			InputMaster.MapAxis( true, OnCameraRotation, "ControllerCameraRotation" );
			InputMaster.MapAxis( true, OnCameraZoom, "ControllerCameraZoom" );
			InputMaster.MapAxii( true, OnPlayerMovement,
				"ControllerMovementHorizontal", "ControllerMovementVertical" );
			InputMaster.MapKey( true, OnCameraAttachToggle, KeyCode.Joystick1Button9 );
		}

		public virtual void OnCursorSelect( InputMaster.KeyMap map )
		{
			Debug.Log( "OnCursorSelect: " + map.Mode );

			if ( RaycastMaster.IsOverUIElement( ) )
				return ;

			if ( RaycastMaster.DidFindPriorityHit( ) == false )
				return ;

			RaycastHit hit = RaycastMaster.PriorityHitCheck.Value;
			GameObject obj = hit.collider.gameObject;
			int layer = obj.layer;

			if ( map.Mode == InputMaster.KeyMode.OneShot )
			{
				if ( layer == 8 )
				{
					map.OpenHoldingWindow( 0.35f );
					PlayerMaster.CharacterMovement.SetMoveTarget( hit.point );
				}
				else if ( layer == 10 )
				{
					Item item = obj.GetComponent<Item>( );

					if ( PlayerMaster.AttemptItemPickup( item ) )
					{
						// Lambda expression delegates...love this
						UIMaster.AddInventoryButton( item ).onClick.AddListener( () =>
						{
							// Automatically caches prop reference until called! Very powerful.
							PlayerMaster.UseInventoryItem( item );
							UIMaster.RemoveInventoryButton( item );
						} );
					}
				}
			}
			else if ( map.Mode == InputMaster.KeyMode.Holding )
			{
				if ( RaycastMaster.IsOverUIElement( ) )
					return;

				if ( layer == 8 )
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
			if ( obj.layer == 10 )
			{
				Item item = obj.GetComponent<Item>( );
				KeyCode key = InputMaster.ActionToKey[OnCursorSelect];
				UIMaster.DisplayPrompt( item, key );
			}
		}

		public void OnCursorFocusLost( GameObject obj )
		{
			if ( obj.layer == 10 )
			{
				Item item = obj.GetComponent<Item>( );
				UIMaster.RemovePrompt( item );
			}
		}

		public virtual void OnCameraMoveHorizontal( float movement )
		{
			CameraMaster.FixedTiltZoomableCamera.HandleMovement( 0f, movement );
		}

		public virtual void OnCameraMoveVertical( float movement )
		{
			CameraMaster.FixedTiltZoomableCamera.HandleMovement( movement, 0f );
		}

		public virtual void OnCameraRotation( float movement )
		{
			CameraMaster.FixedTiltZoomableCamera.HandleRotation( movement );
		}

		public virtual void OnCameraZoom( float movement )
		{
			CameraMaster.FixedTiltZoomableCamera.HandleZoom( movement );
		}

		public virtual void OnPlayerMovement( float[] movements )
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
				map.OpenHoldingWindow( 0f );
			}
			else if ( map.Mode == InputMaster.KeyMode.Holding )
			{
				float movement = InputMaster.CheckAxis( "MouseCameraRotation" );

				if ( movement != 0f )
				{
					OnCameraRotation( movement );
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
	}


}
