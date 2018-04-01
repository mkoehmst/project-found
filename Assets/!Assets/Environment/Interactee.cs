using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using ProjectFound.Environment.Handlers;
using ProjectFound.Environment.Characters;

namespace ProjectFound.Environment {


	public abstract class Interactee : MonoBehaviour
	{
		// TODO Decide if non-interactable inanimate objects might also have health

		// Bring health all the way down to the Interactee level because inanimate objects
		// can have health too, the amount of damage before they are destroyed
		[SerializeField] protected float m_maxHealthPoints = 100f;
		[SerializeField] protected float m_curHealthPoints = 1f;
		[SerializeField] protected string m_activateText = "Pickup";
		[SerializeField] protected string m_deactivateText = "Drop";
		[SerializeField] protected string m_ingameName = "Unknown";
		[SerializeField] protected bool m_isReceptive = true;
		[SerializeField] protected bool m_isActivated = false;
		[SerializeField] protected NodeCanvas.DialogueTrees.DialogueTree m_dialogueTree;
		[SerializeField] protected CommentSpec m_commentSpec;
		[SerializeField] protected InteracteeHandler m_handler;

		protected ActionType m_currentActionType = ActionType.None;

		public bool IsFocused { get; set; }

		public string ActivateText
		{
			get { return m_activateText; }
		}

		public string DeactivateText
		{
			get { return m_deactivateText; }
		}

		public string PromptText
		{
			get { return m_isActivated ? m_deactivateText : m_activateText; }
		}

		public string IngameName
		{
			get { return m_ingameName; }
		}

		public bool IsReceptive
		{
			get { return m_isReceptive; }
			set { m_isReceptive = value; }
		}

		public bool IsActivated
		{
			get { return m_isActivated; }
			set { m_isActivated = value; }
		}

		public NodeCanvas.DialogueTrees.DialogueTree DialogueTree
		{
			get { return m_dialogueTree; }
		}

		public CommentSpec CommentSpec
		{
			get { return m_commentSpec; }
		}

		public float HealthAsPercentage
		{
			get { return m_curHealthPoints / m_maxHealthPoints; }
		}

		public float Health
		{
			get { return m_curHealthPoints; }
		}

		new protected void Start( )
		{
			Debug.Assert( m_curHealthPoints > 0f, "Must start with positive health" );

			m_curHealthPoints = m_maxHealthPoints;
			IsFocused = false;
		}

		public void Use( )
		{
			StartCoroutine( m_handler.Use( this ) );
		}

		public abstract bool ValidateAction( ActionType actionType );
		public abstract void Reaction( );
	}


}
