namespace ProjectFound.Environment.Handlers
{

	using System.Collections.Generic;
	
	using UnityEngine;

	using ProjectFound.Environment.Props;

	[CreateAssetMenu(menuName =("Project Found/Handlers/Drop Item Handler"))]
	public class DropItemHandler : InteracteeHandler
	{
		public override IEnumerator<float> Handler( Interactee ie, Interactor ir )
		{
			Item item = ie as Item;

			PlayerMaster.DropItem( item );
			UIMaster.RemoveInventoryButton( item );

			item.GetComponent<MeshRenderer>( ).enabled = true;
			item.GetComponent<Collider>( ).enabled = true;

			yield break;
		}
	}

}
