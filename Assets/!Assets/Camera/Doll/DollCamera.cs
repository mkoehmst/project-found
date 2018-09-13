namespace ProjectFound.CameraUI
{ 


	using UnityEngine;
	using UnityEngine.Assertions;

	public class DollCamera : MonoBehaviour 
	{
		[SerializeField] RenderTexture _renderTexture;
		[SerializeField] Transform _target;
		[SerializeField] float _heightOffset;

		public Camera UnityCamera { get; private set; }

		void Awake( )
		{
			Assert.IsNotNull( _target );

			UnityCamera = GetComponent<Camera>( );
			Assert.IsNotNull( UnityCamera );
		}

		void Start( )
		{
			UnityCamera.enabled = false;
		}

		void LateUpdate( )
		{
			Vector3 targetForward = _target.transform.forward;
			targetForward = new Vector3( targetForward.x, 0f, targetForward.z );
			targetForward.Normalize( );

			transform.position = _target.transform.position + (targetForward * 6f);
			transform.Translate( 0f, _heightOffset, 0f );
			transform.LookAt( _target.transform.position + new Vector3( 0f, _heightOffset, 0f ) );
		}

		public void Disable( )
		{
			UnityCamera.targetTexture = null;
			UnityCamera.enabled = false;
		}

		public void Enable( )
		{
			UnityCamera.targetTexture = _renderTexture;
			UnityCamera.enabled = true;
		}
	}


}