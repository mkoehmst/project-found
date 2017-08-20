using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ProjectFound.Environment {

public abstract class Interactee : MonoBehaviour
{
	protected ActionType m_currentActionType = ActionType.None;

	// TODO Decide if non-interactable inanimate objects might also have health

	// Bring health all the way down to the Interactee level because inanimate objects
	// can have health too, the amount of damage before they are destroyed
	[SerializeField] protected float m_maxHealthPoints = 100f;

	protected float m_curHealthPoints = 1f;

	public float healthAsPercentage
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
