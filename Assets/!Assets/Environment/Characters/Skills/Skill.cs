using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ProjectFound.Environment.Characters
{


	[CreateAssetMenu( menuName = ("Project Found/Skill") )]
	public class Skill : ScriptableObject
	{
		[SerializeField] SkillDefinition m_definition;
		[SerializeField] SkillHandler m_handler;

		public SkillDefinition Definition
		{
			get { return m_definition; }
		}

		public SkillHandler Handler
		{
			get { return m_handler; }
		}

		public void Handle( )
		{
			m_handler.Handle( m_definition );
		}
	}


}
