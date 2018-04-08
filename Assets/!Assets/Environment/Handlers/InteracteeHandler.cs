using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

using ProjectFound.Core;
using ProjectFound.Misc;
using ProjectFound.Environment;

namespace ProjectFound.Environment.Handlers
{


	public abstract class InteracteeHandler : BaseHandler
	{
		public abstract IEnumerator Activate( Interactee i );
	}


}
