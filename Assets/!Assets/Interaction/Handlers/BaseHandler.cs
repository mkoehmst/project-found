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
				return MoveCharacterTowards( PlayerMaster.CharacterController, approach.transform );
			}
			else
			{
				return MoveCharacterTowards( PlayerMaster.CharacterController, i.transform );
			}
		}

		protected IEnumerator<float>
			MoveCharacterTowards( MK_RPGCharacterControllerFREE characterController, Interactee i )
		{
			Approach approach = i.GetComponentInChildren<Approach>( );

			if ( approach != null )
			{
				return MoveCharacterTowards( characterController, approach.transform );
			}
			else
			{
				return MoveCharacterTowards( characterController, i.transform );
			}
		}

		protected IEnumerator<float>
			MoveCharacterTowards( MK_RPGCharacterControllerFREE characterController, 
				Transform xform )
		{
			const float maxCheckRange = .6f;

			NavMeshHit navHit;

			/*bool didFindNavHit =*/ NavMesh.SamplePosition(
				xform.position, out navHit, maxCheckRange, NavMesh.AllAreas );

			Vector3 destination = navHit.position;
			characterController.SetMovementTarget( ref destination );

			Transform characterXform = characterController.transform;
			// Give a little buffer room for the StoppingDistance
			float distanceThreshold = characterController.StoppingDistance * 1.25f;

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
				//ShaderMaster.ToggleFocusOutline( gameObj );
			}
		}
	}


}
