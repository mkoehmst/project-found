namespace ProjectFound.Interaction
{ 


	using System.Collections.Generic;
	using UnityEngine;

	using ProjectFound.Environment.Characters;

	[CreateAssetMenu(menuName=("Found/Skill Handler"))]
	public class SkillHandler : InteracteeHandler 
	{
		[Header("Skill Parameters")]
		[SerializeField] int _actionPointCost;
		[SerializeField] float _damageRating;

		public override IEnumerator<float> Handler( Interactee ie, Interactor ir )
		{
			Debug.Log( "Skill " + ToString().Split( '(' )[0] + "Frame " + Time.frameCount );

			Combatant attacker = ir as Combatant;
			Combatant defender = ie as Combatant;

			attacker.ActionPoints -= _actionPointCost;
			//defender.TakeDamage( Mathf.CeilToInt( _damageRating * 2f ) );

			yield break;
		}
	}


}
