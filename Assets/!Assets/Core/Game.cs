using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using ProjectFound.Master;

namespace ProjectFound.Core {


	public class Game : Misc.Singleton<Game>
	{
		public PlayerMaster PlayerMaster { get; private set; }

		public CombatContext	CombatContext	{ get; private set; }
		//public NonCombatContext NonCombatContext{ get; private set; }
		public GameContext		CurrentContext	{ get; set; }


		void Start( )
		{
			PlayerMaster		= new PlayerMaster( );
			CombatContext		= new CombatContext( PlayerMaster );
			//NonCombatContext	= new NonCombatContext( );

			CurrentContext = CombatContext;
		}

		void Update( )
		{
			CurrentContext.Loop( );
		}
	}


}
