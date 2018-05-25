namespace ProjectFound.Environment.Handlers
{

	using UnityEngine;

	[CreateAssetMenu( menuName = ("Project Found/Handler Chains/Camera/OneShot Camera Attach") )]
	public class OneShotCameraAttachChain : HandlerChain
	{
		public AttachCameraHandler m_attachCameraHandler;

		new void OnEnable( )
		{
			base.OnEnable( );

			base.AddHandler( m_attachCameraHandler, HandlerExecutionMode.Blocking );
		}
	}

}
