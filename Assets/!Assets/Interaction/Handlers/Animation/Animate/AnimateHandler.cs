namespace ProjectFound.Interaction
{


	using System.Collections.Generic;

	using UnityEngine;

	[CreateAssetMenu( menuName = ("Project Found/Handlers/Animation Handler") )]
	public class AnimateHandler : InteracteeHandler
	{
		private const string m_activateTriggerString = "Prop_Activate";
		private const string m_deactivateTriggerString = "Prop_Deactivate";
		private int m_activateTrigger;
		private int m_deactivateTrigger;

		void OnEnable( )
		{
			if ( m_activateTrigger == 0 )
			{
				m_activateTrigger = Animator.StringToHash( m_activateTriggerString );
			}

			if ( m_deactivateTrigger == 0 )
			{
				m_deactivateTrigger = Animator.StringToHash( m_deactivateTriggerString );
			}
		}

		public override IEnumerator<float> Handler( Interactee ie, Interactor ir )
		{
			Debug.Log( "AnimationHandler" );

			Animator animator = ie.GetComponentInChildren<Animator>( );

			if ( ie.IsActivated == false )
			{
				animator?.SetTrigger( m_activateTrigger );
				//m_animator?.SetBool( "IsActivated", true );
				ie.IsActivated = true;
			}
			else
			{
				animator?.SetTrigger( m_deactivateTrigger );
				//m_animator?.SetBool( "IsActivated", false );
				ie.IsActivated = false;
			}

			yield return MEC.Timing.WaitForSeconds( 5f );
		}
	}


}
