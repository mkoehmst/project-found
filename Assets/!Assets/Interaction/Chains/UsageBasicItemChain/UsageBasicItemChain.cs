namespace ProjectFound.Environment.Handlers
{
	using System.Collections;
	using UnityEngine;

	[CreateAssetMenu( menuName = ("Project Found/Handler Chains/Usage Basic Item Chain") )]
	public class UsageBasicItemChain : HandlerChain
	{
		public DropItemHandler m_dropItemHandler;

		new void OnEnable( )
		{
			base.OnEnable( );

			AddHandler( m_dropItemHandler, HandlerExecutionMode.Async );
		}
	}

}

