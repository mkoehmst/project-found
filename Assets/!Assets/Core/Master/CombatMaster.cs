namespace ProjectFound.Core.Master
{ 


	using System.Collections.Generic;
	using UnityEngine;
	using UnityEngine.Assertions;
	using Autelia.Serialization;

	using ProjectFound.Environment.Characters;
	using ProjectFound.Interaction;

	public class CombatMaster  
	{
		private MEC.CoroutineHandle _encounterHandle;
		private MEC.CoroutineHandle _roundHandle;
		private MEC.CoroutineHandle _turnHandle;
		private Protagonist _protagonist;
		
		public List<Combatant> Combatants { get; private set; }
		public Combatant ActiveCombatant { get; private set; }

		public CombatMaster( )
		{
			if ( Serializer.IsLoading ) return;

			_protagonist = GameObject.FindObjectOfType<Protagonist>( );
			Assert.IsNotNull( _protagonist );

			Combatants = new List<Combatant>( ) 
				{ _protagonist as Combatant };

			Combatant.DelegateBeginCombatEncounter = BeginCombatEncounter;
			Combatant.DelegateEndCombatTurn = EndCombatTurn;
		}

		public void BeginCombatEncounter( Combatant combatant )
		{
			Combatants.Add( combatant );

			int count = Combatants.Count;
			for ( int i = 0; i < count; ++i )
			{
				Combatants[i].OnBeginCombatEncounter( );
			}

			MEC.Timing.RunThisCoroutine( ExecuteCombatEncounter( ), out _encounterHandle );
		}

		public void EndCombatTurn( )
		{
			ActiveCombatant = null;
		}

		public IEnumerator<float> ExecuteCombatEncounter( )
		{
			Debug.Log( "ExecuteCombatEncounter Frame " + Time.frameCount );

			yield return MEC.Timing.WaitForSeconds( 1.25f );

			for ( int i = 0; i < 2; ++i )
			{
				Debug.Log( 
					"Round " + (i+1) + " ExecuteCombatEncounter Frame " + Time.frameCount );

				yield return MEC.Timing.RunThisCoroutine( ExecuteCombatRound( ), 
					out _roundHandle, ref _encounterHandle, MEC.NestingType.ChildBlock );
			}

			Debug.Log( "EndCombatEncounter Frame " + Time.frameCount );
			_encounterHandle = MEC.CoroutineHandle.RawHandle;
			_roundHandle = MEC.CoroutineHandle.RawHandle;
			_turnHandle = MEC.CoroutineHandle.RawHandle;
			ActiveCombatant = null;
			Combatants.RemoveRange( 1, Combatants.Count - 1 );

			yield break;
		}

		public IEnumerator<float> ExecuteCombatRound( )
		{
			Debug.Log( "ExecuteCombatRound Frame " + Time.frameCount );

			int count = Combatants.Count;
			for ( int i = 0; i < count; ++i )
			{
				ActiveCombatant = Combatants[i];
				SetCombatTarget( ActiveCombatant );

				Debug.Log( "Combatant " + (i+1) + " ExecuteCombatRound Frame " + Time.frameCount );

				yield return MEC.Timing.RunThisCoroutine( 
					ActiveCombatant.CombatTurnChain.RunHandlerChain( 
						ActiveCombatant.CombatTarget, ActiveCombatant ), 
					out _turnHandle, ref _roundHandle, 
					MEC.NestingType.ChildBlock );

				yield return MEC.Timing.WaitForSeconds( 1f );
			}

			Debug.Log( "EndCombatRound Frame " + Time.frameCount );

			yield break;
		}

		public void SetCombatTarget( Combatant combatant )
		{
			if ( combatant == Combatants[0] )
			{
				combatant.CombatTarget = Combatants[1];
			}
			else
			{
				combatant.CombatTarget = Combatants[0];
			}
		}
	}


}
