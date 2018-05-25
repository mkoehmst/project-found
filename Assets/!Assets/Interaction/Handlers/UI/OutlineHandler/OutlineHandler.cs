namespace ProjectFound.Environment.Handlers
{ 

	using System.Collections;
	using System.Collections.Generic;
	using UnityEngine;

	[CreateAssetMenu(menuName = ("Project Found/Handlers/UI/Outline Handler"))]
	public class OutlineHandler : InteracteeHandler 
	{
		void OnEnable( )
		{
			DelegateHandlerRelease = HandlerRelease;
			DelegateHandlerAbort = HandlerRelease;
		}

		public override IEnumerator<float> Handler( Interactee ie, Interactor ir )
		{
			GameObject gameObj = ie.gameObject;

			ShaderMaster.SetFocusOutlineActive( gameObj, true );

			// Want it shown for at least one frame
			//yield return MEC.Timing.WaitForOneFrame;

			yield break;
		}

		public override IEnumerator<float> HandlerRelease( Interactee ie, Interactor ir )
		{
			GameObject gameObj = ie.gameObject;

			ShaderMaster.SetFocusOutlineActive( gameObj, false );

			yield break;
		}
	}

}
