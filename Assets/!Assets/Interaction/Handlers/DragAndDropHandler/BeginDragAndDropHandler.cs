namespace ProjectFound.Environment.Handlers
{

	using System.Collections;
	using System.Collections.Generic;
	using UnityEngine;

	using ProjectFound.Environment.Props;

	[CreateAssetMenu( menuName = ("Project Found/Handlers/Drag And Drop/Begin Drag And Drop") )]
	public class BeginDragAndDropHandler : PropHandler
	{
		public override IEnumerator HandleDragAndDrop( Prop prop, Interactor interactor )
		{
			var raycaster = RaycastMaster.CurrentInteracteeRaycaster;
			var spec = interactor.SelectionSpec;

			if ( prop.IsReceptive == false || prop.IsDraggable == false )
			{
				yield break;
			}

			RemoveFocus( prop );
			raycaster.IsEnabled = false;
			raycaster = RaycastMaster.CurrentInteracteeRaycaster =
				RaycastMaster.PropPlacementRaycaster;
			raycaster.IsEnabled = true;
			raycaster.AddBlacklistee( prop );
			PlayerMaster.StartPropPlacement( prop, spec.gameObj, ref spec.hit );

			yield break;
		}
	}

}
