using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ProjectFound.Master {

	public class InputMaster
	{
		public delegate void KeyAction( KeyMap map );
		public delegate void AxisAction( AxisMap map, float movement );
		public delegate void AxiiAction( AxiiMap map, float[] movements );

		public enum KeyMode
		{
			Undefined,
			OneShot,
			OneShotRelease,
			Holding,
			HoldingRelease,
			HoldingWindow
		}

		public enum InputDevice
		{
			Undefined,
			MouseAndKeyboard,
			Gamepad
		}

		public class KeyMap
		{
			public bool IsEnabled		{ get; set; }
			public InputDevice Device	{ get; private set; }
			public KeyMode Mode			{ get; private set; }
			public KeyCode Key			{ get; private set; }
			public KeyAction Action		{ get; private set; }
			public float? HoldingWindow { get; private set; }
			public int HoldingCount		{ get; set; }

			public KeyMap( bool isEnabled, InputDevice device, KeyAction action, KeyCode key )
			{
				IsEnabled = isEnabled;
				Device = device;
				Action = action;
				Key = key;

				ResetMode( );
				ResetHoldingWindow( );
			}

			public void OpenHoldingWindow( float window )
			{
				if ( window != 0f && window < 0.2f )
				{
					window = 0.2f;
				}

				HoldingWindow = window;
			}

			public void WindowTick( float timeDelta )
			{
				HoldingWindow -= timeDelta;
			}

			public void ResetMode( )
			{
				Mode = KeyMode.Undefined;
			}

			public void ResetHoldingWindow( )
			{
				HoldingWindow = null;
				HoldingCount = -1;
			}

			public void Enable( )
			{
				IsEnabled = true;
			}

			public void Disable( )
			{
				IsEnabled = false;

				ResetMode( );
				ResetHoldingWindow( );
			}

			public void Fire( KeyMode mode )
			{
				Mode = mode;

				Action( this );
			}
		}

		public class AxisMap
		{
			public bool IsEnabled { get; set; }
			public InputDevice Device { get; private set; }
			public string Axis { get; set; }
			public AxisAction Action { get; set; }

			public AxisMap( bool isEnabled, InputDevice device, AxisAction action, string axis )
			{
				IsEnabled = isEnabled;
				Device = device;
				Action = action;
				Axis = axis;
			}

			public void Enable( )
			{
				IsEnabled = true;
			}

			public void Disable( )
			{
				IsEnabled = false;
			}

			public void Fire( float movement )
			{
				Action( this, movement );
			}
		}

		public class AxiiMap
		{
			public class AxiiEqualityComparer : IEqualityComparer<string[]>
			{
				public bool Equals( string[] x, string[] y )
				{
					if ( x.Length != y.Length )
						return false;

					for ( int i = 0; i < x.Length; i++ )
					{
						if ( x[i] != y[i] )
							return false;
					}

					return true;
				}

				public int GetHashCode( string[] str )
				{
					int hashCode = 1;

					for ( int i = 0; i < str.Length; ++i )
					{
					
						unchecked
						{
							hashCode *= str[i].GetHashCode( );
						}
					}

					return hashCode;
				}
			}

			public bool IsEnabled { get; set; }
			public InputDevice Device { get; private set; }
			public string[] Axii { get; set; }
			public AxiiAction Action { get; set; }

			public AxiiMap( bool isEnabled, InputDevice device, AxiiAction action, string[] axii  )
			{
				IsEnabled = isEnabled;
				Device = device;
				Action = action;
				Axii = axii;
			}

			public void Enable( )
			{
				IsEnabled = true;
			}

			public void Disable( )
			{
				IsEnabled = false;
			}

			public void Fire( float[] movements )
			{
				Action( this, movements );
			}
		}

		public class DeviceMapping
		{
			public InputDevice Device { get; private set; }

			public Dictionary<KeyAction,KeyCode> ActionToKey { get; private set; }
			public Dictionary<AxisAction,string> ActionToAxis { get; private set; }
			public Dictionary<AxiiAction,string[]> ActionToAxii { get; private set; }

			public DeviceMapping( InputDevice device )
			{
				Device = device;

				ActionToKey = new Dictionary<KeyAction,KeyCode>( );
				ActionToAxis = new Dictionary<AxisAction,string>( );
				ActionToAxii = new Dictionary<AxiiAction,string[]>( );
			}
		}

		private Rect m_screenRect;
		private bool m_isCursorInWindow;

		private InputDevice m_currentDeviceUsed;
		public InputDevice CurrentDeviceUsed
		{
			get { return m_currentDeviceUsed; }
			private set
			{
				if ( m_currentDeviceUsed != value )
				{
					m_currentDeviceUsed = value;
					DelegateInputTracker( value );
				}
			}
		}

		public System.Action<InputDevice> DelegateInputTracker { get; set; }
		public System.Action DelegateCursorLost { get; set; }
		public System.Action DelegateCursorGained { get; set; }

		public Dictionary<KeyCode,KeyMap> KeyMaps { get; private set; }
		public Dictionary<string,AxisMap> AxisMaps { get; private set; }
		public Dictionary<string[],AxiiMap> AxiiMaps { get; private set; }

		public Dictionary<InputDevice,DeviceMapping> DeviceMappings { get; private set; }
		public InputDevice CurrentDeviceMapped { get; set; }

		public Vector3 MousePosition { get; private set; }
		public Vector3 PreviousMousePosition { get; private set; }
		public Vector3 MouseMovementVector { get; private set; }

		public InputMaster( )
		{
			m_screenRect = new Rect( );
			UpdateScreenRect();
			MousePosition = Input.mousePosition;
			// Set it to the opposite of what it actually is so it always triggers on first loop
			m_isCursorInWindow = !m_screenRect.Contains(MousePosition);

			DeviceMappings = new Dictionary<InputDevice,DeviceMapping>( );

			KeyMaps = new Dictionary<KeyCode,KeyMap>( );
			AxisMaps = new Dictionary<string,AxisMap>( );
			AxiiMaps = new Dictionary<string[],AxiiMap>( new AxiiMap.AxiiEqualityComparer( ) );

			CurrentDeviceMapped = CurrentDeviceUsed = InputDevice.Undefined;
		}

		public void TrackingLoop( )
		{
			UpdateScreenRect( );

			PreviousMousePosition = MousePosition;
			MousePosition = Input.mousePosition;
			MouseMovementVector = MousePosition - PreviousMousePosition;

			if (!Misc.Floater.Equal(MouseMovementVector.magnitude, 0f))
			{
				CurrentDeviceUsed = InputDevice.MouseAndKeyboard;
			}

			// Temporarily assign to MouseAndKeyboard and then back for safety
			InputDevice loopDevice = CurrentDeviceUsed;
			CurrentDeviceUsed = InputDevice.MouseAndKeyboard;
			CheckMouseCursorLocation( );
			CurrentDeviceUsed = loopDevice;
		}

		public void MappingLoop( )
		{
			/* For Troubleshooting without joystick present
			if ( CheckKeyDown( KeyCode.Comma ) )
			{
				KeyIsDown( KeyCode.Joystick1Button11 );
			}
			*/

			foreach ( KeyCode keyToCheck in KeyMaps.Keys )
			{
				if ( CheckKeyDown( keyToCheck ) )
				{
					KeyIsDown( keyToCheck );

					if ( CheckKeyUp( keyToCheck ) )
					{
						KeyIsUp( keyToCheck );
					}
				}
				else
				{
					if ( CheckKeyUp( keyToCheck ) )
					{
						KeyIsUp( keyToCheck );
					}
					else if ( CheckKeyHolding( keyToCheck ) )
					{
						KeyIsHolding( keyToCheck );
					}
				}
				// Use an else-if because holding check should start the frame after
			}

			foreach ( string axisToCheck in AxisMaps.Keys )
			{
				float axisMovement = CheckAxis( axisToCheck );

				if ( !Misc.Floater.Equal( axisMovement, 0f ) )
				{
					AxisHasMoved( axisToCheck, axisMovement );
				}
			}

			foreach ( string[] axiiToCheck in AxiiMaps.Keys )
			{
				float[] axiiMovements = CheckAxii( axiiToCheck );

				foreach ( float axisMovement in axiiMovements )
				{
					if ( !Misc.Floater.Equal( axisMovement, 0f ) )
					{
						AxiiHaveMoved( axiiToCheck, axiiMovements );
						break;
					}
				}
			}
		}

		public void AddNewDevice( InputDevice device )
		{
			CurrentDeviceMapped = device;

			DeviceMappings[device] = new DeviceMapping( device );
		}

		/*public void ResetKeyMap( KeyAction action )
		{
			KeyCode key = DeviceMappings[CurrentDeviceUsed].ActionToKey[action];
			KeyMap map = KeyMaps[key];

			map.Initialize( );
		}*/

		public KeyMap GetKeyMap( KeyAction action )
		{
			//if ( CurrentDeviceUsed == InputDevice.Undefined )
			//{
			//	return null;
			//}

			var deviceMapping = DeviceMappings[CurrentDeviceUsed];

			KeyCode key = deviceMapping.ActionToKey[action];
			KeyMap map = KeyMaps[key];

			return map;
		}

		public void MapKey( bool isEnabled, KeyAction action, KeyCode key )
		{
			DeviceMapping mapping = DeviceMappings[CurrentDeviceMapped];
			mapping.ActionToKey[action] = key;
			KeyMaps[key] = new KeyMap( isEnabled, CurrentDeviceMapped, action, key );
		}

		public void MapAxis( bool isEnabled, AxisAction action, string axis )
		{
			DeviceMapping mapping = DeviceMappings[CurrentDeviceMapped];
			mapping.ActionToAxis[action] = axis;
			AxisMaps[axis] = new AxisMap( isEnabled, CurrentDeviceMapped, action, axis );
		}

		public void MapAxii( bool isEnabled, AxiiAction action, params string[] axii )
		{
			DeviceMapping mapping = DeviceMappings[CurrentDeviceMapped];
			mapping.ActionToAxii[action] = axii;
			// TODO: String concatenation and parsing instead of string arrays?
			AxiiMaps[axii] = new AxiiMap( isEnabled, CurrentDeviceMapped, action, axii );
		}

		public void EnableMap( KeyCode key )
		{ KeyMaps[key].IsEnabled = true; }

		public void EnableMap( string axis )
		{ AxisMaps[axis].IsEnabled = true; }

		public void EnableMap( string[] axii )
		{ AxiiMaps[axii].IsEnabled = true; }

		public void DisableMap( KeyCode key )
		{ KeyMaps[key].IsEnabled = false; }

		public void DisableMap( string axis )
		{ AxisMaps[axis].IsEnabled = false; }

		public void DisableMap( string[] axii )
		{ AxiiMaps[axii].IsEnabled = false; }

		public KeyCode GetKeyFromAction( KeyAction action )
		{ return DeviceMappings[CurrentDeviceUsed].ActionToKey[action]; }

		public string GetAxisFromAction( AxisAction action )
		{ return DeviceMappings[CurrentDeviceUsed].ActionToAxis[action]; }

		public string[] GetAxiiFromAction( AxiiAction action )
		{ return DeviceMappings[CurrentDeviceUsed].ActionToAxii[action]; }

		public bool CheckKeyDown( KeyCode keyToCheck )
		{ return Input.GetKeyDown( keyToCheck ); }

		public bool CheckKeyUp( KeyCode keyToCheck )
		{ return Input.GetKeyUp( keyToCheck ); }

		public bool CheckKeyHolding( KeyCode keyToCheck )
		{ return Input.GetKey( keyToCheck ); }

		public float CheckAxis( string axisToCheck )
		{ return Input.GetAxis( axisToCheck ); }

		// TODO: Eliminate garbage generation
		public float[] CheckAxii( string[] axiiToCheck )
		{
			float[] movements = new float[axiiToCheck.Length];

			for ( int i = 0; i < movements.Length; ++i )
			{
				movements[i] = CheckAxis( axiiToCheck[i] );
			}

			return movements;
		}

		private void KeyIsDown( KeyCode key )
		{
			KeyMap map = KeyMaps[key];

			CurrentDeviceUsed = map.Device;

			if ( !map.IsEnabled )
				return ;

			//Debug.Log("Key is OneShot");

			map.Fire( KeyMode.OneShot );
		}

		private void KeyIsUp( KeyCode key )
		{
			KeyMap map = KeyMaps[key];

			CurrentDeviceUsed = map.Device;

			if ( !map.IsEnabled )
			{ 
				return ;
			}

			// Can't have a key up without a key down first, so check previous Mode
			if ( !(map.Mode == KeyMode.OneShot 
				|| map.Mode == KeyMode.Holding 
				|| map.Mode == KeyMode.HoldingWindow) )
			{ 
				return ;
			}

			if ( map.HoldingWindow.HasValue )
			{
				if ( map.HoldingWindow.Value <= 0f )
				{
					//Debug.Log("Key is HoldingRelease");
					map.Fire( KeyMode.HoldingRelease );
				}
				else
				{
					//Debug.Log("Key is OneShotRelease (1)");
					map.Fire( KeyMode.OneShotRelease );
				}

				map.ResetHoldingWindow( );
			}
			else
			{
				//Debug.Log("Key is OneShotRelease (2)");
				map.Fire( KeyMode.OneShotRelease );
			}
		}

		private void KeyIsHolding( KeyCode key )
		{
			KeyMap map = KeyMaps[key];

			CurrentDeviceUsed = map.Device;

			if ( !map.IsEnabled )
				return ;

			if ( map.HoldingWindow.HasValue )
			{
				map.WindowTick( Time.deltaTime );

				if ( map.HoldingWindow.Value <= 0f )
				{
					if ( ++map.HoldingCount == 1 )
					{
						//Debug.Log( "Key is Holding" );
						map.Fire( KeyMode.Holding );
					}
				}
				else if ( map.HoldingCount == -1 )
				{
					map.HoldingCount = 0;
					//Debug.Log( "Key is HoldingWindow" );
					map.Fire( KeyMode.HoldingWindow );
				}
			}
			// Else fire OneShot?
		}

		public void AxisHasMoved( string axis, float movement )
		{
			AxisMap map = AxisMaps[axis];

			CurrentDeviceUsed = map.Device;

			if ( !map.IsEnabled )
				return ;

			map.Fire( movement );
		}

		private void AxiiHaveMoved( string[] axii, float[] movements )
		{
			AxiiMap map = AxiiMaps[axii];

			CurrentDeviceUsed = map.Device;

			if ( !map.IsEnabled )
				return ;

			map.Fire( movements );
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
