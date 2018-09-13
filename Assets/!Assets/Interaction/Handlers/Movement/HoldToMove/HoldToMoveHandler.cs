namespace ProjectFound.Interaction
{


	using System.Collections.Generic;

	using UnityEngine;

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

			yield break;
		}

		public override IEnumerator<float> WindowRelease( Interactee ie, Interactor ir )
		{
			RaycastMaster.SwitchToPrevious( );

			yield break;
		}

		public override IEnumerator<float> Handler( Interactee ie, Interactor ir )
		{
			ir.KillOneShotChain( );

			var report = RaycastMaster.Report;

			while ( true )
			{
				// "Dead zone" to reduce jerky movements
				if ( ir.DistanceTo( ref report.HitPoint ) > .025f )
				{ 
					PlayerMaster.Protagonist.SetMovementTarget( ref report.HitPoint );
				}

				yield return MEC.Timing.WaitForOneFrame;
			}
		}

		public override IEnumerator<float> HandlerRelease( Interactee ie, Interactor ir )
		{
			ir.KillHoldingChain( );

			RaycastMaster.SwitchToPrevious( );
			PlayerMaster.Protagonist.ResetMovementTarget( );

			//Debug.Log( "HoldToMoveHandler.HandlerRelease() exited gracefullly" );

			yield break;
		}
	}


}
