namespace ProjectFound.CameraUI 
{

	using System.Linq;
	using System.Collections.Generic;

	using UnityEngine;
	using Autelia.Serialization;

	using ProjectFound.Environment;

	[RequireComponent(typeof(LineRenderer))]
	public class DetectionRadius : MonoBehaviour
	{
		[Range(1f, 6f)]
		[SerializeField] float m_fullRadius = 3f;

		[Range(0.1f,3f)]
		[SerializeField] float m_growthTime = 0.8f;

		[Range(0.005f, 0.02f)]
		[SerializeField] float m_thetaScale = 0.01f;

		[Range(.02f, .5f)]
		[SerializeField] float _lineWidth = 0.1f;

		[System.NonSerialized] private bool _isEnabled;

		private int size;
		private float theta = 0f;
		private float _radius;
		private float _growthRate;

		private LineRenderer LineDrawer { get; set; }

		public Collider[] ObjectsWithin { get; private set; }
		public int ObjectsWithinCount { get; private set; }

		void Awake( )
		{
			_isEnabled = false;

			if (Serializer.IsLoading) return;

			LineDrawer = GetComponent<LineRenderer>( );

			ObjectsWithin = new Collider[512];
		}

		void LateUpdate( )
		{
			if ( !_isEnabled ) return ;
			if ( Serializer.IsLoading ) return;

			_radius += _growthRate * Time.deltaTime;
			if ( _radius > m_fullRadius )
			{
				_radius = m_fullRadius;
			}

			theta = 0f;
			size = (int)((1f / m_thetaScale) + 2f);
			LineDrawer.positionCount = size;

			for( int i = 0; i < size; i++ )
			{
				theta += (2.0f * Mathf.PI * m_thetaScale);
				float x = _radius * Mathf.Sin( theta );
				float y = _radius * Mathf.Cos( theta );
				LineDrawer.SetPosition( i, new Vector3( x, y, 0f ) );
			}
		}

		public void GatherDetections( )
		{
			const int layerMask = (1 << (int)LayerID.Prop) | (1 << (int)LayerID.Item);

			ObjectsWithinCount = 
				Physics.OverlapSphereNonAlloc( 
					transform.position, _radius, ObjectsWithin, layerMask );

			Debug.Log( ObjectsWithinCount );
		}

		public void Enable( )
		{
			_radius = 0f;
			_growthRate = m_fullRadius / m_growthTime;
			LineDrawer.positionCount = 0;
			LineDrawer.widthMultiplier = _lineWidth;

			_isEnabled = true;
		}

		public void Disable( )
		{

			LineDrawer.positionCount = 0;
			ObjectsWithinCount = 0;

			_isEnabled = false;
		}
	}


}
