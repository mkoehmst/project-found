using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ProjectFound.Environment.Characters
{


	public class SkillDefinition : MonoBehaviour
	{
		[SerializeField] string m_skillName;
		[SerializeField] string m_skillDescription;
		[SerializeField] Sprite m_icon;
		[SerializeField] int m_actionPointCost;

		public System.Action DelegateSkillAction { get; set; }

		public Sprite SkillIcon
		{
			get { return m_icon; }
		}

		public string SkillName
		{
			get { return m_skillName; }
		}


		// Use this for initialization
		void Start () {

		}

		// Update is called once per frame
		void Update () {

		}
	}


}
