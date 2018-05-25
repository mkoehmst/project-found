namespace ProjectFound.Environment.Handlers
{

	using System.Collections.Generic;

	using UnityEngine;

	[CreateAssetMenu( menuName = ("Project Found/Handlers/Camera/Attach Camera") )]
	public class AttachCameraHandler : InteracteeHandler
	{
		public override IEnumerator<float> Handler( Interactee ie, Interactor ir )
		{
			string[] moveAxii =
				{ "ControllerMovementHorizontal", "ControllerMovementVertical" };
			string cameraH = "ControllerCameraHorizontal";
			string cameraV = "ControllerCameraVertical";

			if ( CameraMaster.FixedTiltZoomableCamera.IsAttached )
			{
				CameraMaster.FixedTiltZoomableCamera.HandleDetachment( );

				InputMaster.DisableMap( moveAxii );
				InputMaster.EnableMap( cameraH );
				InputMaster.EnableMap( cameraV );
			}
			else
			{
				CameraMaster.FixedTiltZoomableCamera.HandleAttachment( );

				InputMaster.EnableMap( moveAxii );
				InputMaster.DisableMap( cameraH );
				InputMaster.DisableMap( cameraV );
			}

			yield break;
		}
	}

}