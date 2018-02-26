using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

using ProjectFound.Core;
using ProjectFound.Misc;
using ProjectFound.Environment.Characters;

namespace ProjectFound.Environment.Handlers
{


	public abstract class InteracteeHandler : ContextHandler
	{
		public abstract IEnumerator Use( Interactee i );

		protected IEnumerator MovePlayerTowards( Interactee i )
		{
			const float maxCheckRange = .6f;

			NavMeshHit navHit;
			bool didFindNavHit;
			Vector3 destination;
			Approach approach = i.GetComponentInChildren<Approach>( );

			if ( approach != null )
			{
				didFindNavHit = NavMesh.SamplePosition(
					approach.transform.position, out navHit, maxCheckRange, NavMesh.AllAreas );
			}
			else
			{
				didFindNavHit = NavMesh.SamplePosition(
					i.transform.position, out navHit, maxCheckRange, NavMesh.AllAreas );
			}

			destination = navHit.position;
			PlayerMaster.CharacterMovement.SetMoveTarget( destination );

			Transform playerTransform = PlayerMaster.Player.transform;
			// Give a little buffer room for the StoppingDistance
			float distanceThreshold = PlayerMaster.CharacterMovement.StoppingDistance * 1.25f;

			while ( true )
			{
				Vector3 playerPosition = new Vector3(
					playerTransform.position.x, destination.y, playerTransform.position.z );

				float distance = (playerPosition - destination).magnitude;
				if ( distance >	distanceThreshold )
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
