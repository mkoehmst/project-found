using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

using ProjectFound.Environment.Props;

namespace ProjectFound.Environment.Handlers
{


	public abstract class ItemHandler : PropHandler
	{
		public override IEnumerator Use( Interactee interactee )
		{
			yield return MovePlayerTowards( interactee );

			Item item = interactee as Item;

			// Begin process of picking up object
			GameObject gameObj = item.gameObject;

			RemoveFocusDirectly( item );

			PlayerMaster.AddInventoryItem( item );

			// Lambda statement delegates...love this
			UIMaster.AddInventoryButton( item ).onClick.AddListener( ( ) =>
			{
				// Automatically caches item reference until called! Very powerful.
				//PlayerMaster.Use( item, () =>
				//{
				// Layering of lamba expressions even more powerful!
				UIMaster.RemoveInventoryButton( item );
				PlayerMaster.DropItem( item );
				gameObj.SetActive( true );
				//} );
			} );

			gameObj.SetActive( false );
		}

		public virtual void RemoveFromInventory( Item item )
		{

		}
	}


}
