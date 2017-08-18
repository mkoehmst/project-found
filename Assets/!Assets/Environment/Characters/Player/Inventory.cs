using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Inventory
{
	public struct Item
	{
		public Prop Prop { get; set; }
		public Button Button { get; set; }
	}

	private int Count { get; set; }
	public Prop[] Items { get; set; }

	public Inventory( )
	{
		Items = new Prop[3];
		Count = 0;
	}

	public void AddItem( Prop prop )
	{
		if ( Count < 3 )
			Items[Count++] = prop;
	}

	public void RemoveItem( Prop prop )
	{
		for ( int i = 0; i < Count; ++i )
		{
			if ( Items[i] == prop )
			{
				// If something other than the last item is removed, move others down to fill
				// the gap

				while ( i < (Count - 1) )
				{
					Items[i] = Items[++i];
				}

				Items[i] = null;
				--Count;
			}
		}
	}
}
