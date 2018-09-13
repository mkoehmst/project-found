namespace ProjectFound.Interaction
{ 


	using System.Collections.Generic;
	using UnityEngine;

	using ProjectFound.Environment.Characters;

	[CreateAssetMenu(menuName=("Found/Handlers/Combat/Wait For Combat Action"))]
	public class WaitForCombatActionHandler : InteracteeHandler 
	{

		public override IEnumerator<float> Handler( Interactee ie, Interactor ir )
		{
			Combatant attacker = ir as Combatant;

			MEC.CoroutineHandle handle = new MEC.CoroutineHandle( );
			MEC.CoroutineHandle localHandle = new MEC.CoroutineHandle( );
			yield return MEC.Timing.GetMyHandle( x => localHandle = x );

			Debug.Log("WaitForCombatActionHandler Enter Frame " + Time.frameCount);

			
			if ( attacker != PlayerMaster.Protagonist )
				yield break;

			while ( true )
			{
				HandlerChain actionChain = attacker.CombatActionChain;
				if ( actionChain == null )
				{
					yield return MEC.Timing.WaitForOneFrame;
					continue;
				}

				attacker.CombatActionChain = null;

				yield return MEC.Timing.RunThisCoroutine( actionChain.RunHandlerChain( ie, ir ), 
					out handle, ref localHandle, MEC.NestingType.ChildBlock );

				Debug.Log( "WaitForCombatActionHandler Exit Frame " + Time.frameCount );
				yield break;
			}
		}
	}


}
