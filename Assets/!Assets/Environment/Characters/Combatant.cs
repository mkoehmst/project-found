namespace ProjectFound.Environment.Characters
{ 


	using System.Collections.Generic;

	using UnityEngine;
	using UnityEngine.Assertions;
	using Autelia.Serialization;

	using ProjectFound.Interaction;

	public abstract class Combatant : Character 
	{
		public delegate void BeginCombatEncounterDelegate( Combatant combatant );
		public delegate void EndCombatTurnDelegate( );

		static public BeginCombatEncounterDelegate DelegateBeginCombatEncounter { get; set; }
		static public EndCombatTurnDelegate DelegateEndCombatTurn { get; set; }

		public float DistanceSinceLastAP { get; set; }

		[Header("Combatant Details")]

		[SerializeField] HandlerChain _combatTurnChain;
		public HandlerChain CombatTurnChain { get { return _combatTurnChain; } }

		[SerializeField] GameObject _projectileChoice;
		public GameObject ProjectileChoice { get { return _projectileChoice; } }
		
		public Combatant CombatTarget { get; set; }
		public HandlerChain CombatActionChain { get; set; }
		public bool IsEngaged { get; protected set; }
		public bool IsDecisionReady { get; set; }

		[SerializeField] List<HandlerChain> _actionBar = new List<HandlerChain>( );
		public List<HandlerChain> ActionBar { get { return _actionBar; } }

		public int ActionPoints { get; set; } = 10;

		new protected void Awake( )
		{
			base.Awake( );

			if ( Serializer.IsLoading ) return;

			Assert.IsNotNull( _combatTurnChain );
			Assert.IsTrue( _actionBar.Count > 0 );
		}

		new protected void Start( ) 
		{
			base.Start( );

			if ( Serializer.IsLoading ) return;
		}

		public void SetCombatActionChain( int actionBarHotKey )
		{
			int index = (actionBarHotKey == 0 ? 9 : actionBarHotKey - 1);

			CombatActionChain = ActionBar[index];
		}

		public virtual void OnBeginCombatEncounter( )
		{ }

		public virtual void OnBeginCombatRound( )
		{ }

		public virtual void OnBeginCombatTurn( )
		{ }

		public virtual IEnumerator<float> ExecuteCombatTurn( )
		{ yield break; }
	}


}
