using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace ProjectFound.Environment.Props
{


	public class PlacementCursorOffset : MonoBehaviour
	{
		public Vector3 Offset { get; private set; }

		public void InitialCursorHit( Vector3 cursorPos )
		{
			Offset = new Vector3(
				cursorPos.x - transform.position.x, 0f, cursorPos.z - transform.position.z );
		}
	}


}
