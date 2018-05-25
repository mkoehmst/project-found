namespace ProjectFound.Environment.Occlusion
{

	using UnityEngine;

	public class Occludable : Interactee
	{
		public void Hide( )
		{
			Hide( transform );
		}

		public void Show( )
		{
			Show( transform );
		}

		private void Hide( Transform parent )
		{
			int count = parent.childCount;
			for ( int i = 0; i < count; ++i )
			{
				Transform child = parent.GetChild( i );
				Hide( child );
			}

			parent.gameObject.layer = (int)LayerID.RoofHidden;
		}

		private void Show( Transform parent )
		{
			int count = parent.childCount;
			for ( int i = 0; i < count; ++i )
			{
				Transform child = parent.GetChild( i );
				Show( child );
			}

			parent.gameObject.layer = (int)LayerID.Roof;
		}
	}

}
