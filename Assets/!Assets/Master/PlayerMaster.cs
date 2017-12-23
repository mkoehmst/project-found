using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using ProjectFound.Environment;
using ProjectFound.Environment.Characters;
using ProjectFound.Environment.Props;

namespace ProjectFound.Master {


	public class PlayerMaster
	{
		private Placement Placement { get; set; }

		public GameObject PropBeingPlaced { get; private set; }
		public Player Player { get; private set; }
		public CharacterMovement CharacterMovement { get; private set; }

		public bool OccludedFromCamera { get; set; }

		public PlayerMaster( )
		{
			Player = GameObject.FindObjectOfType<Player>( );
			CharacterMovement = Player.gameObject.GetComponent<CharacterMovement>( );
			OccludedFromCamera = false;
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

		public void StartPropPlacement( Prop prop, GameObject obj, ref RaycastHit hit )
		{
			PropBeingPlaced = obj;
			Placement = obj.AddComponent<Placement>( );
			//Placement.RecordCursorOffset( ref hit );
		}

		public void PropPlacement( ref RaycastHit hit )
		{
			Placement.Place( ref hit );
		}

		public void EndPropPlacement( ref RaycastHit hit )
		{
			Placement.Place( ref hit );
			EndPropPlacement( );
		}

		public void EndPropPlacement( )
		{
			Placement.Cleanup( );
			PropBeingPlaced = null;
		}

		public void DropItem( Item item )
		{
			item.transform.position = Player.transform.position + Player.transform.forward * 0.75f;
		}
	}


}
