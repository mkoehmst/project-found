using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ProjectFound.Environment.Characters
{


	[CreateAssetMenu( menuName = ("Project Found/Skill Handler/Punch Handler") )]
	public class PunchHandler : SkillHandler
	{
		public override IEnumerator Handle( SkillSpec skillDefinition, Combatant wielder )
		{
			if ( PlayerMaster.CanMoveToTarget( ) == false )
			{
				yield break;
			}

			float distance = PlayerMaster.NavMeshDistanceTo( );

			int actionPointCost = CombatMaster.CalculateMovementCost(
						PlayerMaster.Player as Combatant, distance );

			actionPointCost += skillDefinition.ActionPointCost;

			if ( CombatMaster.HasEnoughActionPoints(
				PlayerMaster.Player as Combatant, actionPointCost ) )
			{
				yield return MovePlayerTowards( PlayerMaster.Player.Target );

				PlayerMaster.Player.Target.TakeDamage( PlayerMaster.Player, 5f );
			}
		}
	}


}
