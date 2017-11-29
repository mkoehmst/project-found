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
			CharacterMovement = Player.gameObject.GetComponent<CharacterMovement>( );
		}

		public void Loop( )
		{

		}

		public bool PickUp( Item item, System.Action action )
		{
			Debug.Log( "Player PickUp: " + item );

			return Player.Action( ActionType.PickUp, item as Interactee, action );
		}

		public bool Use( Item item, System.Action action )
		{
			Debug.Log( "Player Use: " + item );

			return Player.Action( ActionType.Use, item as Interactee, action );
		}

		public bool Activate( Prop prop, System.Action action )
		{
			Debug.Log( "Player Activate: " + prop );

			return Player.Action( ActionType.Activate, prop as Interactee, action );
		}
	}


}
