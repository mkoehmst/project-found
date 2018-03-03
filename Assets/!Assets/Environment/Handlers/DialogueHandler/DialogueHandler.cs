using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using NodeCanvas.DialogueTrees;

namespace ProjectFound.Environment.Handlers
{

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

		public override IEnumerator Use( Interactee interactee )
		{
			yield return MovePlayerTowards( interactee );

			//DialogueActor dialogueActor = interactee.GetComponent<DialogueActor>( );

			m_dialogueTreeController.graph = interactee.DialogueTree;

			// Dialogue instigator is the Player
			m_dialogueTreeController.StartDialogue(
				PlayerMaster.Player.GetComponent<IDialogueActor>( ) );
		}
	}


}
