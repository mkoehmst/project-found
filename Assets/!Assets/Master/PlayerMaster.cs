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

	public bool AttemptPropPickup( Prop prop )
	{
		Debug.Log( "InteractWithProp: " + prop );

		return Player.Action( ActionType.PickUp, prop as Interactee );
	}

	public void UseInventoryItem( Interactee item )
	{
		Player.Action( ActionType.UseItem, item );
	}
}

}
