namespace ProjectFound.Environment.Characters
{


	using System.Collections.Generic;

	using UnityEngine;
	using UnityEngine.Assertions;
	using Autelia.Serialization;

	public class Protagonist : Combatant
	{
		private ProtagonistController _controller;

		//public List<Item> InventoryItems { get; private set; }

		public override void OnBeginCombatEncounter( )
		{
			KillOneShotChain( );
		}

		public override void OnBeginCombatTurn( )
		{
			//base.OnBeginCombatTurn( ); )
		}

		new protected void Awake( )
		{
			base.Awake( );

			LayerID = LayerID.Protagonist;

			if ( Serializer.IsLoading ) return;

			_controller = GetComponent<ProtagonistController>( );
			Assert.IsNotNull( _controller );
		}

		new protected void Start( )
		{
			base.Start( );

			if ( Serializer.IsLoading ) return;
//if (Serializer.IsLoading) return;
			

			//CombatEncounter.Instance.DelegateEncounterBegin += OnCombatEncounterBegin;
			//CombatEncounter.Instance.DelegateRoundBegin += OnCombatRoundBegin;
			//CombatEncounter.Instance.DelegateDeath += OnCombatDeath;

			//m_initiative = 10;

			//InventoryItems = new List<Item>( );
		}

		//	public override IEnumerator ExecuteRoundActions( )
		//{
		////	while ( DelegateCombatHandler == null )
		//{
		//	yield return new WaitForSeconds( 1.0f );
		//	}

		//yield return DelegateCombatHandler( this );
		//	DelegateCombatHandler = null;
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
		//	}



		//private void OnCombatEncounterBegin( List<Combatant> combatants )
		//	{
		//		m_combatTarget = combatants[1];
		//	}

		//	private void OnCombatRoundBegin( List<Combatant> combatants )
		//	{
		//		// Placeholder
		//	}

		/*		private void OnCombatDeath( Combatant deceased, List<Combatant> remainders )
				{
					if ( deceased != m_combatTarget )
						return ;

					foreach ( Combatant combatant in remainders )
					{
						if ( combatant != this )
						{
							m_combatTarget = combatant;
							return ;
						}
					}

					m_combatTarget = null;
				}*/
	}


}