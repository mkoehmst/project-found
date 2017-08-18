using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMaster
{
	public Player Player { get; private set; }
	public PlayerMovement PlayerMovement { get; private set; }

	public PlayerMaster( )
	{
		Player = GameObject.FindObjectOfType<Player>( );
		PlayerMovement = GameObject.FindObjectOfType<PlayerMovement>( );
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
