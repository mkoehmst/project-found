namespace ProjectFound.Environment.Handlers
{
	using System.Collections;
	using UnityEngine;

	[CreateAssetMenu(menuName=("Project Found/Handler Chains/Selection Basic Item Chain"))]
	public class SelectionBasicItemChain : HandlerChain
	{
		public PickupHandler m_pickupHandler;

		void OnEnable( )
		{
			if ( m_pickupHandler == null )
			{
				Debug.LogError( "m_pickupHandler is null!" );
			}
			else
			{
				m_asyncHandlers.Add( m_pickupHandler );
			}
		}

		public override IEnumerator ExecuteChain( Interactee ie, Interactor ir )
		{
			BeginChain( ir );

			ir.StartCoroutine( m_pickupHandler.Handle( ie, ir ) );

			while ( EndChain( ir ) == false )
			{
				yield return null;
			}

			yield break;
		}
	}

}
