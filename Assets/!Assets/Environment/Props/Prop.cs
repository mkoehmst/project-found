using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

namespace ProjectFound.Environment.Props {


	public class Prop : Interactee
	{
		[SerializeField] Sprite m_icon;

		private int m_activateTrigger;

		public Animator Animator { get; private set; }

		public Sprite Icon { get { return m_icon; } }
		public GameObject Prompt { get; set; }

		void Awake( )
		{
			Assert.IsNotNull( m_icon );

			Animator = GetComponent<Animator>( );
		}

		new void Start( )
		{
			base.Start( );

			m_activateTrigger = Animator.StringToHash( "Prop_Activate" );
		}

		public override bool ValidateAction( ActionType actionType )
		{
			switch ( actionType )
			{
				case ActionType.Activate:
					m_currentActionType = actionType;
					return true;
				default:
					m_currentActionType = ActionType.None;
					return false;
			}
		}

		public override void Reaction( )
		{
			switch ( m_currentActionType )
			{
				case ActionType.Activate:
					Debug.Log( "Prop has been activated!" );
					if ( Animator != null )
						Animator.SetTrigger( m_activateTrigger );
					m_isReceptive = false;
					break;
			}
		}
	}


}
