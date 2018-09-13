namespace ProjectFound.Environment.Surfaces
{


	using UnityEngine;
	using Autelia.Serialization;

	using ProjectFound.Interaction;

	[RequireComponent(typeof(Walkable))]
	public class WalkableSurface : UsableSurface
	{
		new protected void Awake( )
		{
			base.Awake( );

			LayerID = LayerID.Walkable;

			if ( Serializer.IsLoading ) return ;
		}

		new protected void Start( )
		{
			base.Start( );

			if ( Serializer.IsLoading ) return;
		}
	}


}
