using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using ProjectFound.Misc;

namespace ProjectFound.CameraUI {


	public class DetectionRadius : MonoBehaviour
	{
		[SerializeField] float m_fullRadius = 1.5f;
		[SerializeField] float m_growthTime = 3f;
		[SerializeField] float m_thetaScale = 0.01f;

		public Collider[] ObjectsWithin { get; private set; }

		private int size;
		private LineRenderer LineDrawer;
		private float theta = 0f;
		private float m_radius;
		private float m_growthRate;

		void Start( )
		{
			LineDrawer = gameObject.GetComponent<LineRenderer>( );
		}

		void Update( )
		{
			m_radius += m_growthRate * Time.deltaTime;
			if ( m_radius > m_fullRadius )
			{
				m_radius = m_fullRadius;
			}

			theta = 0f;
			size = (int)((1f / m_thetaScale) + 2f);
			LineDrawer.positionCount = size;

			for( int i = 0; i < size; i++ )
			{
				theta += (2.0f * Mathf.PI * m_thetaScale);
				float x = m_radius * Mathf.Sin(theta);
				float y = m_radius * Mathf.Cos(theta);
				LineDrawer.SetPosition( i, new Vector3( x, y, 0f ) );
			}
		}

		void OnEnable( )
		{
			m_radius = 0f;
			m_growthRate = m_fullRadius / m_growthTime;

			if ( LineDrawer != null )
			{
				LineDrawer.positionCount = 0;
			}
		}

		public void GatherDetections( )
		{
			ObjectsWithin = Physics.OverlapSphere( transform.position, m_radius );
		}
	}


}
