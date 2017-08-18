using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Item : Interactee
{
	public override bool ValidateAction( ActionType actionType )
	{
		switch ( actionType )
		{
			case ActionType.PickUp:
			case ActionType.UseItem:
				m_currentActionType = actionType;
				return true;
			default:
				m_currentActionType = ActionType.None;
				return false;
		}
	}
}
