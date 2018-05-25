namespace ProjectFound.Environment.Handlers
{

	using System.Collections;
	using UnityEngine;

	[CreateAssetMenu( menuName = ("Project Found/Handler Chains/Selection Basic Prop Chain") )]
	public class SelectionBasicPropChain : HandlerChain
	{
		public AnimationHandler m_animationHandler;
		public ConsoleHandler m_consoleHandler;

		new void OnEnable( )
		{
			base.OnEnable( );

			AddHandler( m_animationHandler, HandlerExecutionMode.Blocking );
			AddHandler( m_consoleHandler, HandlerExecutionMode.Async );
		}
	}

}


