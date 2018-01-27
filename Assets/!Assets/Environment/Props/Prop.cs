using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

namespace ProjectFound.Environment.Props {


	public class Prop : Interactee
	{
		[SerializeField] Sprite m_icon;
		[SerializeField] Mesh m_clearanceMesh;
		[SerializeField] protected PropDefinition m_definition;
		[SerializeField] protected PropHandler m_handler;

		public Sprite Icon { get { return m_icon; } }
		public Mesh ClearanceMesh { get { return m_clearanceMesh; } }
		public GameObject Prompt { get; set; }

		protected void Awake( )
		{
			Assert.IsNotNull( m_icon );
		}

		new protected void Start( )
		{
			base.Start( );

			m_handler.Initialize( this );
		}

		public void Activate( )
		{
			m_handler.Use( );
		}

		public void StartDragAndDrop( ref RaycastHit hit )
		{
			m_handler.DragAndDrop( ref hit );
		}

		public override bool ValidateAction( ActionType actionType )
		{
			/*switch ( actionType )
			{
				case ActionType.Activate:
					m_currentActionType = actionType;
					return true;
				default:
					m_currentActionType = ActionType.None;
					return false;
			}*/

			return true;
		}

		public override void Reaction( )
		{
			/*switch ( m_currentActionType )
			{
				case ActionType.Activate:
					Debug.Log( "Prop has been activated!" );
					if ( Animator != null )
						Animator.SetTrigger( m_activateTrigger );
					m_isReceptive = false;
					break;
			}*/
		}
	}


}
