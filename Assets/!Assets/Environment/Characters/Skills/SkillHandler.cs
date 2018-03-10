using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using ProjectFound.Environment.Handlers;

namespace ProjectFound.Environment.Characters
{


	public abstract class SkillHandler : BaseHandler
	{
		public abstract IEnumerator Handle( SkillSpec skillDefinition, Combatant wielder );
	}


}
