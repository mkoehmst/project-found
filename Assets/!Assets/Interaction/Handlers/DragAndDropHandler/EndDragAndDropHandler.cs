namespace ProjectFound.Environment.Handlers
{

	using System.Collections;
	using System.Collections.Generic;
	using UnityEngine;

	using ProjectFound.Environment.Props;

	[CreateAssetMenu( menuName = ("Project Found/Handlers/Drag And Drop/End Drag And Drop") )]
	public class EndDragAndDropHandler : PropHandler
	{
		public override IEnumerator HandleDragAndDrop( Prop prop, Interactor interactor )
		{
			var raycaster = RaycastMaster.CurrentInteracteeRaycaster;
			var spec = interactor.SelectionSpec;

			raycaster.RemoveBlacklistee( PlayerMaster.PropBeingPlaced );

			PlayerMaster.EndPropPlacement( ref spec.hit );

			raycaster.IsEnabled = false;

			raycaster = RaycastMaster.CurrentInteracteeRaycaster =
				RaycastMaster.CursorSelectionRaycaster;

			raycaster.IsEnabled = true;

			yield break;
		}
	}

}
