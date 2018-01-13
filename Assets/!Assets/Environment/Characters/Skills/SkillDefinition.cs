using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ProjectFound.Environment.Characters
{


	[CreateAssetMenu( menuName = ("Project Found/Skill Definition") )]
	public class SkillDefinition : ScriptableObject
	{
		[SerializeField] string m_name;
		[SerializeField] string m_description;
		[SerializeField] Sprite m_icon;
		[SerializeField] int	m_actionPointCost;
		[SerializeField] float	m_range;

		public string	Name			{ get { return m_name; } }
		public string	Description		{ get { return m_description; } }
		public Sprite	Icon			{ get { return m_icon; } }
		public int		ActionPointCost { get { return m_actionPointCost; } }
		public float	Range			{ get { return m_range; } }
	}


}
