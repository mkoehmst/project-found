using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ProjectFound.Environment.Characters
{


	// This script governs each individual combat encounter
	// Make it a singleton because we know only one combat encounter at a time max
	public class CombatEncounter : Misc.Singleton<CombatEncounter>
	{
		// Prevent code from creating additonal CombatEncounter instances
		protected CombatEncounter( ) { }

		public delegate void CombatBegin( List<Combatant> combatants );
		public delegate void CombatDeath( Combatant deceased, List<Combatant> remainders );

		List<Combatant> m_combatants		= null;
		int				m_combatRound		= 0;
		int				m_enemiesRemaining	= 0;


		public event CombatBegin DelegateEncounterBegin;
		public event CombatBegin DelegateRoundBegin;
		public event CombatDeath DelegateDeath;

		public Combatant ActiveCombatant { get; private set; }

		void Awake( )
		{
			m_combatants = new List<Combatant>( );
		}

		void Start( )
		{
			AddCombatant( FindObjectOfType<Player>( ) as Combatant );
		}

		void LateUpdate( )
		{
			if ( m_combatRound == 0 && m_combatants.Count > 1 )
				BeginEncounter( );
		}

		// TODO Co-routine to check for end-of-encounter (i.e. player dies or all enemies die)

		public void AddCombatant( Combatant combatant )
		{
			Debug.Assert( combatant != null, "Cannot add null combatant" );

			// Adding a combatant after encounter has begun
			// In other words, don't count the Player as an Enemy
			if ( m_combatRound != 0 )
				++m_enemiesRemaining;

			combatant.IsInCombat = true;
			m_combatants.Add( combatant );
		}

		public void RemoveCombatant( Combatant combatant )
		{
			Debug.Assert( combatant != null, "Canot remove null combatant" );

			m_combatants.Remove( combatant );

			DelegateDeath( combatant, m_combatants );

			if ( --m_enemiesRemaining == 0 )
				EndEncounter( );
		}

		public void BeginEncounter( )
		{
			Debug.Assert( m_combatants.Count > 1 );
			Debug.Assert( m_combatants[0] != null );
			Debug.Assert( m_combatRound == 0 );
			Debug.Assert( m_enemiesRemaining == 0 );

			m_enemiesRemaining = m_combatants.Count - 1;

			StartCoroutine( BeginRound( ) );

			DelegateEncounterBegin( m_combatants );
		}

		public void EndEncounter( )
		{
			m_enemiesRemaining = 0;
			m_combatRound = 0;
		}

		private IEnumerator BeginRound( )
		{
			++m_combatRound;

			DetermineAttackOrder( );
			DelegateRoundBegin( m_combatants );

			yield return new WaitForSeconds( 2f );

			// Do NOT use foreach because the iteration gets screwed if a Combatant is removed
			for ( int i = 0; i < m_combatants.Count; ++i )
			{
				ActiveCombatant = m_combatants[i];
				ActiveCombatant.IsActiveCombatant = true;
				StartCoroutine( ActiveCombatant.ExecuteRoundActions( ) );
				yield return new WaitForSeconds( 1.5f );
			}

			EndRound( );

			yield break;
		}

		private void EndRound( )
		{
			ActiveCombatant = null;

			// Temporarily cap round count at 10 to prevent infinite looping
			if ( m_enemiesRemaining > 0 && m_combatRound < 10 )
			{
				StartCoroutine( BeginRound( ) );
			}
			else
			{
				EndEncounter( );
			}
		}

		private void DetermineAttackOrder( )
		{
			// Beautiful...delegates and lambda expressions sure are neat!
			// Flipped the x & y order to get descending sort
			m_combatants.Sort( ( x, y ) => y.initiative.CompareTo( x.initiative ) );
		}
	}


}