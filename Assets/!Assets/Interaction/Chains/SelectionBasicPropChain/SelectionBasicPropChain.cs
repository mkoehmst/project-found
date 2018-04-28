namespace ProjectFound.Environment.Handlers
{

	using System.Collections;
	using UnityEngine;

	[CreateAssetMenu( menuName = ("Project Found/Handler Chains/Selection Basic Prop Chain") )]
	public class SelectionBasicPropChain : HandlerChain
	{

		public AnimationHandler m_animationHandler;
		public ConsoleHandler m_consoleHandler;

		void OnEnable( )
		{
			m_asyncHandlers.Add( m_consoleHandler );
		}

		public override IEnumerator ExecuteChain( Interactee ie, Interactor ir )
		{
			// Set Interactor busy flag
			BeginChain( ir );

			// Blocking Handler
			yield return m_animationHandler.Handle( ie, ir );

			// Async Handler
			ie.StartCoroutine( m_consoleHandler.Handle( ie, ir ) );

			// Wait until all async handlers are complete and unset Interactor busy flag
			while ( EndChain( ir ) == false )
			{
				yield return null;
			}

			// End Chain
			yield break;
		}
	}

}


