namespace ProjectFound.Interaction
{ 


	using System.Collections.Generic;
	
	using UnityEngine;

	using ProjectFound.Environment.Items;

	[CreateAssetMenu( menuName = ("Project Found/Handlers/Inventory/Pickup Item") )]
	public class PickupItemHandler : InteracteeHandler
	{
		public override IEnumerator<float> Handler( Interactee ie, Interactor ir )
		{
			// Make it disappear from game world
			var mrs = ie.GetComponentsInChildren<MeshRenderer>( );

			foreach (var mr in mrs)
			{
				mr.GetComponent<Collider>( ).enabled = false;
				mr.enabled = false;
			}

			UIMaster.InventoryUI.AddItem( ie as Item ).onClick.AddListener( () =>
			{
				PlayerMaster.Protagonist.RunUsageChain( ie );
			} );

			yield break;
		}
	}



}
