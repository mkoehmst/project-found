namespace ProjectFound.Environment.Handlers
{ 

	using UnityEngine;

	[CreateAssetMenu(menuName=("Project Found/Handler Chains/Camera/Holding Camera Mod"))]
	public class HoldingCameraModChain : HandlerChain 
	{
		public RotateCameraModHandler m_rotateCameraModHandler;

		new void OnEnable( )
		{
			base.OnEnable( );

			base.AddHandler( m_rotateCameraModHandler, HandlerExecutionMode.Blocking );
		}
	}

}
