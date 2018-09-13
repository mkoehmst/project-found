namespace ProjectFound.Core.Master 
{


	using UnityEngine;
	using UnityEngine.Assertions;

	using ProjectFound.CameraUI;

	public class CameraMaster
	{
		public FixedTiltZoomableCamera Camera { get; private set; }
		public Camera UnityCamera { get; private set; }

		public CameraMaster( )
		{
			Camera = GameObject.FindObjectOfType<FixedTiltZoomableCamera>( );
			UnityCamera = Camera.GetComponentInChildren<Camera>( );

			Assert.IsNotNull( Camera );
			Assert.IsNotNull( UnityCamera );
		}

		public void Loop( )
		{
		}
	}


}
