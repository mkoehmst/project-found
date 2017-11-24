using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using ProjectFound.Environment;
using ProjectFound.Environment.Characters;
using ProjectFound.Environment.Items;

namespace ProjectFound.Master {


	public class PlayerMaster
	{
		public Player Player { get; private set; }
		public CharacterMovement CharacterMovement { get; private set; }

		public PlayerMaster( )
		{
			Player = GameObject.FindObjectOfType<Player>( );
			CharacterMovement = GameObject.FindObjectOfType<CharacterMovement>( );
		}

		public void Loop( )
		{

		}

		public bool PickUp( Item item )
		{
			Debug.Log( "Player PickUp: " + item );

			return Player.Action( ActionType.PickUp, item as Interactee );
		}

		public bool Use( Interactee interactee )
		{
			Debug.Log( "Player Use: " + interactee );

			return Player.Action( ActionType.Use, interactee );
		}
	}


}
