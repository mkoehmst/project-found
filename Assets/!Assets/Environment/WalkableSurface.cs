using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ProjectFound.Environment
{

	using ProjectFound.Environment.Handlers;

	public class WalkableSurface : UsableSurface
	{
		new void Start( )
		{
			if ( m_areLayersSet == false )
			{
				SetLayer( transform, LayerID.Walkable );

				m_areLayersSet = true;
			}

			base.Start( );
		}
	}



}
