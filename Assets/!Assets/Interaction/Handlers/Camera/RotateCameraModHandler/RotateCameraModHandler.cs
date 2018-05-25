namespace ProjectFound.Environment.Handlers
{

	using System.Collections.Generic;

	using UnityEngine;

	[CreateAssetMenu(menuName = ("Project Found/Handlers/Camera/Rotate Camera Mod"))]
	public class RotateCameraModHandler : InteracteeHandler
	{
		public override IEnumerator<float> Handler( Interactee ie, Interactor ir )
		{
			while ( true )
			{ 
				float movement = InputMaster.CheckAxis( "MouseCameraRotation" );
				if ( movement != 0f )
				{
					CameraMaster.FixedTiltZoomableCamera.HandleRotation( movement );
				}

				yield return MEC.Timing.WaitForOneFrame;
			}
		}
	}

}