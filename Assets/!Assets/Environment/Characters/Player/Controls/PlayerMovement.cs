using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityStandardAssets.Characters.ThirdPerson;

[RequireComponent( typeof( NavMeshAgent ) )]
[RequireComponent( typeof( ThirdPersonCharacter ) )]
[RequireComponent( typeof( AICharacterControl ) )]
public class PlayerMovement : MonoBehaviour
{
	AICharacterControl m_ai	= null;
	GameObject m_walkTarget	= null;

	void Awake( )
	{
		m_walkTarget = new GameObject( "Walk Target" );
	}

    void Start( )
    {
		m_ai = GetComponent<AICharacterControl>( );

		CombatEncounter.singleton.DelegateEncounterBegin += OnCombatEncounterBegin;
    }

	public void MoveToTarget( Vector3 destination )
	{
		m_walkTarget.transform.position = destination;
		m_ai.SetTarget( m_walkTarget.transform );
	}

	public void StopMovement( )
	{
		m_walkTarget.transform.position = transform.position;
		m_ai.SetTarget( transform );
	}

	public void HandleDirectMovement( float h, float v )
	{
		if ( h != 0f || v != 0f )
		{
			Transform xform = Camera.main.transform;

			Vector3 camForward = Vector3.Scale( xform.forward, new Vector3( 1, 0, 1 ) ).normalized;
			Vector3 movement = v * camForward + h * xform.right;

			m_walkTarget.transform.position = transform.position + movement;

			m_ai.SetTarget( m_walkTarget.transform );
		}
		else
		{
			m_ai.SetTarget( transform );
		}
	}

	private void OnCombatEncounterBegin( List<Combatant> combatants )
	{
		StopMovement( );
	}

	private void OnDrawGizmos( )
	{
		if ( m_walkTarget != null )
		{
			Gizmos.color = Color.black;
			Gizmos.DrawLine( transform.position, m_walkTarget.transform.position );
			Gizmos.DrawSphere( m_walkTarget.transform.position, 0.08f );
		}
	}
}