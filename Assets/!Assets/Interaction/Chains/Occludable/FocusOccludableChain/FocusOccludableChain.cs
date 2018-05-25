namespace ProjectFound.Environment.Handlers
{

	using UnityEngine;

	[CreateAssetMenu(menuName=("Project Found/Handler Chains/Occludable/Focus Occludable"))]
	public class FocusOccludableChain : HandlerChain
	{
		public ToggleOccludableHandler m_toggleOccludableHandler;

		new void OnEnable( )
		{
			base.OnEnable( );

			base.AddHandler( m_toggleOccludableHandler, HandlerExecutionMode.Blocking );
		}
	}

}
