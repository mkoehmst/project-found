namespace ProjectFound.Environment.Handlers
{

	using System.Collections.Generic;
	using UnityEngine;

	using ProjectFound.Environment.Props;

	[CreateAssetMenu(menuName =("Project Found/Handlers/Drop Item Handler"))]
	public class DropItemHandler : InteracteeHandler
	{
		public override IEnumerator<float> Handle( Interactee ie, Interactor ir )
		{
			ir.HandlerExecutionDictionary[this] = true;

			Item item = ie as Item;

			PlayerMaster.DropItem( item );
			UIMaster.RemoveInventoryButton( item );

			item.GetComponent<MeshRenderer>( ).enabled = true;

			ir.HandlerExecutionDictionary[this] = false;

			yield break;
		}
	}

}
