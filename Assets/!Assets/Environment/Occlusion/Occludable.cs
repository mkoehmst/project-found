using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ProjectFound.Environment.Occlusion
{


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

		public override bool ValidateAction( ActionType actionType )
		{
			return true;
		}

		public override void Reaction( )
		{ }
	}


}