namespace ProjectFound.Interaction
{ 
	

	using UnityEngine;
	using UnityEngine.Assertions;
	using Autelia.Serialization;

	public class Usable : Attributable 
	{
		[SerializeField] HandlerChain _usageChain;

		protected new void Awake( )
		{
			base.Awake( );

			if ( Serializer.IsLoading ) return ;

			Assert.IsNotNull( _usageChain );
		}

		protected void Start( )
		{
			if ( Serializer.IsLoading ) return ;

			Interactee.UsageChain = _usageChain;
		}
	}


}
