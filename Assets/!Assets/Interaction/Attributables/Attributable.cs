namespace ProjectFound.Interaction
{ 


	using UnityEngine;
	using UnityEngine.Assertions;

	using Autelia.Serialization;

	public class Attributable : MonoBehaviour 
	{
		public Interactee Interactee { get; private set; }

		protected void Awake( )
		{
			if ( Serializer.IsLoading ) return;

			Interactee = GetComponent<Interactee>( );
			Assert.IsNotNull( Interactee );
		}
	}


}
