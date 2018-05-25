namespace ProjectFound.Environment.Handlers
{

	using UnityEngine;

	[CreateAssetMenu(menuName=("Project Found/Handler Chains/Selection Basic Walkable Chain") )]
	public class SelectionBasicWalkableChain : HandlerChain
	{
		public ClickToMoveHandler m_clickToMoveHandler;

		new void OnEnable( )
		{
			base.OnEnable( );

			AddHandler( m_clickToMoveHandler, HandlerExecutionMode.Blocking );
		}
	}

}