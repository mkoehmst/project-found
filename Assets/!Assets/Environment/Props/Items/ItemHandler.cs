using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using ProjectFound.Core;

namespace ProjectFound.Environment.Props
{


	public abstract class ItemHandler : PropHandler
	{




		//public abstract void AddToInventory( ItemDefinition item );
		//public abstract void RemoveFromInventory( ItemDefinition item );
		public abstract override void Use( );

		public virtual void AddToInventory( )
		{
			Item item = m_component as Item;

			RemoveFocusDirectly( );

			PlayerMaster.AddInventoryItem( item );
			// Nullify Raycast Hit Check so RemoveFocus isn't called twice
			//RaycastMaster.CurrentRaycaster.PriorityHitCheck.Remove( m_gameObject );
			//RemoveFocus( prop );

			// Lambda statement delegates...love this
			UIMaster.AddInventoryButton( item ).onClick.AddListener( () =>
			{
				// Automatically caches item reference until called! Very powerful.
				//PlayerMaster.Use( item, () =>
				//{
					// Layering of lamba expressions even more powerful!
				UIMaster.RemoveInventoryButton( item );
				PlayerMaster.DropItem( item );
				m_gameObject.SetActive( true );
				//} );
			} );

			m_gameObject.SetActive( false );
		}

		public virtual void RemoveFromInventory( )
		{

		}


	}


}
