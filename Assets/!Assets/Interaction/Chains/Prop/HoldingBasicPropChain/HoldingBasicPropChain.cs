namespace ProjectFound.Environment.Handlers
{

	using UnityEngine;

	[CreateAssetMenu(menuName = ("Project Found/Handler Chains/Holding/Basic Prop Chain"))]
	public class HoldingBasicPropChain : HandlerChain
	{
		public DragAndDropHandler m_dragAndDropHandler;

		new void OnEnable( )
		{
			base.OnEnable( );

			AddHandler( m_dragAndDropHandler, HandlerExecutionMode.Blocking );
		}
	}

}