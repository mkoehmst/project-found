namespace ProjectFound.Interaction
{
	

	using UnityEngine;
	using UnityEngine.Assertions;

	using Autelia.Serialization;

	public class Walkable : Attributable
	{
		[SerializeField] HandlerChain _oneShotMovementChain;
		[SerializeField] HandlerChain _holdingMovementChain;

		new void Awake( )
		{
			base.Awake( );
			
			if ( Serializer.IsLoading ) return ;

			Assert.IsNotNull( _oneShotMovementChain );
		}

		void Start( ) 
		{
			if ( Serializer.IsLoading ) return;

			Interactee.OneShotChain = _oneShotMovementChain;
			Interactee.OneShotReleaseChain = _oneShotMovementChain;
			Interactee.WindowChain = _holdingMovementChain;
			Interactee.WindowReleaseChain = _holdingMovementChain;
			Interactee.HoldingChain = _holdingMovementChain;
			Interactee.HoldingReleaseChain = _holdingMovementChain;
		}
	}


}