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

	public bool AttemptItemPickup( Item item )
	{
		Debug.Log( "AttemptItemPickup: " + item );

		return Player.Action( ActionType.PickUp, item as Interactee );
	}

	public void UseInventoryItem( Interactee item )
	{
		Player.Action( ActionType.Use, item );
	}
}

}
