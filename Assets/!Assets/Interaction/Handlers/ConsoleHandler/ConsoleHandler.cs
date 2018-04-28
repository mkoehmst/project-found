namespace ProjectFound.Environment.Handlers
{

	using System.Collections;
	using UnityEngine;

	[CreateAssetMenu( menuName = ("Project Found/Handlers/Console Handler") )]
	public class ConsoleHandler : InteracteeHandler
	{
		public override IEnumerator Handle( Interactee interactee, Interactor interactor )
		{
			interactor.HandlerExecutionDictionary[this] = true;

			Debug.Log( "ConsoleHandler: " + interactor );

			interactor.HandlerExecutionDictionary[this] = false;

			yield break;
		}
	}

}

