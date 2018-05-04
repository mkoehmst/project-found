namespace ProjectFound.Environment.Handlers
{

	using System.Collections.Generic;
	using UnityEngine;

	[CreateAssetMenu( menuName = ("Project Found/Handlers/Animation Handler") )]
	public class AnimationHandler : InteracteeHandler
	{
		private const string m_activateTriggerString = "Prop_Activate";
		private const string m_deactivateTriggerString = "Prop_Deactivate";
		private int m_activateTrigger;
		private int m_deactivateTrigger;

		private WaitForSeconds m_waitFor5s = new WaitForSeconds( 5f );

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

		public override IEnumerator<float> Handle( Interactee interactee, Interactor interactor )
		{
			interactor.HandlerExecutionDictionary[this] = true;

			Animator animator = interactee.GetComponent<Animator>( );

			if ( interactee.IsActivated == false )
			{
				animator?.SetTrigger( m_activateTrigger );
				//m_animator?.SetBool( "IsActivated", true );
				interactee.IsActivated = true;
			}
			else
			{
				animator?.SetTrigger( m_deactivateTrigger );
				//m_animator?.SetBool( "IsActivated", false );
				interactee.IsActivated = false;
			}

			yield return MEC.Timing.WaitForSeconds( 5f );

			interactor.HandlerExecutionDictionary[this] = false;
		}
	}

}
