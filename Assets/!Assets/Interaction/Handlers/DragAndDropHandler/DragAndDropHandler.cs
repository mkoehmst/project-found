namespace ProjectFound.Environment.Handlers
{

	using System.Collections;
	using System.Collections.Generic;
	using UnityEngine;

	using ProjectFound.Environment.Props;

	[CreateAssetMenu( menuName = ("Project Found/Handlers/Drag And Drop/Begin Drag And Drop") )]
	public class DragAndDropHandler : InteracteeHandler
	{
		public override IEnumerator Handle( Interactee ie, Interactor ir )
		{
			ir.HandlerExecutionDictionary[this] = true;

			Prop prop = ie as Prop;
			var raycaster = RaycastMaster.CurrentInteracteeRaycaster;
			var spec = ir.SelectionSpec;

			raycaster.IsEnabled = false;
			raycaster = RaycastMaster.CurrentInteracteeRaycaster =
				RaycastMaster.PropPlacementRaycaster;
			raycaster.IsEnabled = true;

			raycaster.AddBlacklistee( ie );
			UIMaster.RemovePrompt( prop );
			PlayerMaster.StartPropPlacement( prop, spec.gameObj, ref spec.hit );

			yield return null;

			while ( prop.ContinueDragAndDropChain == true )
			{
				PlayerMaster.PropPlacement( ref spec.hit );
				yield return null;
			}

			prop.IsFocused = false;
			PlayerMaster.EndPropPlacement( ref spec.hit );
			raycaster.RemoveBlacklistee( ie );
			ShaderMaster.ToggleSelectionOutline( prop.gameObject );

			raycaster.IsEnabled = false;
			raycaster = RaycastMaster.CurrentInteracteeRaycaster =
				RaycastMaster.CursorSelectionRaycaster;
			raycaster.IsEnabled = true;

			ir.HandlerExecutionDictionary[this] = false;

			yield break;
		}
	}

}
