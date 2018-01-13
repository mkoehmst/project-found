using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ProjectFound.Environment.Characters
{

	[CreateAssetMenu( menuName = ("Project Found/Conduct Bar State") )]
	public class ConductBarState : ScriptableObject
	{
		public Skill[] m_skills;
	}


}
