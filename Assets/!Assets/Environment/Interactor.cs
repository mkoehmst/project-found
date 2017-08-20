using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ProjectFound.Environment {

public abstract class Interactor : Interactee
{
	public abstract bool Action( ActionType actionType, Interactee interactee );
}

}
