using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using ProjectFound.CameraUI;

namespace ProjectFound.Environment.Handlers
{


	[CreateAssetMenu(menuName=("Project Found/Handlers/Comment Handler"))]
	public class CommentHandler : InteracteeHandler
	{


		public override IEnumerator Handle( Interactee ie, Interactor ir )
		{
			List<string> comments = ie.CommentSpec.m_comments;

			int index = Random.Range( 0, comments.Count );

			string comment = comments[index];

			GameObject displayPrefab = ie.CommentSpec.m_displayPrefab;
			GameObject display = GameObject.Instantiate( displayPrefab, ie.transform );

			yield return null;

			InGameCommentUI igcUI = display.GetComponentInChildren<InGameCommentUI>( );
			igcUI.WorldAnchor = ie.transform;

			yield return igcUI.DisplayComment( comment );

			while ( igcUI.IsVisible( ) )
			{
				yield return null;
			}

			GameObject.Destroy( display );
		}
	}


}
