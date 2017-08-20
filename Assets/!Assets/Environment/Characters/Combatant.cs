using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ProjectFound.Environment.Characters {

public abstract class Combatant : Character, IDamageable
{
	protected System.Random m_rng = new System.Random( );

	protected bool m_isAggro = false;
	protected bool m_isAttacking = false;

	protected Combatant m_target = null;

	protected int m_initiative = 1;
	public int initiative
	{
		get { return m_initiative; }
		set { m_initiative = value; }
	}

	// Player and Enemy have very different implementations so mark abstract
	public abstract IEnumerator ExecuteRoundActions( );

	// ********************************************************************************************
	// ** IDamageable
	// ********************************************************************************************
	public void TakeDamage( IDamageable attacker, float damage )
	{
		Debug.Log( attacker + " does " + damage + " damage to " + this );

		float computedHealthPoints = m_curHealthPoints - damage;

		if ( computedHealthPoints <= 0f )
		{
			if ( gameObject.tag != "Player" )
				Destroy( gameObject );

			CombatEncounter.singleton.RemoveCombatant( this );

			// TODO figure out what to do on Player death
		}
		else
		{
			m_curHealthPoints = computedHealthPoints;
		}
	}
	// ********************************************************************************************
}

}