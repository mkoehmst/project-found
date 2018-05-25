namespace ProjectFound.Environment.Handlers
{

	using System.Collections.Generic;
	
	using UnityEngine;

	using ProjectFound.CameraUI;

	[CreateAssetMenu(menuName=("Project Found/Handlers/Comment Handler"))]
	public class CommentHandler : InteracteeHandler
	{
		public override IEnumerator<float> Handler( Interactee ie, Interactor ir )
		{
			List<string> comments = ie.CommentSpec.m_comments;

			int index = Random.Range( 0, comments.Count );

			string comment = comments[index];

			GameObject displayPrefab = ie.CommentSpec.m_displayPrefab;
			GameObject display = GameObject.Instantiate( displayPrefab, ie.transform );

			yield return MEC.Timing.WaitForOneFrame;

			InGameCommentUI igcUI = display.GetComponentInChildren<InGameCommentUI>( );
			igcUI.WorldAnchor = ie.transform;

			yield return MEC.Timing.WaitUntilDone( igcUI.DisplayComment( comment ) );

			while ( igcUI.IsVisible( ) )
			{
				yield return MEC.Timing.WaitForOneFrame;
			}

			GameObject.Destroy( display );
		}
	}

}
