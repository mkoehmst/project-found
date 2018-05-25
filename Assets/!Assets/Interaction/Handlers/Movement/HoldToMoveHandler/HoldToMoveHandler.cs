namespace ProjectFound.Environment.Handlers
{

	using System.Collections.Generic;

	using UnityEngine;

	using ProjectFound.Master;

	[CreateAssetMenu( menuName = ("Project Found/Handlers/Movement/Hold To Move") )]
	public class HoldToMoveHandler : InteracteeHandler
	{
		void OnEnable()
		{
			DelegateWindow = Window;
			DelegateWindowRelease = WindowRelease;
			DelegateWindowAbort = WindowRelease;
			DelegateHandlerRelease = HandlerRelease;
			DelegateHandlerAbort = HandlerRelease;
		}

		public override IEnumerator<float> Window( Interactee ie, Interactor ir )
		{
			RaycastMaster.SwitchTo( RaycastMaster.HoldToMoveRaycaster );

			//Debug.Log( "HoldToMoveHandler.Window() exited gracefully." );

			yield break;
		}

		public override IEnumerator<float> WindowRelease( Interactee ie, Interactor ir )
		{
			RaycastMaster.SwitchToPrevious( );

			//Debug.Log( "HoldToMoveHandler.WindowRelease() exited gracefully." );

			yield break;
		}

		public override IEnumerator<float> Handler( Interactee ie, Interactor ir )
		{
			var report = RaycastMaster.Report;

			while ( true )
			{
				PlayerMaster.MoveTo( ref report.hitPoint );

				yield return MEC.Timing.WaitForOneFrame;
			}
		}

		public override IEnumerator<float> HandlerRelease( Interactee ie, Interactor ir )
		{
			RaycastMaster.SwitchToPrevious( );
			PlayerMaster.StopMoving( );

			//Debug.Log( "HoldToMoveHandler.HandlerRelease() exited gracefullly" );

			yield break;
		}
	}

}
