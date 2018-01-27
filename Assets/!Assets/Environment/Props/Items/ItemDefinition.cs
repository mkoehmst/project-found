using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ProjectFound.Environment.Props
{


	[CreateAssetMenu(menuName=("Project Found/Item Definition"))]
	public class ItemDefinition : PropDefinition
	{
		public int m_slotCountX = 1;
		public int m_slotCountY = 1;
	}


}