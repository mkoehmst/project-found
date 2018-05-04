using System.Collections.Generic;
using UnityEngine;

namespace ProjectFound.Environment.Handlers
{
	using ProjectFound.Environment.Props;

	[CreateAssetMenu( menuName = ("Project Found/Handlers/Pickup Handler") )]
	public class PickupHandler : InteracteeHandler
	{
		public override IEnumerator<float> Handle( Interactee ie, Interactor ir )
		{
			ir.HandlerExecutionDictionary[this] = true;

			Item item = ie as Item;

			PlayerMaster.AddInventoryItem( item );

			UIMaster.AddInventoryButton( item ).onClick.AddListener( ( ) =>
			{
				item.ExecuteUsageChain( ir );
				//item.BoltVariables.Set( "doStartUsage", true );
				//item.StartCoroutine( item.UsageHandler.Handle( interactee ) );
				// Automatically caches item reference until called! Very powerful.
				/*PlayerMaster.Use( item, ( ) =>
				{
					// Layering of lamba expressions even more powerful!
					UIMaster.RemoveInventoryButton( item );
					PlayerMaster.DropItem( item );
				} );*/
			} );

			item.GetComponent<MeshRenderer>( ).enabled = false;

			RemoveFocus( item as Prop );

			ir.HandlerExecutionDictionary[this] = false;

			yield break;
		}
	}


}