using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ProjectFound.Environment.Characters
{


	public class SkillBook : MonoBehaviour
	{
		[SerializeField] Skill[] m_skills;

		public Skill[] Skills
		{
			get { return m_skills; }
		}
	}


}
