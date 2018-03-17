using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ProjectFound.Environment.Characters
{


	public class SkillBook : MonoBehaviour
	{
		[SerializeField] List<Skill> m_skills;

		public List<Skill> Skills
		{
			get { return m_skills; }
		}
	}


}
