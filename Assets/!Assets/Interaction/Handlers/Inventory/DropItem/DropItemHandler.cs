namespace ProjectFound.Interaction
{ 


	using System.Collections.Generic;

	using UnityEngine;

	using ProjectFound.Environment.Items;

	[CreateAssetMenu( menuName = ("Project Found/Handlers/Inventory/Drop Item") )]
	public class DropItemHandler : InteracteeHandler 
	{
		public override IEnumerator<float> Handler( Interactee ie, Interactor ir )
		{
			// Make it appear again in game world
			var mrs = ie.GetComponentsInChildren<MeshRenderer>( );

			foreach ( var mr in mrs )
			{
				mr.GetComponent<Collider>( ).enabled = true;
				mr.enabled = true;
			}

			Transform xform = PlayerMaster.Protagonist.transform;

			ie.transform.position = xform.position + (xform.forward * 2.0f);

			UIMaster.InventoryUI.RemoveItem( ie as Item );

			yield break;
		}
	}


}
