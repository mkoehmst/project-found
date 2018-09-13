namespace ProjectFound.Core.Master
{


	using UnityEngine;

	public class ShaderMaster
	{
		public void SetFocusOutlineActive( GameObject obj, bool isActive )
		{
			var outlines = obj.GetComponentsInChildren<cakeslice.Outline>( );

			int length = outlines.Length;
			for ( int i = 0; i < length; ++i )
			{
				outlines[i].SetOutlineActive( isActive );
			}
		}
	}


}