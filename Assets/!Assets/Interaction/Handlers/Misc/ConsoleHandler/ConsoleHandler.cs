namespace ProjectFound.Environment.Handlers
{

	using System.Collections.Generic;
	
	using UnityEngine;

	[CreateAssetMenu( menuName = ("Project Found/Handlers/Console Handler") )]
	public class ConsoleHandler : InteracteeHandler
	{
		public override IEnumerator<float> Handler( Interactee ie, Interactor ir )
		{
			Debug.Log( "ConsoleHandler: " + ir );

			yield break;
		}
	}

}

