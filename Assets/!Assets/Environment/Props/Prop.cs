using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

namespace ProjectFound.Environment.Items {


	public class Prop : Interactee
	{
		[SerializeField] Sprite m_icon;

		public Sprite Icon { get { return m_icon; } }
		public GameObject Prompt { get; set; }

		void Awake( )
		{
			Assert.IsNotNull( m_icon );
		}

		public override bool ValidateAction( ActionType actionType )
		{
			switch ( actionType )
			{
				case ActionType.Use:
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
				case ActionType.Use:
					Debug.Log( "Prop has been used!" );
					break;
			}
		}
	}


}
