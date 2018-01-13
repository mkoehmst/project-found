using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ProjectFound.Environment.Characters
{


	[CreateAssetMenu( menuName = ("Project Found/Skill Handler/Punch Handler") )]
	public class PunchHandler : SkillHandler
	{
		public override void Handle( SkillDefinition skillDefinition )
		{
			if ( PlayerMaster.CanMoveToTarget( ) == false )
			{
				return;
			}

			float distance = PlayerMaster.NavMeshDistanceTo( );

			int actionPointCost = CombatMaster.CalculateMovementCost(
						PlayerMaster.Player as Combatant, distance );

			actionPointCost += skillDefinition.ActionPointCost;

			if ( CombatMaster.HasEnoughActionPoints(
				PlayerMaster.Player as Combatant, actionPointCost ) )
			{
				PlayerMaster.MoveToTarget( );
			}
		}
	}


}
