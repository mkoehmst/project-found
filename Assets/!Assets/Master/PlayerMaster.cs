using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using ProjectFound.Environment;
using ProjectFound.Environment.Characters;
using ProjectFound.Environment.Props;

namespace ProjectFound.Master {


	public class PlayerMaster
	{
		private const float m_placementElevation = .002f;

		private PlacementClearance PlacementClearance { get; set; }
		private PlacementCursorOffset PlacementCursorOffset { get; set; }

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

		public void StartPropPlacement( Prop prop, GameObject obj, Vector3 hitPoint )
		{
			PropBeingPlaced = obj;
			PlacementClearance = obj.AddComponent<PlacementClearance>( );
			PlacementCursorOffset = obj.AddComponent<PlacementCursorOffset>( );
			PlacementCursorOffset.InitialCursorHit( hitPoint );
			obj.transform.Translate( 0f, m_placementElevation, 0f );
		}

		public void PropPlacement( Vector3 hitPoint )
		{
			Vector3 offsetPoint = hitPoint - PlacementCursorOffset.Offset;

			PropBeingPlaced.transform.position =
				new Vector3( offsetPoint.x, offsetPoint.y + m_placementElevation, offsetPoint.z );
		}

		public void EndPropPlacement( Vector3 hitPoint )
		{
			PropPlacement( hitPoint );
			EndPropPlacement( );
		}

		public void EndPropPlacement( )
		{
			PlacementClearance.Cleanup( );
			//Misc.SmartDestroy.Destroy( PropBeingPlaced.GetComponent<PlacementAngleDetection>( ) );
			Misc.SmartDestroy.Destroy( PlacementCursorOffset );
			PropBeingPlaced = null;
		}

		public void DropItem( Item item )
		{
			item.transform.position = Player.transform.position + Player.transform.forward * 0.75f;
		}
	}


}
