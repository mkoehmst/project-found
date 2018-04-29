namespace ProjectFound.Environment.Handlers
{

	using System.Collections;
	using UnityEngine;

	[CreateAssetMenu( menuName = ("Project Found/Handler Chains/Drag And Drop Basic Prop Chain") )]
	public class DragAndDropBasicPropChain : HandlerChain
	{
		public DragAndDropHandler m_dragAndDropHandler;

		new void OnEnable( )
		{
			base.OnEnable( );

			AddHandler( m_dragAndDropHandler, HandlerExecutionMode.Async );
		}
	}

}