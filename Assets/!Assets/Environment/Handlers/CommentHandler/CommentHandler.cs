using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using ProjectFound.CameraUI;

namespace ProjectFound.Environment.Handlers
{


	[CreateAssetMenu(menuName=("Project Found/Handlers/Comment Handler"))]
	public class CommentHandler : InteracteeHandler
	{


		public override IEnumerator Activate( Interactee i )
		{
			List<string> comments = i.CommentSpec.m_comments;

			int index = Random.Range( 0, comments.Count );

			string comment = comments[index];

			GameObject displayPrefab = i.CommentSpec.m_displayPrefab;
			GameObject display = GameObject.Instantiate( displayPrefab, i.transform );

			yield return null;

			InGameCommentUI igcUI = display.GetComponentInChildren<InGameCommentUI>( );
			igcUI.WorldAnchor = i.transform;

			yield return igcUI.DisplayComment( comment );

			while ( igcUI.IsVisible( ) )
			{
				yield return null;
			}

			GameObject.Destroy( display );
		}
	}


}
