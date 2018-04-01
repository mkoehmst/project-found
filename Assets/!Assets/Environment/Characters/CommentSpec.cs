using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ProjectFound.Environment.Characters
{


	[CreateAssetMenu(menuName=("Project Found/Comment Spec"))]
	public class CommentSpec : ScriptableObject
	{
		public GameObject m_displayPrefab;
		public List<string> m_comments;
	}


}