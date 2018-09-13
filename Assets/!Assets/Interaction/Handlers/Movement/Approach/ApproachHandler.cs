namespace ProjectFound.Interaction
{

	using System.Collections.Generic;

	using UnityEngine;

	using ProjectFound.Environment.Characters;

	[CreateAssetMenu( menuName = ("Project Found/Handlers/Movement/Approach") )]
	public class ApproachHandler : InteracteeHandler
	{
		[Header("Approach Parameters")]
		[SerializeField] float _stopDistance;

		public override IEnumerator<float> Handler( Interactee ie, Interactor ir )
		{
			Combatant attacker = ir as Combatant;
			Combatant defender = attacker.CombatTarget;

			if ( attacker == PlayerMaster.Protagonist )
			{
				Vector3 destination = defender.transform.position;
				float distance = attacker.DistanceTo( destination );
				
				if ( distance <= _stopDistance )
					yield break;
			
				PlayerMaster.Protagonist.SetMovementTarget( ref destination );
				
				while ( distance > _stopDistance )
				{
					yield return MEC.Timing.WaitForOneFrame;
					distance = attacker.DistanceTo( destination );
				}
				PlayerMaster.Protagonist.ResetMovementTarget( );
				Debug.Log( "Finished approach Frame " + Time.frameCount );
			}

			yield break;
		}
	}

}