using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

using ProjectFound.Environment.Characters;
using ProjectFound.Core;

namespace ProjectFound.Environment.Handlers
{


	public abstract class BaseHandler : ContextHandler
	{
		static public void StaticTest( )
		{ }

		protected IEnumerator MovePlayerTowards( Interactee i )
		{
			Approach approach = i.GetComponentInChildren<Approach>( );

			if ( approach != null )
			{
				return MoveCharacterTowards( PlayerMaster.CharacterMovement, approach.transform );
			}
			else
			{
				return MoveCharacterTowards( PlayerMaster.CharacterMovement, i.transform );
			}
		}

		protected IEnumerator MoveCharacterTowards( CharacterMovement character, Interactee i )
		{
			Approach approach = i.GetComponentInChildren<Approach>( );

			if ( approach != null )
			{
				return MoveCharacterTowards( character, approach.transform );
			}
			else
			{
				return MoveCharacterTowards( character, i.transform );
			}
		}

		protected IEnumerator MoveCharacterTowards( CharacterMovement character, Transform xform )
		{
			const float maxCheckRange = .6f;

			NavMeshHit navHit;

			bool didFindNavHit = NavMesh.SamplePosition(
				xform.position, out navHit, maxCheckRange, NavMesh.AllAreas );

			Vector3 destination = navHit.position;
			character.SetMoveTarget( destination );

			Transform characterXform = character.transform;
			// Give a little buffer room for the StoppingDistance
			float distanceThreshold = character.StoppingDistance * 1.25f;

			while ( true )
			{
				Vector3 characterPosition = new Vector3(
						characterXform.position.x, destination.y, characterXform.position.z );

				float distance = (characterPosition - destination).magnitude;
				if ( distance > distanceThreshold )
				{
					yield return new WaitForSeconds( 0.3333f );
				}
				else
				{
					yield break;
				}
			}
		}
	}


}
