namespace ProjectFound.Environment.Surfaces
{ 

	
	using Autelia.Serialization;

	using ProjectFound.Interaction;

	public abstract class Surface : Interactee 
	{
		new protected void Start( ) 
		{
			base.Start( );

			if ( Serializer.IsLoading ) return ;
		}
	}


}