using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ProjectFound.Environment.Characters {

public abstract class Combatant : Character, IDamageable
{
	[SerializeField] int m_startingActionPoints;

	protected System.Random m_rng = new System.Random( );

	public bool IsInCombat { get; set; } = false;
	public bool IsActiveCombatant { get; set; } = false;

	protected Combatant m_target = null;
	public Combatant Target
	{
		get { return m_target; }
	}

	protected int m_initiative = 1;
	public int initiative
	{
		get { return m_initiative; }
		set { m_initiative = value; }
	}

	public int ActionPoints { get; private set; }
	public float MovementScore { get; private set; } = 2.0f;

	public System.Action<Combatant> DelegateCombatHandler { get; set; }

	new protected void Start( )
	{
		base.Start( );

		ActionPoints = m_startingActionPoints;
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