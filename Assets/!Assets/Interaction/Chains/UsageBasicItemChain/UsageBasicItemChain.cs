namespace ProjectFound.Environment.Handlers
{
	using System.Collections;
	using UnityEngine;

	[CreateAssetMenu( menuName = ("Project Found/Handler Chains/Usage Basic Item Chain") )]
	public class UsageBasicItemChain : HandlerChain
	{
		public DropItemHandler m_dropItemHandler;

		void OnEnable( )
		{
			if ( m_dropItemHandler == null )
			{
				Debug.LogError( "m_dropItemHandler is null!" );
			}
			else
			{
				m_asyncHandlers.Add( m_dropItemHandler );
			}
		}

		public override IEnumerator ExecuteChain( Interactee ie, Interactor ir )
		{
			BeginChain( ir );

			ir.StartCoroutine( m_dropItemHandler.Handle( ie, ir ) );

			while ( EndChain( ir ) == false )
			{
				yield return null;
			}

			yield break;
		}
	}

}

