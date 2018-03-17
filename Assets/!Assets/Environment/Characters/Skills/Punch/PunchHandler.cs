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
			CharacterMovement mover = wielder.GetComponent<CharacterMovement>( );
			Combatant target = wielder.Target;
			Transform targetXform = target.transform;

			Debug.Log( wielder + " is punching " + target );

			if ( mover.CanMoveTo( targetXform.position ) == false )
			{
				yield break;
			}

			float distance = mover.CalculatePathDistance( );
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
