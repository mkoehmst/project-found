namespace ProjectFound.Interaction
{ 


	using System.Collections.Generic;
	using UnityEngine;

	using ProjectFound.Environment.Characters;

	[CreateAssetMenu( menuName = ("Project Found/Handlers/Combat/Default Combat Target") )]
	public class SetDefaultCombatTargetHandler : InteracteeHandler 
	{
		public override IEnumerator<float> Handler( Interactee ie, Interactor ir )
		{
			/*Combatant combatant = ir as Combatant;
			List<Combatant> otherCombatants = CombatMaster.Combatants;

			if ( combatant == PlayerMaster.Protagonist )
			{ 
				combatant.CombatTarget = otherCombatants[1];
			}
			else
			{
				combatant.CombatTarget = otherCombatants[0];
			}*/

			//Debug.Log( "Target is " + combatant.CombatTarget + " Frame " + Time.frameCount );
			
			yield break;
		}
	}


}
