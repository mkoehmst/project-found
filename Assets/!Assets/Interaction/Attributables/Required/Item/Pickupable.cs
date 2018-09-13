namespace ProjectFound.Interaction
{ 


	using System.Collections.Generic;

	using UnityEngine;
	using UnityEngine.Assertions;
	using Autelia.Serialization;

	public class Pickupable : Attributable 
	{
		[SerializeField] HandlerChain _pickupChain;

		protected new void Awake( )
		{
			base.Awake( );

			if ( Serializer.IsLoading ) return ;

			Assert.IsNotNull( _pickupChain );
		}

		protected void Start () 
		{
			if ( Serializer.IsLoading ) return ;

			Interactee.OneShotReleaseChain = _pickupChain;
		}
	}


}