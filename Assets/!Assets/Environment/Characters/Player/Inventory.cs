using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using ProjectFound.Environment.Props;

namespace ProjectFound.Environment.Characters
{


	public class Inventory : MonoBehaviour
	{
		[SerializeField] InventoryState m_inventoryState;

		void Start( )
		{
			m_inventoryState.Initialize( );
		}

		public void AddItem( Item item )
		{
			Dictionary<Item,InventoryEntry> entries = m_inventoryState.m_entries;
			Stack<InventoryEntry> availables = m_inventoryState.m_availables;

			if ( entries.Count < m_inventoryState.m_maxEntries )
			{
				InventoryEntry entry = null;

				if ( availables.Count > 0 )
				{
					entry = availables.Pop( );
				}
				else
				{
					entry = new InventoryEntry( );
				}

				entries[item] = entry;
			}
		}

		public void RemoveItem( Item item )
		{
			Dictionary<Item,InventoryEntry> entries = m_inventoryState.m_entries;
			Stack<InventoryEntry> availables = m_inventoryState.m_availables;

			if ( entries.ContainsKey( item ) )
			{
				InventoryEntry entry = entries[item];

				availables.Push( entry );

				entries.Remove( item );
			}
		}
	}


}
