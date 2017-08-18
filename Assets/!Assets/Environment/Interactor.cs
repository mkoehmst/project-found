using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Interactor : Interactee
{
	public abstract bool Action( ActionType actionType, Interactee interactee );
}
