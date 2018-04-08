using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ProjectFound.Environment.Handlers
{

	[CreateAssetMenu( menuName = ("Project Found/Item Handler/Book Handler") )]
	public class BookHandler : ItemHandler
	{
		public override IEnumerator Activate( Interactee interactee )
		{
			yield return base.Activate( interactee );

			// Show book on screen
			//yield break;
		}
	}


}