namespace ProjectFound.Interaction
{ 


	using System.Collections.Generic;
	using UnityEngine;
	using UnityEngine.Assertions;

	using ProjectFound.Environment.Characters;

	[CreateAssetMenu(menuName=("Found/Action Handler/Basic/Movement"))]
	public class MovementActionHandler : InteracteeHandler
	{
		public override IEnumerator<float> Handler( Interactee ie, Interactor ir )
		{
			Combatant combatant = ir as Combatant;
			bool hasSubtractedFirstPoint = false;
			float distanceSinceLastAP = 0f;

			Assert.IsNotNull( combatant );

			combatant.TranslateMoveTarget( 0f, 3.5f, combatant.CombatTarget.transform );

			Assert.IsTrue( combatant.MovementTarget.HasValue );
			
			Vector3 destination = combatant.MovementTarget.Value;

			//combatant.SetMovementTarget( ref destination );

			while ( combatant.DistanceTo( ref destination ) > .05f )
			{
				Vector3 startingPoint = combatant.transform.position;

				yield return MEC.Timing.WaitForOneFrame;

				// First AP subtracted as soon as movement starts
				if ( !hasSubtractedFirstPoint )
					--combatant.ActionPoints;

				Vector3 endingPoint = combatant.transform.position;
				distanceSinceLastAP += (endingPoint - startingPoint).magnitude;

				if ( distanceSinceLastAP >= combatant.MovementSpeed )
				{ 
					--combatant.ActionPoints;
					distanceSinceLastAP -= combatant.MovementSpeed;
				}
			}

			yield break;
		}
	}


}
