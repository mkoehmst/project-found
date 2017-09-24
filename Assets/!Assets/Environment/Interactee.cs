using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ProjectFound.Environment {


	public abstract class Interactee : MonoBehaviour
	{
		// TODO Decide if non-interactable inanimate objects might also have health

		// Bring health all the way down to the Interactee level because inanimate objects
		// can have health too, the amount of damage before they are destroyed
		[SerializeField] protected float m_maxHealthPoints = 100f;
		[SerializeField] protected string m_promptText = "Pickup";

		protected ActionType m_currentActionType = ActionType.None;
		protected float m_curHealthPoints = 1f;

		public string PromptText
		{
			get { return m_promptText; }
		}

		public float HealthAsPercentage
		{
			get { return m_curHealthPoints / m_maxHealthPoints; }
		}

		protected void Start( )
		{
			Debug.Assert( m_curHealthPoints > 0f, "Must start with positive health" );

			m_curHealthPoints = m_maxHealthPoints;
		}

		public abstract bool ValidateAction( ActionType actionType );
		public abstract void Reaction( );
	}


}
