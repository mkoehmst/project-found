using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

public class InputMaster
{
	public enum KeyMode
	{
		Undefined,
		OneShot,
		OneShotRelease,
		Holding,
		HoldingRelease,
		HoldingWindow
	}

	public class KeyMap
	{
		public bool IsEnabled		{ get; private set; }
		public KeyMode Mode			{ get; private set; }
		public KeyCode Key			{ get; private set; }
		public KeyAction Action		{ get; private set; }
		public float? HoldingWindow { get; set; }

		public KeyMap( bool isEnabled, KeyAction action, KeyCode key )
		{
			IsEnabled = isEnabled;
			Mode = KeyMode.Undefined;
			Action = action;
			Key = key;
			HoldingWindow = null;
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

			Mode = KeyMode.Undefined;
		}

		public void OpenHoldingWindow( float delay )
		{
			HoldingWindow = delay;
		}

		public void CloseHoldingWindow( )
		{
			HoldingWindow = null;
		}

		public bool IsHoldingWindowOpen( )
		{
			return HoldingWindow.HasValue;
		}
	}

	public class AxisMap
	{
		public bool IsEnabled { get; private set; }
		public string Axis { get; set; }
		public AxisAction Action { get; set; }

		public AxisMap( bool isEnabled, AxisAction action, string axis )
		{
			IsEnabled = isEnabled;
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
			Action( movement );
		}
	}

	public class AxiiMap
	{
		public bool IsEnabled { get; private set; }
		public string[] Axii { get; set; }
		public AxiiAction Action { get; set; }

		public AxiiMap( bool isEnabled, AxiiAction action, string[] axii )
		{
			IsEnabled = isEnabled;
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
			Action( movements );
		}
	}

	public delegate void KeyAction( KeyMap map );
	public delegate void AxisAction( float movement );
	public delegate void AxiiAction( float[] movements );

	public Dictionary<KeyCode,KeyMap> KeyMaps { get; set; }
	public Dictionary<string,AxisMap> AxisMaps { get; set; }
	public Dictionary<string[], AxiiMap> AxiiMaps { get; set; }

	public InputMaster( )
	{
		KeyMaps = new Dictionary<KeyCode,KeyMap>( );
		AxisMaps = new Dictionary<string,AxisMap>( );
		AxiiMaps = new Dictionary<string[],AxiiMap>( new AxiiEqualityComparer( ) );
	}

	public void Loop( )
	{
		foreach ( KeyCode keyToCheck in KeyMaps.Keys )
		{
			if ( CheckKeyDown( keyToCheck ) )
			{
				KeyIsDown( keyToCheck );
			}
			else if ( CheckKeyHolding( keyToCheck ) )
			{
				// Use an else-if because holding check should start the frame after
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

			if ( axisMovement != 0f )
			{
				AxisHasMoved( axisToCheck, axisMovement );
			}
		}

		foreach ( string[] axiiToCheck in AxiiMaps.Keys )
		{
			float[] axiiMovements = CheckAxii( axiiToCheck );

			foreach ( float axisMovement in axiiMovements )
			{
				if ( axisMovement != 0f )
				{
					AxiiHaveMoved( axiiToCheck, axiiMovements );
					break;
				}
			}
		}
	}

	public KeyMap MapKey( bool isEnabled, KeyAction action, KeyCode key )
	{
		return KeyMaps[key] = new KeyMap( isEnabled, action, key );
	}

	public AxisMap MapAxis( bool isEnabled, AxisAction action, string axis )
	{
		return AxisMaps[axis] = new AxisMap( isEnabled, action, axis );
	}

	public void MapAxii( bool isEnabled, AxiiAction action, params string[] axii )
	{
		// TODO: String concatenation and parsing instead of string arrays?
		AxiiMaps[axii] = new AxiiMap( isEnabled, action, axii );
	}

	public bool CheckKeyDown( KeyCode keyToCheck )
	{
		return Input.GetKeyDown( keyToCheck );
	}

	public bool CheckKeyUp( KeyCode keyToCheck )
	{
		return Input.GetKeyUp( keyToCheck );
	}

	public bool CheckKeyHolding( KeyCode keyToCheck )
	{
		return Input.GetKey( keyToCheck );
	}

	public float CheckAxis( string axisToCheck )
	{
		return Input.GetAxis( axisToCheck );
	}

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
		KeyMap map = FindKeyMap( key );

		if ( !map.IsEnabled )
			return ;

		map.Fire( KeyMode.OneShot );
	}

	private void KeyIsUp( KeyCode key )
	{
		KeyMap map = FindKeyMap( key );

		if ( !map.IsEnabled )
			return ;

		if ( map.IsHoldingWindowOpen( ) )
		{
			if ( map.HoldingWindow <= 0f )
			{
				map.Fire( KeyMode.HoldingRelease );
			}
			else
			{
				map.Fire( KeyMode.OneShotRelease );
			}

			map.CloseHoldingWindow( );
		}
		else
		{
			map.Fire( KeyMode.OneShotRelease );
		}
	}

	private void KeyIsHolding( KeyCode key )
	{
		KeyMap map = FindKeyMap( key );

		if ( !map.IsEnabled )
			return ;

		if ( map.IsHoldingWindowOpen( ) )
		{
			map.HoldingWindow -= Time.deltaTime;

			if ( map.HoldingWindow <= 0f )
			{
				map.Fire( KeyMode.Holding );
			}
		}
	}

	private void AxisHasMoved( string axis, float movement )
	{
		AxisMap map = FindAxisMap( axis );

		if ( !map.IsEnabled )
			return ;

		map.Fire( movement );
	}

	private void AxiiHaveMoved( string[] axii, float[] movements )
	{
		AxiiMap map = FindAxiiMap( axii );

		if ( !map.IsEnabled )
			return ;

		map.Fire( movements );
	}

	private KeyMap FindKeyMap( KeyCode key )
	{
		return KeyMaps[key];
	}

	private AxisMap FindAxisMap( string axis )
	{
		return AxisMaps[axis];
	}

	private AxiiMap FindAxiiMap( string[] axii )
	{
		return AxiiMaps[axii];
	}
}
