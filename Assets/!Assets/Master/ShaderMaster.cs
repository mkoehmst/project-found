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

			Assert.IsTrue(
				outlines.Length > 0,
				"ShaderMaster: GO(" + obj +") or its children does not have Outline component." );

			foreach ( var outline in outlines )
			{
				outline.enabled = !outline.enabled;
			}
		}
	}


}
