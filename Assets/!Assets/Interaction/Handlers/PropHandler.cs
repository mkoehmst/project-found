using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using ProjectFound.Core;
using ProjectFound.Master;
using ProjectFound.Environment.Props;

namespace ProjectFound.Environment.Handlers
{

	public abstract class PropHandler : InteracteeHandler
	{
		public virtual IEnumerator HandleDragAndDrop( Prop ie, Interactor ir )
		{ yield break; }
	}


}
