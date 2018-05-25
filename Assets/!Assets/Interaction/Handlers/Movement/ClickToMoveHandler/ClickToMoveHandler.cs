namespace ProjectFound.Environment.Handlers
{

	using System.Collections.Generic;

	using UnityEngine;

	using ProjectFound.Master;

	[CreateAssetMenu(menuName=("Project Found/Handlers/Movement/Click To Move"))]
	public class ClickToMoveHandler : InteracteeHandler
	{
		public override IEnumerator<float> Handler( Interactee ie, Interactor ir )
		{
			var report = RaycastMaster.Report;

			PlayerMaster.MoveTo( ref report.hitPoint );

			//Debug.Log("ClickToMoveHandler.Handler() exited gracefully");

			yield break;
		}
	}

}