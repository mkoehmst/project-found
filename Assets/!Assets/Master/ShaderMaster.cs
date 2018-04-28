using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

namespace ProjectFound.Master
{


	public class ShaderMaster
	{
		public void ToggleSelectionOutline( GameObject obj )
		{
			var outlines = obj.GetComponentsInChildren<cakeslice.Outline>( );

			for ( int i = 0; i < outlines.Length; ++i )
			{
				outlines[i].ToggleOutline( );
			}
		}
	}


}
