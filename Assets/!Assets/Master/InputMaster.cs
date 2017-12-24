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
			public float? HoldingWindow { get; set; }
			public uint HoldingCount	{ get; set; }

			public KeyMap( bool isEnabled, InputDevice device, KeyAction action, KeyCode key )
			{
				IsEnabled = isEnabled;
				Device = device;
				Mode = KeyMode.Undefined;
				Action = action;
				Key = key;
				HoldingWindow = null;
				HoldingCount = 0;
			}

			public void Enable( )
			{
				IsEnabled = true;
			}

			public void Disable( )
			{
				IsEnabled = false;
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

				public int GetHashCode( string[] obj )
				{
					int hashCode = 1;

					for ( int i = 0; i < obj.Length; ++i )
					{
						unchecked
						{
							hashCode *= obj[i].GetHashCode( );
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

		public InputMaster( )
		{
			m_screenRect = new Rect( );
			m_isCursorInWindow = true;

			DeviceMappings = new Dictionary<InputDevice,DeviceMapping>( );

			KeyMaps = new Dictionary<KeyCode,KeyMap>( );
			AxisMaps = new Dictionary<string,AxisMap>( );
			AxiiMaps = new Dictionary<string[],AxiiMap>( new AxiiMap.AxiiEqualityComparer( ) );

			CurrentDeviceMapped = CurrentDeviceUsed = InputDevice.Undefined;
		}

		public void Loop( )
		{
			if ( CheckMouseCursorLocation( ) == true )
			{
				float mouseX = Input.GetAxis( "Mouse X" );
				float mouseY = Input.GetAxis( "Mouse Y" );
				if ( !Misc.Floater.Equal( mouseX, 0f ) || !Misc.Floater.Equal( mouseY, 0f ) )
				{
					CurrentDeviceUsed = InputDevice.MouseAndKeyboard;
				}
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
					}
					// Use an else-if because holding check should start the frame after
					else if ( CheckKeyHolding( keyToCheck ) )
					{
						KeyIsHolding( keyToCheck );
					}

					if ( CheckKeyUp( keyToCheck ) )
					{
						KeyIsUp( keyToCheck );
					}
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
		}

		public void AddNewDevice( InputDevice device )
		{
			CurrentDeviceMapped = device;

			DeviceMappings[device] = new DeviceMapping( device );
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

			map.Fire( KeyMode.OneShot );
		}

		private void KeyIsUp( KeyCode key )
		{
			KeyMap map = KeyMaps[key];

			CurrentDeviceUsed = map.Device;

			if ( !map.IsEnabled )
				return ;

			// Can't have a key up without a key down first, so check previous Mode
			if ( !(map.Mode == KeyMode.OneShot || map.Mode == KeyMode.Holding) )
				return ;

			if ( map.HoldingWindow != null )
			{
				if ( map.HoldingWindow <= 0f )
				{
					map.Fire( KeyMode.HoldingRelease );
				}
				else
				{
					map.Fire( KeyMode.OneShotRelease );
				}

				map.HoldingWindow = null;
				map.HoldingCount = 0;
			}
			else
			{
				map.Fire( KeyMode.OneShotRelease );
			}
		}

		private void KeyIsHolding( KeyCode key )
		{
			KeyMap map = KeyMaps[key];

			CurrentDeviceUsed = map.Device;

			if ( !map.IsEnabled )
				return ;

			if ( map.HoldingWindow != null )
			{
				map.HoldingWindow -= Time.deltaTime;

				if ( map.HoldingWindow <= 0f )
				{
					++map.HoldingCount;
					map.Fire( KeyMode.Holding );
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
			m_screenRect.width = Screen.width;
			m_screenRect.height = Screen.height;

			if ( m_isCursorInWindow == true )
			{
				if ( !m_screenRect.Contains( Input.mousePosition ) )
				{
					Debug.Log( "Cursor has gone outside game window" );
					DelegateCursorLost( );
					m_isCursorInWindow = false;
				}
			}
			else
			{
				if ( m_screenRect.Contains( Input.mousePosition ) )
				{
					Debug.Log( "Cursor has come back within game window" );
					DelegateCursorGained( );
					m_isCursorInWindow = true;
				}
			}

			return m_isCursorInWindow;
		}
	}


}
