namespace ProjectFound.Environment.Handlers
{
	using System.Collections;
	using UnityEngine;

	[CreateAssetMenu(menuName=("Project Found/Handler Chains/Selection Basic Item Chain"))]
	public class SelectionBasicItemChain : HandlerChain
	{
		public PickupHandler m_pickupHandler;

		new void OnEnable( )
		{
			base.OnEnable( );

			AddHandler( m_pickupHandler, HandlerExecutionMode.Blocking );
		}
	}

}
