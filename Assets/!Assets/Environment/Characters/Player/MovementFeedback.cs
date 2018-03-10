using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ProjectFound.Environment.Characters
{


	public class MovementFeedback : MonoBehaviour
	{
		[SerializeField] float m_radius = 0.3f;
		[SerializeField] float m_thetaScale = 0.01f;
		[SerializeField] float m_yAxisOffset = 0.01f;
		[SerializeField] Material m_feedbackGoodMaterial;
		[SerializeField] Material m_feedbackBadMaterial;

		private int size;
		//private bool m_feedbackIsGood = false;
		private float theta = 0f;

		public LineRenderer LineDrawer { get; private set; }
		public bool IsFeedbackGood { get; set; } = false;


		void Awake( )
		{
			LineDrawer = GetComponent<LineRenderer>( );
		}

		void Start( )
		{
			theta = 0f;
			size = (int)((1f / m_thetaScale) + 2f);
			LineDrawer.positionCount = size;
		}

		public void DrawCenter( Vector3 worldPos )
		{
			if ( IsFeedbackGood == true )
			{
				LineDrawer.material = m_feedbackGoodMaterial;
			}
			else
			{
				LineDrawer.material = m_feedbackBadMaterial;
			}

			for ( int i = 0; i < size; i++ )
			{
				theta += (2.0f * Mathf.PI * m_thetaScale);
				float x = m_radius * Mathf.Sin( theta );
				float y = m_radius * Mathf.Cos( theta );
				LineDrawer.SetPosition( i,
					new Vector3( worldPos.x + x, worldPos.y + m_yAxisOffset, worldPos.z + y) );
			}
		}
	}


}