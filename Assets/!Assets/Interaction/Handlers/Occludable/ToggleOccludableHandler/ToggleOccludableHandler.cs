namespace ProjectFound.Environment.Handlers
{

	using System.Collections.Generic;

	using UnityEngine;

	using ProjectFound.Environment.Occlusion;

	[CreateAssetMenu( menuName = ("Project Found/Handlers/Occludable/Toggle Occludable") )]
	public class ToggleOccludableHandler : InteracteeHandler
	{
		private void OnEnable( )
		{
			DelegateHandlerRelease = HandlerRelease;
		}

		public override IEnumerator<float> Handler( Interactee ie, Interactor ir )
		{
			Occludable occludable = ie as Occludable;

			PlayerMaster.OccludedFromCamera = true;

			occludable.Hide( );

			var raycaster = RaycastMaster.CameraOcclusionRaycaster;

			raycaster.RemovePriority( LayerID.Roof );
			raycaster.AddPriority( LayerID.RoofHidden );

			yield break;
		}

		public override IEnumerator<float> HandlerRelease( Interactee ie, Interactor ir )
		{
			Occludable occludable = ie as Occludable;

			PlayerMaster.OccludedFromCamera = false;

			occludable.Show( );

			var raycaster = RaycastMaster.CameraOcclusionRaycaster;

			raycaster.RemovePriority( LayerID.RoofHidden );
			raycaster.AddPriority( LayerID.Roof );

			yield break;
		}
	}

}