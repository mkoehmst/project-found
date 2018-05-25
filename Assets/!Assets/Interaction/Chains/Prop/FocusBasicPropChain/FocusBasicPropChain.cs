namespace ProjectFound.Environment.Handlers
{

	using UnityEngine;

	[CreateAssetMenu(menuName = ("Project Found/Handler Chains/Focus Basic Prop Chain"))]
	public class FocusBasicPropChain : HandlerChain
	{
		public OutlineHandler m_outlineHandler;
		public PromptHandler m_promptHandler;

		new void OnEnable( )
		{
			base.OnEnable( );

			AddHandler( m_outlineHandler, HandlerExecutionMode.Async );
			AddHandler( m_promptHandler, HandlerExecutionMode.Async );
		}
	}

}
