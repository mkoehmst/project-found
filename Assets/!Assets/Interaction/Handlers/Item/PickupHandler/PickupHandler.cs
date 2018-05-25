using System.Collections.Generic;
using UnityEngine;

namespace ProjectFound.Environment.Handlers
{
	using ProjectFound.Environment.Props;
	using ProjectFound.Master;

	[CreateAssetMenu( menuName = ("Project Found/Handlers/Pickup Handler") )]
	public class PickupHandler : InteracteeHandler
	{
		public override IEnumerator<float> Handler( Interactee ie, Interactor ir )
		{
			Item item = ie as Item;

			PlayerMaster.AddInventoryItem( item );

			UIMaster.AddInventoryButton( item ).onClick.AddListener( ( ) =>
			{
				ir.RunUsageChain( ie, ie.UsageChain );
			} );

			item.GetComponent<MeshRenderer>( ).enabled = false;
			item.GetComponent<Collider>( ).enabled = false;

			//RemoveFocus( item as Prop );

			yield break;
		}
	}


}