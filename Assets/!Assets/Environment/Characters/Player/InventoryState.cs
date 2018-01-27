using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using ProjectFound.Environment.Props;

namespace ProjectFound.Environment.Characters
{


	[CreateAssetMenu(menuName=("Project Found/Inventory State"))]
	public class InventoryState : ScriptableObject
	{
		public Dictionary<Item,InventoryEntry> m_entries;
		public Stack<InventoryEntry> m_availables;
		public int m_maxEntries = 8;

		public void Initialize( )
		{
			m_entries = new Dictionary<Item, InventoryEntry>( );
			m_availables = new Stack<InventoryEntry>( );
		}
	}


}