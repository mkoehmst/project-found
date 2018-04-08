using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ProjectFound.Environment
{

	public class UsableSurface : Interactee
	{
		protected bool m_areLayersSet = false;

		new void Start( )
		{
			if ( m_areLayersSet == false )
			{
				SetLayer( transform, LayerID.Usable );

				m_areLayersSet = true;
			}

			base.Start( );
		}

		protected void SetLayer( Transform parent, LayerID layer )
		{
			int count = parent.childCount;
			for ( int i = 0; i < count; ++i )
			{
				Transform child = parent.GetChild( i );
				SetLayer( child, layer );
			}

			parent.gameObject.layer = (int)layer;
		}
	}


}
