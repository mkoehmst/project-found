namespace ProjectFound.CameraUI
{

	using System.Collections.Generic;

	public class InGameCommentUI : FadeInFadeOutTextUI
	{

		public IEnumerator<float> DisplayComment( string comment )
		{
			m_tmPro.SetText( comment );

			yield return
				MEC.Timing.WaitUntilDone( FadeInThenOut( 0f, 0.3333f, 1f, 3f, 2f, 0f ) );
		}
	}

}