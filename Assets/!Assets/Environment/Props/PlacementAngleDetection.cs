using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace ProjectFound.Environment.Props
{


	public class PlacementAngleDetection : MonoBehaviour
	{



		// Use this for initialization
		void Start () {

		}

		// Update is called once per frame
		void Update ()
		{
			Ray ray = new Ray( transform.position, new Vector3( 0f, -1f, 0f ) );
			RaycastHit hit;

			bool success = Physics.Raycast( ray, out hit, 10f );
			if ( success == true )
			{
				cakeslice.Outline outline = GetComponent<cakeslice.Outline>( );

				if ( !Misc.Floater.GreaterThan( hit.normal.y, 0.8f ) )
				{
					outline.color = 0;
					outline.enabled = true;
				}
				else
				{
					outline.enabled = false;
				}
			}

		}
	}


}
