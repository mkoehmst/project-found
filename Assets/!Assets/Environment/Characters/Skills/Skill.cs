using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ProjectFound.Environment.Characters
{


	[CreateAssetMenu( menuName = ("Project Found/Skill") )]
	public class Skill : ScriptableObject
	{
		[SerializeField] SkillSpec m_specification;
		[SerializeField] SkillHandler m_handler;

		public SkillSpec Specification
		{
			get { return m_specification; }
		}

		public SkillHandler Handler
		{
			get { return m_handler; }
		}

		public void Handle( )
		{
			m_handler.Handle( m_specification );
		}
	}


}
