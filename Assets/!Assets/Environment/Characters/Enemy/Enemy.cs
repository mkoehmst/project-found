using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using NodeCanvas.BehaviourTrees;

namespace ProjectFound.Environment.Characters {

public class Enemy : Combatant
{
	[SerializeField] EnemyDefinition m_enemyDefinition;

	Transform m_player = null;

	void Awake( )
	{
		m_player = FindObjectOfType<Player>( ).transform;
	}

	new void Start( )
	{
		base.Start( );

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
		BehaviourTreeOwner bto = GetComponent<BehaviourTreeOwner>( );
		bto.Tick( );

		Skill skill = bto.blackboard.GetVariable<Skill>( "_skill" ).value;

		yield return skill.Handle( this );

		//float damageCaused = m_rng.Next( 12, 15 );
		//m_target.TakeDamage( this, damageCaused );
		//IsActiveCombatant = false;

		//yield break;
	}

	private void OnCombatEncounterBegin( List<Combatant> combatants )
	{
		m_target = combatants[0];
	}

	private void AggroCheck( float distance )
	{
		if ( IsInCombat == false && distance < m_enemyDefinition.m_aggroRadius )
		{
			CombatEncounter.singleton.AddCombatant( this as Combatant );
		}
	}

	private void OnDrawGizmos( )
	{
		Gizmos.color = Color.red;
		Gizmos.DrawWireSphere( transform.position, m_enemyDefinition.m_aggroRadius );
	}
}

}