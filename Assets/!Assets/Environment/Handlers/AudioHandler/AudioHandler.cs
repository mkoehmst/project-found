using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace ProjectFound.Environment.Handlers
{


	[CreateAssetMenu(menuName=("Project Found/Handlers/Audio Handler"))]
	public class AudioHandler : InteracteeHandler
	{
		public override IEnumerator Activate( Interactee i )
		{
			Debug.Log( "AudioHandler!!" );
			yield break;
		}
	}


}
