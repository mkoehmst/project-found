using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ProjectFound.Environment.Characters {

public class Enemy : Combatant
{
	[SerializeField] float m_aggroRadius = 6f;

	AICharacterControl m_ai = null;
	Transform m_player = null;

	void Awake( )
	{
		m_ai = GetComponent<AICharacterControl>( );
		m_player = FindObjectOfType<Player>( ).transform;
	}

	new void Start( )
	{
		base.Start( );

		Debug.Assert( m_ai != null, "No AICharacterControl component" );
		Debug.Assert( m_player != null, "No GameObject with Player component found" );

		CombatEncounter.singleton.DelegateEncounterBegin += OnCombatEncounterBegin;
	}

	// Check distances in LateUpdate to make sure all Transforms are up-to-date
	void LateUpdate( )
	{
		// TODO Aggro check should account for enemy's line-of-sight

		float distance = (m_player.position - transform.position).magnitude;

		AggroCheck( distance );
	}

	public override IEnumerator ExecuteRoundActions( )
	{
		float damageCaused = m_rng.Next( 12, 15 );
		m_target.TakeDamage( this, damageCaused );

		yield break;
	}

	public override bool ValidateAction( ActionType actionType )
	{
		return true;
	}

	public override bool Action( ActionType actionType, Interactee interactee )
	{
		return true;
	}

	public override void Reaction( )
	{

	}

	private void OnCombatEncounterBegin( List<Combatant> combatants )
	{
		m_target = combatants[0];
	}

	private void AggroCheck( float distance )
	{
		if ( m_isAggro == false && distance < m_aggroRadius )
		{
			m_isAggro = true;

			CombatEncounter.singleton.AddCombatant( this as Combatant );
		}
	}

	private void OnDrawGizmos( )
	{
		Gizmos.color = Color.red;
		Gizmos.DrawWireSphere( transform.position, m_aggroRadius );
	}
}

}