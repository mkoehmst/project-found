namespace ProjectFound.Interaction
{ 
	

	using System.Collections.Generic;
	using UnityEngine;
	using UnityEngine.Assertions;

	using ProjectFound.Environment.Characters;

	[CreateAssetMenu(menuName = ("Found/Handlers/Combat/Launch Projectile"))]
	public class LaunchProjectileHandler : InteracteeHandler
	{
		public override IEnumerator<float> Handler( Interactee ie, Interactor ir )
		{
			Combatant attacker = ir as Combatant;

			Assert.IsNotNull( attacker );
			Assert.IsNotNull( attacker.ProjectileChoice );
			Assert.IsNotNull( attacker.CombatTarget );

			Combatant defender = attacker.CombatTarget;

			GameObject projectile = GameObject.Instantiate( attacker.ProjectileChoice );

			projectile.transform.position = attacker.transform.position 
				+ new Vector3( 0f, 1f, 0f ) + attacker.transform.forward * -.5f;

			projectile.transform.LookAt( defender.transform.position + new Vector3( 0f, 1f, 0f ) );

			Rigidbody rb = projectile.GetComponent<Rigidbody>( );
			rb.AddForce( projectile.transform.forward * 10f, ForceMode.Impulse );

			while ( defender.DistanceTo( 
				projectile.transform.position - new Vector3( 0f, 1f, 0f ) ) > .125f )
			{
				yield return MEC.Timing.WaitForOneFrame;
			}

			GameObject.Destroy( projectile );

			yield break;
		}
	}


}
