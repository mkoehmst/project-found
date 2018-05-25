using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using ProjectFound.Environment.Handlers;
using ProjectFound.Environment.Characters;

namespace ProjectFound.Environment {


	public class Interactee : MonoBehaviour
	{
		// TODO Decide if non-interactable inanimate objects might also have health

		// Bring health all the way down to the Interactee level because inanimate objects
		// can have health too, the amount of damage before they are destroyed
	//	[UnityEditor.]
		[SerializeField] protected string m_ingameName = "Unknown";
		[SerializeField] protected float m_maxHealthPoints = 100f;
		[SerializeField] protected float m_curHealthPoints = 1f;
		[SerializeField] protected string m_activateText = "Pickup";
		[SerializeField] protected string m_deactivateText = "Drop";
		[SerializeField] protected bool m_isActivated = false;
		[SerializeField] protected NodeCanvas.DialogueTrees.DialogueTree m_dialogueTree;
		[SerializeField] protected CommentSpec m_commentSpec;



		[SerializeField] protected HandlerChain m_oneShotChain;
		public HandlerChain OneShotChain { get { return m_oneShotChain; } }

		[SerializeField] protected HandlerChain m_oneShotReleaseChain;
		public HandlerChain OneShotReleaseChain {  get { return m_oneShotReleaseChain; } }

		[SerializeField] protected HandlerChain m_windowChain;
		public HandlerChain WindowChain { get { return m_windowChain; } }

		[SerializeField] protected HandlerChain m_windowReleaseChain;
		public HandlerChain WindowReleaseChain { get { return m_windowReleaseChain; } }

		[SerializeField] protected HandlerChain m_holdingChain;
		public HandlerChain HoldingChain { get { return m_holdingChain; } }

		[SerializeField] protected HandlerChain m_holdingReleaseChain;
		public HandlerChain HoldingReleaseChain { get { return m_holdingReleaseChain; } }

		[SerializeField] protected HandlerChain m_focusChain;
		public HandlerChain FocusChain { get { return m_focusChain; } }

		[SerializeField] protected HandlerChain m_focusReleaseChain;
		public HandlerChain FocusReleaseChain { get { return m_focusReleaseChain; } }

		[SerializeField] protected HandlerChain m_usageChain;
		public HandlerChain UsageChain { get { return m_usageChain; } }

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

		public LayerID LayerID
		{
			get { return (LayerID)gameObject.layer; }
		}

		protected void Start( )
		{
			Debug.Assert( m_curHealthPoints > 0f, "Must start with positive health" );

			m_curHealthPoints = m_maxHealthPoints;
			IsFocused = false;
		}


	}


}
