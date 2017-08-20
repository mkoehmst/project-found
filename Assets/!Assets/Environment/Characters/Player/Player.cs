using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

using ProjectFound.Environment.Items;

namespace ProjectFound.Environment.Characters {

public class Player : Combatant
{
	PlayerMovement m_movement = null;
	public List<Item> InventoryItems { get; private set; }

	new void Start( )
	{
		base.Start( );

		CombatEncounter.singleton.DelegateEncounterBegin += OnCombatEncounterBegin;
		CombatEncounter.singleton.DelegateRoundBegin += OnCombatRoundBegin;
		CombatEncounter.singleton.DelegateDeath += OnCombatDeath;

		m_initiative = 10;

		m_movement = GetComponent<PlayerMovement>( );
		InventoryItems = new List<Item>( );
	}

	public override IEnumerator ExecuteRoundActions( )
	{
		Vector3 target = m_target.transform.position;
		float distance = (target - transform.position).magnitude;

		if ( distance > 1f )
		{
			m_movement.MoveToTarget( target );
		}

		// TODO Figure out some kind of guaranteed end to this loop
		while ( (target - transform.position).magnitude > 1f )
		{
			yield return new WaitForSeconds( 0.1f );
		}

		float damageCaused = m_rng.Next( 15, 20 );
		m_target.TakeDamage( this, damageCaused );

		yield break;
	}

	public override bool Action( ActionType actionType, Interactee interactee )
	{
		// First ask the item being acted upon whether it allows this type of action
		if ( interactee.ValidateAction( actionType ) == true )
		{
			if ( actionType == ActionType.PickUp )
			{
				InventoryItems.Add( interactee as Item );
			}
			else if ( actionType == ActionType.UseItem )
			{
				InventoryItems.Remove( interactee as Item );
			}

			// Allow the item to react
			interactee.Reaction( );

			return true;
		}

		return false;
	}

	public override bool ValidateAction( ActionType actionType )
	{
		return true;
	}

	public override void Reaction( )
	{

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

	public void TestButton( )
	{
		Debug.Log( "BUTTON TEST!!" );
	}
}

}