using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

using ProjectFound.Environment.Props;

namespace ProjectFound.Environment.Characters
{


	public class Player : Combatant
	{
		CharacterMovement m_movement = null;
		public List<Item> InventoryItems { get; private set; }

		protected new void Start( )
		{
			base.Start( );

			CombatEncounter.singleton.DelegateEncounterBegin += OnCombatEncounterBegin;
			CombatEncounter.singleton.DelegateRoundBegin += OnCombatRoundBegin;
			CombatEncounter.singleton.DelegateDeath += OnCombatDeath;

			m_initiative = 10;

			m_movement = GetComponent<CharacterMovement>( );
			InventoryItems = new List<Item>( );
		}

		public override IEnumerator ExecuteRoundActions( )
		{
			while ( DelegateCombatHandler == null )
			{
				yield return new WaitForSeconds( 1.0f );
			}

			yield return DelegateCombatHandler( this );
			DelegateCombatHandler = null;
			//IsActiveCombatant = false;
			//yield break;


			/*

			Vector3 target = m_target.transform.position;
			float distance = (target - transform.position).magnitude;

			if ( distance > 1f )
			{
				m_movement.SetMoveTarget( target );
			}

			// TODO Figure out some kind of guaranteed end to this loop
			while ( (target - transform.position).magnitude > 1f )
			{
				yield return new WaitForSeconds( 0.1f );
			}

			float damageCaused = m_rng.Next( 15, 20 );
			m_target.TakeDamage( this, damageCaused );

			yield break;
			*/
		}

		private void OnCombatEncounterBegin( List<Combatant> combatants )
		{
			m_target = combatants[1];
		}

		private void OnCombatRoundBegin( List<Combatant> combatants )
		{
			// Placeholder
		}

		private void OnCombatDeath( Combatant deceased, List<Combatant> remainders )
		{
			if ( deceased != m_target )
				return ;

			foreach ( Combatant combatant in remainders )
			{
				if ( combatant != this )
				{
					m_target = combatant;
					return ;
				}
			}

			m_target = null;
		}
	}


}