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
			var outline = obj.GetComponent<cakeslice.Outline>( );

			Assert.IsNotNull(
				outline, "ShaderMaster: GO(" + obj +") does not have Outline component." );

			outline.enabled = !outline.enabled;
		}

	}


}
