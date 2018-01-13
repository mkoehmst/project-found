using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using ProjectFound.Core;

namespace ProjectFound.Environment.Characters
{


	public abstract class SkillHandler : ContextHandler
	{
		public abstract void Handle( SkillDefinition skillDefinition );
	}


}
