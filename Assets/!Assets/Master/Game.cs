using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Game : Singleton<Game>
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
