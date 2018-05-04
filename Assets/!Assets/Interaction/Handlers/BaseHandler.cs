namespace ProjectFound.Environment.Handlers
{

	using System.Collections;
	using System.Collections.Generic;
	using UnityEngine;
	using UnityEngine.AI;

	using ProjectFound.Environment.Characters;
	using ProjectFound.Environment.Props;
	using ProjectFound.Core;

	public abstract class BaseHandler : ContextHandler
	{
		protected IEnumerator<float> MovePlayerTowards( Interactee i )
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

		protected IEnumerator<float>
			MoveCharacterTowards( CharacterMovement character, Interactee i )
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

		protected IEnumerator<float>
			MoveCharacterTowards( CharacterMovement character, Transform xform )
		{
			const float maxCheckRange = .6f;

			NavMeshHit navHit;

			/*bool didFindNavHit =*/ NavMesh.SamplePosition(
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
					yield return MEC.Timing.WaitForSeconds( 0.3333f );
				}
				else
				{
					yield break;
				}
			}
		}


		protected void RemoveFocusDirectly( Prop prop )
		{
			// Nullify Previous Raycast Hit Check so RemoveFocus isn't called twice
			RaycastMaster.CurrentInteracteeRaycaster.PreviousPriorityHitCheck.Remove( prop );
			RemoveFocus( prop );
		}

		protected void RemoveFocus( Prop prop )
		{
			if ( prop.IsFocused == true )
			{
				GameObject gameObj = prop.gameObject;
				prop.IsFocused = false;

				UIMaster.RemovePrompt( prop );
				ShaderMaster.ToggleSelectionOutline( gameObj );
			}
		}
	}


}
