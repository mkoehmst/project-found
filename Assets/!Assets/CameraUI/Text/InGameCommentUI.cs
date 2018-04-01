using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

namespace ProjectFound.CameraUI
{


	public class InGameCommentUI : FadeInFadeOutTextUI
	{

		public IEnumerator DisplayComment( string comment )
		{
			m_tmPro.SetText( comment );

			yield return FadeInThenOut( 0f, 0.3333f, 1f, 3f, 2f, 0f );
		}
	}


}