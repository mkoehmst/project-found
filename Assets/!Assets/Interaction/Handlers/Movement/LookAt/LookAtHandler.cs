namespace ProjectFound.Interaction
{

	using System.Collections.Generic;

	using UnityEngine;
	using UnityEngine.Assertions;

	[CreateAssetMenu( menuName = ("Found/Handlers/Movement/Look At") )]
	public class LookAtHandler : InteracteeHandler
	{
		[SerializeField] float _degreesPerSecond = 180f;
		[SerializeField] bool _ignoreY = true;

		public override IEnumerator<float> Handler( Interactee ie, Interactor ir )
		{
			Assert.IsNotNull( ie );
			Assert.IsNotNull( ir );

			var renderer = ie.GetComponentInChildren<Renderer>( );
			Assert.IsNotNull( renderer );

			Vector3 lookFrom = ir.transform.position;
			Vector3 lookTo = renderer.bounds.center;
			if ( _ignoreY )
				lookFrom.y = lookTo.y;

			Quaternion start = ir.transform.rotation;
			Quaternion end = Quaternion.LookRotation( lookTo - lookFrom );
			float angle = Mathf.Abs(
					Quaternion.Angle( end, start ) );

			float lerpTrack = 0f;
			while ( lerpTrack < 1f )
			{
				lerpTrack += Time.deltaTime * _degreesPerSecond / angle;

				ir.transform.rotation = 
					Quaternion.Lerp( start, end, lerpTrack );

				yield return MEC.Timing.WaitForOneFrame;
			}

			yield break;
		}
	}

}