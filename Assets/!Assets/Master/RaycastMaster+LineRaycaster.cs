using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.EventSystems;

using mattmc3.dotmore.Collections.Generic;

namespace ProjectFound.Master
{


	public partial class RaycastMaster
	{
		public LineRaycaster<_T> AddLineRaycaster<_T>(
			RaycastMode mode, float maxDistance, int resolution, bool isEnabled = true )
			where _T : Component
		{
			LineRaycaster<_T> raycaster =
				new LineRaycaster<_T>( mode, maxDistance, resolution, isEnabled );

			Raycasters[mode] = raycaster;

			return raycaster;
		}

		public class LineRaycaster<_T> : Raycaster<_T>
			where _T : Component
		{
			private const int m_maxPoints = 24;

			public delegate void LineTrackingDelegate( ref Vector3 start, ref Vector3 end );
			public delegate void CasterAssignmentsDelegate( ref Ray ray, ref Vector3 screenPos );

			private int m_pixelResolution;
			private Vector3 m_lineStart;
			private Vector3 m_lineEnd;

			private Ray[] m_raycasters = new Ray[m_maxPoints];

			private List<_T> m_lastComponentsHit;
			//private _T m_lastHit;

			public LineTrackingDelegate DelegateLineTracking { get; set; }
			public CasterAssignmentsDelegate DelegateCasterAssignments { get; set; }

			public LineRaycaster(
				RaycastMode mode, float maxDistance, int resolution, bool isEnabled = true )
					: base( mode, maxDistance, isEnabled )
			{
				m_pixelResolution = 48 / Mathf.Clamp( resolution, 1, 4 );
			}

			public override void Cast( )
			{
				if ( EventSystem.current.IsPointerOverGameObject( ) )
					return ;

				DelegateLineTracking( ref m_lineStart, ref m_lineEnd );

				Vector3 lineVector = m_lineEnd - m_lineStart;
				Vector3 lineDirection = lineVector.normalized;

				float lineLength = lineVector.magnitude;
				float lineGap = lineLength / m_pixelResolution;

				int numPoints = Mathf.RoundToInt( lineGap ) + 1; // Add one to ensure at least one
				numPoints = (numPoints > m_maxPoints) ? m_maxPoints : numPoints;

				Debug.Log( numPoints );

				for ( int i = 0; i < numPoints; ++i )
				{
					Vector3 linePos = m_lineStart + (lineDirection * lineGap * i);

					DelegateCasterAssignments( ref m_raycasters[i], ref linePos );

					PerformRaycast( ref m_raycasters[i] );
				}

				ProcessRaycastResults( );
				CycleHitCheck( );
			}

			protected void PerformRaycast( ref Ray ray )
			{
				RaycastHit firstHit;

				bool success =
					Physics.Raycast( ray, out firstHit, MaxDistance, LayerMask );

				if ( success == true )
				{
					GameObject obj = firstHit.collider.gameObject;
					_T component = obj.GetComponentInParent<_T>( );

					Assert.IsNotNull( component,
						"Raycast found an object (" + obj + ") but it did not have a " + typeof( _T ) + " Component" );

					if ( component == null )
						return;

					// We want the last hit (closest to final position)
					// Calling Remove on key that doesn't exist is safe
					PriorityHitCheck.Remove( component );
					PriorityHitCheck.Add( component, firstHit );
					//m_lastHit = obj.GetComponent<_T>( );
				}
			}

			protected void ProcessRaycastResults( )
			{
				for ( int i = 0; i < PriorityHitCheck.Count; ++i )
				{
					var pair = PriorityHitCheck.GetItem( i );

					if ( PreviousPriorityHitCheck.ContainsKey( pair.Key ) == false )
					{
						ObjectFocusGained( pair );
					}
				}

				for ( int i = 0; i < PreviousPriorityHitCheck.Count; ++i )
				{
					var pair = PreviousPriorityHitCheck.GetItem( i );
					_T component = pair.Key;

					if ( PriorityHitCheck.ContainsKey( component ) == false )
					{
						ObjectFocusLost( component );
					}
				}
			}
		}
	}


}