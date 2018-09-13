namespace ProjectFound.Interaction
{

	using System.Collections.Generic;

	using UnityEngine;

	using ProjectFound.Environment.Characters;

	[CreateAssetMenu(menuName=("Project Found/Handlers/Movement/Click To Move"))]
	public class ClickToMoveHandler : InteracteeHandler
	{
		public override IEnumerator<float> Handler( Interactee ie, Interactor ir )
		{
			Protagonist protagonist = ir as Protagonist;
			// Save out destination value type as Report will get updated every frame
			Vector3 destination = RaycastMaster.Report.HitPoint;

			protagonist.SetMovementTarget( ref destination );

			while ( protagonist.DistanceTo( ref destination ) > 1f )
			{
				yield return MEC.Timing.WaitForOneFrame;
			}

			//Debug.Log("ClickToMoveHandler.Handler() exited cleanly");

			yield break;
		}
	}

}