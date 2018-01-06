using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using ProjectFound.Environment.Characters;

namespace ProjectFound.Master
{


	public class CombatMaster
	{
		public CombatEncounter CombatEncounter { get; private set; }

		public CombatMaster( )
		{
			CombatEncounter = GameObject.FindObjectOfType<CombatEncounter>( );
		}

		public void Loop( )
		{

		}

		public void SetCombatBeginDelegate( CombatEncounter.CombatBegin combatBeginDelegate )
		{
			CombatEncounter.DelegateEncounterBegin += combatBeginDelegate;
		}

		public int CalculateMovementCost( Combatant combatant, float distance )
		{
			// Can move less than 5 cm at a time without cost (for turning around)
			if ( distance < .05f )
			{
				return 0;
			}

			float costScore = distance / combatant.MovementScore * 2.0f;

			// Round up to always have at least 1 action point cost
			return Mathf.CeilToInt( costScore );
		}

		public bool HasEnoughActionPoints( Combatant combatant, int actionPointCount )
		{
			return combatant.ActionPoints >= actionPointCount;
		}
	}


}