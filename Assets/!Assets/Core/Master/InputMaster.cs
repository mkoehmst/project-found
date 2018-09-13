namespace ProjectFound.Core.Master
{


	using System;
	using System.Collections.Generic;
	
	using UnityEngine;
	using Rewired; 

	public class InputMaster
	{
		#region DELEGATE TYPES
		public delegate void ButtonAction( ButtonMap map );
		public delegate void AxisAction( AxisMap map, float movement );
		public delegate void DualAxisAction( DualAxisMap map, float movementX, float movementY );
		#endregion

		#region ENUM TYPES
		public enum KeyMode
		{
			Undefined,
			OneShot,
			OneShotRelease,
			Holding,
			HoldingRelease,
			Window,
			WindowRelease
		}
		
		public enum InputDevice
		{
			Undefined,
			MouseAndKeyboard,
			Gamepad
		}
		#endregion

		#region CLASS TYPES
		public class ActionMap
		{
			public string ActionName { get; protected set; }
			public int ActionID { get; protected set; }

			public ActionMap( string actionName )
			{
				ActionName = actionName;
				ActionID = ReInput.mapping.GetActionId( actionName );
			}
		}

		public class ButtonMap : ActionMap
		{
			public KeyMode Mode;
			public ButtonAction DelegateOneShot { get; private set; }
			public ButtonAction DelegateOneShotRelease { get; private set; }
			public ButtonAction DelegateWindow { get; private set; }
			public ButtonAction DelegateWindowRelease { get; private set; }
			public ButtonAction DelegateHolding { get; private set; }
			public ButtonAction DelegateHoldingRelease { get; private set; }

			public Func<bool> OneShotCondition { get; private set; }
			public Func<bool> OneShotReleaseCondition { get; private set; }
			public Func<bool> WindowCondition { get; private set; }
			public Func<bool> WindowReleaseCondition { get; private set; }
			public Func<bool> HoldingCondition { get; private set; }
			public Func<bool> HoldingReleaseCondition { get; private set; }

			public ButtonMap( string actionName, 
				ButtonAction delegateOneShot, ButtonAction delegateOneShotRelease,
				ButtonAction delegateWindow,  ButtonAction delegateWindowRelease, 
				ButtonAction delegateHolding, ButtonAction delegateHoldingRelease,
				Func<bool> oneShotCondition, Func<bool> oneShotReleaseCondition,
				Func<bool> windowCondition, Func<bool> windowReleaseCondition,
				Func<bool> holdingCondition, Func<bool> holdingReleaseCondition )
				: base( actionName )
			{
				DelegateOneShot = delegateOneShot; 
				DelegateOneShotRelease = delegateOneShotRelease;
				DelegateWindow = delegateWindow;
				DelegateWindowRelease = delegateWindowRelease;
				DelegateHolding = delegateHolding;
				DelegateHoldingRelease = delegateHoldingRelease;

				OneShotCondition = oneShotCondition;
				OneShotReleaseCondition = oneShotReleaseCondition;
				WindowCondition = windowCondition;
				WindowReleaseCondition = windowReleaseCondition;
				HoldingCondition = holdingCondition;
				HoldingReleaseCondition = holdingReleaseCondition;
			}

			public bool TestOneShotCondition( )
			{
				return OneShotCondition == null || OneShotCondition( );
			}
			
			public bool TestOneShotReleaseCondition( )
			{
				return OneShotReleaseCondition == null || OneShotReleaseCondition( );
			}

			public bool TestWindowCondition( )
			{
				return WindowCondition == null || WindowCondition( );
			}

			public bool TestWindowReleaseCondition( )
			{
				return WindowReleaseCondition == null || WindowReleaseCondition( );
			}

			public bool TestHoldingCondition( )
			{
				return HoldingCondition == null || HoldingCondition( );
			}

			public bool TestHoldingReleaseCondition( )
			{
				return HoldingReleaseCondition == null || HoldingReleaseCondition( );
			}
		}

		public class AxisMap : ActionMap
		{
			public bool IsVerbose { get; private set; }
			public AxisAction DelegateAxis { get; private set; }
			public Func<bool> AxisCondition { get; private set; }

			public AxisMap( string actionName,
				AxisAction delegateAxis,
				bool isVerbose,
				Func<bool> axisCondition )
				: base( actionName )
			{ 
				DelegateAxis = delegateAxis;
				IsVerbose = isVerbose;
				AxisCondition = axisCondition;
			}
		}

		public class DualAxisMap
		{
			public bool IsVerbose { get; private set; }
			public ActionMap ActionMapX { get; private set; }
			public ActionMap ActionMapY { get; private set; }

			public DualAxisAction DelegateDualAxis { get; private set; }
			public Func<bool> DualAxisCondition { get; private set; }

			public DualAxisMap( string actionX, string actionY, 
				DualAxisAction delegateDualAxis, bool isVerbose, Func<bool> dualAxisCondition )
			{
				ActionMapX = new ActionMap( actionX );
				ActionMapY = new ActionMap( actionY );
				DelegateDualAxis = delegateDualAxis;
				IsVerbose = isVerbose;
				DualAxisCondition = dualAxisCondition;
		}

		}
		#endregion

		#region FIELDS
		private Rect m_screenRect = new Rect( );
		private bool m_isCursorInWindow;

		[System.NonSerialized] private Rewired.Player _player = null;
		private List<ButtonMap> _buttonMaps;
		private List<AxisMap> _axisMaps;
		private List<DualAxisMap> _dualAxisMaps;
		#endregion

		#region AUTO-PROPERTIES
		public System.Action<InputDevice> DelegateInputTracker { get; set; }
		public System.Action DelegateCursorLost { get; set; }
		public System.Action DelegateCursorGained { get; set; }

		public InputDevice CurrentDeviceMapped { get; set; }
		public InputDevice CurrentDeviceUsed { get; set; }

		public Vector3 MousePosition { get; private set; }
		public Vector3 PreviousMousePosition { get; private set; }
		public Vector3 MouseMovementVector { get; private set; }
		#endregion

		#region INITIALIZE
		public void Initialize( )
		{
			UpdateScreenRect( );

			MousePosition = Input.mousePosition;
			// Set it to the opposite of what it actually is so it always triggers on first loop
			m_isCursorInWindow = !m_screenRect.Contains( MousePosition );

			CurrentDeviceMapped = CurrentDeviceUsed = InputDevice.Undefined;

			_buttonMaps = new List<ButtonMap>( );
			_axisMaps = new List<AxisMap>( );
			_dualAxisMaps = new List<DualAxisMap>( );
		}
		#endregion

		#region TRACKING LOOP
		public void TrackingLoop( )
		{
			UpdateScreenRect( );

			PreviousMousePosition = MousePosition;
			MousePosition = Input.mousePosition;
			MouseMovementVector = MousePosition - PreviousMousePosition;
			CheckMouseCursorLocation( );
		}
		#endregion

		#region MAPPING LOOP
		public void MappingLoop( )
		{
			/* For Troubleshooting without joystick present
			if ( CheckKeyDown( KeyCode.Comma ) )
			{
				KeyIsDown( KeyCode.Joystick1Button11 );
			}
			*/
			
			if ( _player == null )
			{
				_player = ReInput.players.GetPlayer( 0 );
				_player.controllers.AddLastActiveControllerChangedDelegate( 
					OnActiveControllerChange );
			}

			int buttonMapCount = _buttonMaps.Count;
			for ( int i = 0; i < buttonMapCount; ++i )
			{
				ButtonMap map = _buttonMaps[i];

				int actionID = map.ActionID;

				if ( map.Mode == KeyMode.Holding || map.Mode == KeyMode.Window )
				{
					if ( _player.GetButton( actionID ) == false 
						&& _player.GetButtonUp( actionID ) == false )
					{
						// Special case scenario when loading saves that had
						// an active Holding or Window Chain running.
						if ( map.Mode == KeyMode.Holding )
						{
							if ( map.DelegateOneShotRelease != null 
								&& map.TestOneShotReleaseCondition( ) )
							{
								map.Mode = KeyMode.OneShotRelease;
								map.DelegateOneShotRelease?.Invoke( map );
							}

							if ( map.DelegateHoldingRelease != null
								&& map.TestHoldingReleaseCondition( ) )
							{ 
								map.Mode = KeyMode.HoldingRelease;
								map.DelegateHoldingRelease?.Invoke( map );
							}
						}
						else
						{
							if ( map.DelegateOneShotRelease != null
								&& map.TestOneShotReleaseCondition( ) )
							{ 
								map.Mode = KeyMode.OneShotRelease;
								map.DelegateOneShotRelease?.Invoke( map );
							}

							if ( map.DelegateWindowRelease != null
								&& map.TestWindowReleaseCondition( ) )
							{ 
								map.Mode = KeyMode.WindowRelease;
								map.DelegateWindowRelease?.Invoke( map );
							}
						}

						continue;
					}
				}

				// Down, Short-Press Down, and Long-Press down could all potentially happen
				// on the same frame
				if ( _player.GetButtonDown( actionID ) && map.TestOneShotCondition( ) )
				{
					//Debug.Log("GetButtonDown");
					map.Mode = KeyMode.OneShot;
					map.DelegateOneShot?.Invoke( map );
				}

				if ( _player.GetButtonShortPressDown( actionID ) && map.TestWindowCondition( ) )
				{
					//Debug.Log( "GetShortButtonDown" );
					map.Mode = KeyMode.Window;
					map.DelegateWindow?.Invoke( map );
				}

				if ( _player.GetButtonLongPressDown( actionID ) && map.TestHoldingCondition( ) )
				{
					//Debug.Log( "GetLongButtonDown" );
					map.Mode = KeyMode.Holding;
					map.DelegateHolding?.Invoke( map );
				}
				// However, we only want to execute the most specific Button Up state per frame
				if ( map.DelegateHolding != null && _player.GetButtonLongPressUp( actionID )
					&& map.TestHoldingReleaseCondition( ) )
				{
					//Debug.Log( "GetLongButtonUp" );
					map.Mode = KeyMode.HoldingRelease;
					map.DelegateHoldingRelease?.Invoke( map );
				}
				else 
				{
					if ( map.DelegateWindow != null && _player.GetButtonShortPressUp( actionID )
					&& map.TestWindowReleaseCondition( ) )
					{
						//Debug.Log( "GetShortButtonUp" );
						map.Mode = KeyMode.WindowRelease;
						map.DelegateWindowRelease?.Invoke( map );
					}
					
					if ( _player.GetButtonUp( actionID ) && map.TestOneShotReleaseCondition( ) )
					{
						//Debug.Log( "GetButtonUp" );
						map.Mode = KeyMode.OneShotRelease;
						map.DelegateOneShotRelease?.Invoke( map );
					}
				} 
			}

			int axisMapCount = _axisMaps.Count;
			for ( int i = 0; i < axisMapCount; ++i )
			{
				AxisMap map = _axisMaps[i];

				if ( map.AxisCondition != null && map.AxisCondition( ) == false )
					continue;

				float movement = _player.GetAxis( map.ActionID );
				bool isVerbose = map.IsVerbose;
				if ( isVerbose || 
					(!Misc.Floater.Equal( movement, 0f )) )
				{ 
					map.DelegateAxis( map, movement );
				}
			}

			int dualAxisMapCount = _dualAxisMaps.Count;
			for ( int i = 0; i < dualAxisMapCount; ++i )
			{
				DualAxisMap map = _dualAxisMaps[i];

				if ( map.DualAxisCondition != null && map.DualAxisCondition( ) == false )
					continue;

				float movementX = _player.GetAxis( map.ActionMapX.ActionID );
				float movementY = _player.GetAxis( map.ActionMapY.ActionID );
				bool isVerbose = map.IsVerbose;
				if ( isVerbose || 
					(!Misc.Floater.Equal( movementX, 0f ) || 
					!Misc.Floater.Equal( movementY, 0f )) )
				{
					map.DelegateDualAxis( map, movementX, movementY );
				}
			}
		}
		#endregion

		#region MAPPING
		public void AddNewDevice( InputDevice device )
		{
			CurrentDeviceMapped = device;
		}

		public bool IsUsingGamepad( )
		{
			return CurrentDeviceUsed == InputDevice.Gamepad;
		}

		public bool IsUsingMouseAndKeyboard( )
		{
			return CurrentDeviceUsed == InputDevice.MouseAndKeyboard;
		}

		public void MapButton( string actionName, 
			ButtonAction delegateOneShot = null, ButtonAction delegateOneShotRelease = null,
			ButtonAction delegateWindow = null, ButtonAction delegateWindowRelease = null,
			ButtonAction delegateHolding = null, ButtonAction delegateHoldingRelease = null,
			Func<bool> oneShotCondition = null, Func<bool> oneShotReleaseCondition = null,
			Func<bool> windowCondition = null, Func<bool> windowReleaseCondition = null,
			Func<bool> holdingCondition = null, Func<bool> holdingReleaseCondition = null )
		{
			_buttonMaps.Add( new ButtonMap( actionName, 
				delegateOneShot, delegateOneShotRelease, 
				delegateWindow, delegateWindowRelease, 
				delegateHolding, delegateHoldingRelease,
				oneShotCondition, oneShotReleaseCondition,
				windowCondition, windowReleaseCondition,
				holdingCondition, holdingReleaseCondition ) );
		}

		public void MapAxis( string actionName, AxisAction delegateAxis,
			bool isVerbose = false,
			Func<bool> axisCondition = null )
		{
			_axisMaps.Add( 
				new AxisMap( actionName, delegateAxis, isVerbose, axisCondition) );
		}

		public void MapDualAxis( string actionX, string actionY,  DualAxisAction delegateDualAxis, 
			bool isVerbose = false, 
			Func<bool> dualAxisCondition = null )
		{
			_dualAxisMaps.Add( 
				new DualAxisMap( actionX, actionY, delegateDualAxis, isVerbose, dualAxisCondition ) );
		}
		#endregion

		public void OnActiveControllerChange( Player player, Controller controller )
		{
			if ( controller.ImplementsTemplate<IGamepadTemplate>( ) )
			{
				CurrentDeviceUsed = InputDevice.Gamepad;
			}
			else
			{
				CurrentDeviceUsed = InputDevice.MouseAndKeyboard;
			}
		}

		private bool CheckMouseCursorLocation( )
		{
			if (!m_screenRect.Contains(MousePosition))
			{
				if (m_isCursorInWindow == true)
				{
					//Debug.Log("Cursor has gone outside game window");
					DelegateCursorLost();
					m_isCursorInWindow = false;
				}
			}
			else
			{
				if (m_isCursorInWindow == false)
				{
					//Debug.Log("Cursor has come inside game window");
					DelegateCursorGained();
					m_isCursorInWindow = true;
				}
			}

			return m_isCursorInWindow;
		}

		private void UpdateScreenRect( )
		{
			m_screenRect.width = Screen.width;
			m_screenRect.height = Screen.height;
		}
	}


}
