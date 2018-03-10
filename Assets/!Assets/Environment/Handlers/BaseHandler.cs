using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

using ProjectFound.Core;

namespace ProjectFound.Environment.Handlers
{


	public abstract class BaseHandler : ContextHandler
	{
		protected IEnumerator MovePlayerTowards( Interactee i )
		{
			Approach approach = i.GetComponentInChildren<Approach>( );

			if ( approach != null )
			{
				return MovePlayerTowards( approach.transform );
			}
			else
			{
				return MovePlayerTowards( i.transform );
			}
		}

		protected IEnumerator MovePlayerTowards( Transform xform )
		{
			const float maxCheckRange = .6f;

			NavMeshHit navHit;

			bool didFindNavHit = NavMesh.SamplePosition(
				xform.position, out navHit, maxCheckRange, NavMesh.AllAreas );

			Vector3 destination = navHit.position;
			PlayerMaster.CharacterMovement.SetMoveTarget( destination );

			Transform playerTransform = PlayerMaster.Player.transform;
			// Give a little buffer room for the StoppingDistance
			float distanceThreshold = PlayerMaster.CharacterMovement.StoppingDistance * 1.25f;

			while ( true )
			{
				Vector3 playerPosition = new Vector3(
						playerTransform.position.x, destination.y, playerTransform.position.z );

				float distance = (playerPosition - destination).magnitude;
				if ( distance > distanceThreshold )
				{
					yield return null;
				}
				else
				{
					yield return new WaitForSeconds( 0.40f );
					break;
				}
			}
		}
	}


}
