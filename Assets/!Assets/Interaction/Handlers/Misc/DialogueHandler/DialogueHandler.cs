namespace ProjectFound.Environment.Handlers
{

	using System.Collections.Generic;
	
	using UnityEngine;
	using NodeCanvas.DialogueTrees;

	[CreateAssetMenu(menuName=("Project Found/Handlers/Dialogue Handler"))]
	public class DialogueHandler : InteracteeHandler
	{
		protected DialogueTreeController m_dialogueTreeController;

		void OnEnable( )
		{
			if ( m_dialogueTreeController == null )
			{
				m_dialogueTreeController = FindObjectOfType<DialogueTreeController>( );
			}
		}

		void OnDisable( )
		{
			m_dialogueTreeController = null;
		}

		public override IEnumerator<float> Handler( Interactee ie, Interactor ir )
		{
			yield return MEC.Timing.WaitUntilDone( MovePlayerTowards( ie ) );

			//DialogueActor dialogueActor = interactee.GetComponent<DialogueActor>( );

			m_dialogueTreeController.graph = ie.DialogueTree;

			// Dialogue instigator is the Player
			m_dialogueTreeController.StartDialogue(
				PlayerMaster.Player.GetComponent<IDialogueActor>( ) );
		}
	}

}
