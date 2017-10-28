using System;
using UnityEngine;


namespace ProjectFound.Misc {

	class Floater
	{

		public static bool Equal( float v, float k )
		{
			float leftSide = Math.Abs( v - k );
			float rightSide = (Math.Abs( v ) + Math.Abs( k ) + 1f) * float.Epsilon;

			return leftSide <= rightSide;
		}

		public static bool LessThan( float a, float b )
		{
			if ( !Equal( a, b ) )
				return a < b;
			else
				return false;
		}

		public static bool GreaterThan( float a, float b )
		{
			if ( !Equal( a, b ) )
				return a > b;
			else
				return false;
		}

		public static bool LessThanOrEqual( float a, float b )
		{
			if ( Equal( a, b ) )
				return true;
			else
				return a < b;
		}

		public static bool GreaterThanOrEqual( float a, float b )
		{
			if ( Equal( a, b ) )
				return true;
			else
				return a > b;
		}
	}

}