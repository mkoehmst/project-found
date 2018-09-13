namespace ProjectFound.Environment.Surfaces
{


	using UnityEngine;
	
	using Autelia.Serialization;

	public class UsableSurface : Surface
	{
		new protected void Awake( )
		{
			base.Awake( );

			LayerID = LayerID.Usable;

			if ( Serializer.IsLoading ) return ;
		}

		new protected void Start( )
		{
			base.Start( );

			if ( Serializer.IsLoading ) return;
		}
	}


}
