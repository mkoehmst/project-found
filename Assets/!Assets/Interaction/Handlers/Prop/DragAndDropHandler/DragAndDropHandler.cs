namespace ProjectFound.Environment.Handlers
{

	using System.Collections.Generic;
	using UnityEngine;

	using ProjectFound.Environment.Props;
	using ProjectFound.Master;

	[CreateAssetMenu( menuName = ("Project Found/Handlers/Drag And Drop") )]
	public class DragAndDropHandler : InteracteeHandler
	{
		void OnEnable( )
		{
			DelegateWindow = Window;
			DelegateWindowRelease = WindowRelease;
			DelegateWindowAbort = WindowRelease;
			DelegateHandlerRelease = HandlerRelease;
			DelegateHandlerAbort = HandlerRelease;
		}

		public override IEnumerator<float> Window( Interactee ie, Interactor ir )
		{
			//Debug.Log( "DropAndDropHandler.Window()" );
			var raycaster = RaycastMaster.SwitchTo( RaycastMaster.PropPlacementRaycaster );
			raycaster.AddBlacklistee( ie );
			PlayerMaster.SetPropBeingPlaced( ie as Prop );

			yield break;
		}

		public override IEnumerator<float> WindowRelease( Interactee ie, Interactor ir )
		{
			//Debug.Log( "DragAndDropHandler.WindowRelease()" );
			RaycastMaster.SwitchToPrevious( );
			PlayerMaster.SetPropBeingPlaced( null );

			yield break;
		}

		public override IEnumerator<float> Handler( Interactee ie, Interactor ir )
		{
			//Debug.Log( "DragAndDropHandler.Handler()" );
			Prop prop = ie as Prop;
			var raycaster = RaycastMaster.CurrentInteracteeRaycaster;
			var report = raycaster.Report;

			UIMaster.RemovePrompt( prop );
			PlayerMaster.PreparePropPlacement( );

			while ( true )
			{
				PlayerMaster.PropPlacement( ref report.hitPoint, ref report.hitNormal );

				yield return MEC.Timing.WaitForOneFrame;
			}
		}

		public override IEnumerator<float> HandlerRelease( Interactee ie, Interactor ir )
		{
			Prop prop = ie as Prop;

			//prop.IsFocused = false;
			PlayerMaster.EndPropPlacement( );
			ShaderMaster.SetFocusOutlineActive( prop.gameObject, false );
			RaycastMaster.SwitchToPrevious( );

			yield break;
		}
	}

}
