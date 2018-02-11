using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using ProjectFound.Environment.Props;
using NodeCanvas.DialogueTrees;

namespace ProjectFound.Environment.Handlers
{

	[CreateAssetMenu(menuName=("Project Found/Handlers/Dialogue Handler"))]
	public class DialogueHandler : InteracteeHandler
	{
		protected NodeCanvas.DialogueTrees.DialogueTreeController m_dialogueTreeController;

		void OnEnable( )
		{
			if ( m_dialogueTreeController == null )
			{
				m_dialogueTreeController = FindObjectOfType<DialogueTreeController>( );
			}
		}

		public override void Use( Interactee interactee )
		{
			DialogueActor dialogueActor = interactee.GetComponent<DialogueActor>( );

			//interactee.DialogueTree.SetActorReference( dialogueActor.name, dialogueActor );
			m_dialogueTreeController.graph = interactee.DialogueTree;
			m_dialogueTreeController.StartDialogue(
				PlayerMaster.Player.GetComponent<IDialogueActor>( ) );
		}
	}


}
