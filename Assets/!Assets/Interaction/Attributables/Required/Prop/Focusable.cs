namespace ProjectFound.Interaction
{ 


	using UnityEngine;
	using UnityEngine.Assertions;

	using Autelia.Serialization;

	public class Focusable : Attributable 
	{
		[SerializeField] HandlerChain _focusChain;

		new protected void Awake( )
		{
			base.Awake( );

			if ( Serializer.IsLoading ) return;

			Assert.IsNotNull( _focusChain );
		}

		protected void Start( )
		{
			if ( Serializer.IsLoading ) return;

			Interactee.FocusChain = _focusChain;
			Interactee.FocusReleaseChain = _focusChain;
		}
	}


}
