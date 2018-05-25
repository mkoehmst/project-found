namespace ProjectFound.Environment.Handlers
{

	using UnityEngine;

	[CreateAssetMenu(menuName = ("Project Found/Handler Chains/Holding/Basic Walkable Chain"))]
	public class HoldingBasicWalkableChain : HandlerChain
	{
		public HoldToMoveHandler m_holdToMoveHandler;

		new void OnEnable()
		{
			base.OnEnable();

			AddHandler(m_holdToMoveHandler, HandlerExecutionMode.Blocking);
		}
	}

}
