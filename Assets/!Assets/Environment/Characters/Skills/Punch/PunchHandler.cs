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
			MK_RPGCharacterControllerFREE mover = wielder.GetComponent<MK_RPGCharacterControllerFREE>( );
			Combatant target = wielder.CombatTarget;
			Transform targetXform = target.transform;

			Debug.Log( wielder + " is punching " + target );

			if ( PlayerMaster.CanMoveTo( targetXform.position ) == false )
			{
				yield break;
			}

			float distance = PlayerMaster.NavMeshDistanceTo( );
			int actionPointCost = CombatMaster.CalculateMovementCost( wielder, distance );
			actionPointCost += skillDefinition.ActionPointCost;

			if ( CombatMaster.HasEnoughActionPoints( wielder, actionPointCost ) )
			{
				yield return MoveCharacterTowards( mover, target );

				target.TakeDamage( wielder, 5f );
			}
		}
	}


}
