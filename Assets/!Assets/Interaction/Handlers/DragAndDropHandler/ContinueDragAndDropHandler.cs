namespace ProjectFound.Environment.Handlers
{

	using System.Collections;
	using System.Collections.Generic;
	using UnityEngine;

	using ProjectFound.Environment.Props;

	[CreateAssetMenu( menuName = ("Project Found/Handlers/Drag And Drop/Continue Drag And Drop") )]
	public class ContinueDragAndDropHandler : PropHandler
	{
		public override IEnumerator HandleDragAndDrop( Prop prop, Interactor interactor )
		{
			SelectionSpec spec = interactor.SelectionSpec;

			PlayerMaster.PropPlacement( ref spec.hit );

			yield break;
		}
	}

}
