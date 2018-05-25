namespace ProjectFound.Environment.Handlers
{

	using System.Collections.Generic;
	
	using UnityEngine;
	
	using ProjectFound.Environment.Props;

	[CreateAssetMenu(menuName = ("Project Found/Handlers/UI/Prompt Handler"))]
	public class PromptHandler : InteracteeHandler
	{
		void OnEnable( )
		{
			DelegateHandlerRelease = HandlerRelease;
			DelegateHandlerAbort = HandlerRelease;
		}

		public override IEnumerator<float> Handler( Interactee ie, Interactor ir )
		{
			Prop prop = ie as Prop;

			KeyCode key = InputMaster.GetKeyFromAction( GameContext.OnCursorSelect );
			UIMaster.DisplayPrompt( prop, key, RaycastMaster.Report.hitPoint );

			// Want it shown for at least one frame
			//yield return MEC.Timing.WaitForOneFrame;

			yield break;
		}

		public override IEnumerator<float> HandlerRelease( Interactee ie, Interactor ir )
		{
			Prop prop = ie as Prop;

			UIMaster.RemovePrompt( prop );

			yield break;
		}
	}

}
