using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Prop : Item
{
	public override bool ValidateAction( ActionType actionType )
	{
		if ( base.ValidateAction( actionType ) == true )
			return true;

		switch ( actionType )
		{
			default:
				m_currentActionType = ActionType.None;
				return false;
		}

		m_currentActionType = actionType;
		return true;
	}

	public override void Reaction( )
	{
		if ( m_currentActionType == ActionType.PickUp )
		{
			this.gameObject.SetActive( false );
		}

		else if ( m_currentActionType == ActionType.UseItem )
		{
			this.gameObject.SetActive( true );
		}
	}
}
