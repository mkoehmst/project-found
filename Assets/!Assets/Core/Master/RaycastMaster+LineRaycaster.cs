using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.EventSystems;

using mattmc3.dotmore.Collections.Generic;

using ProjectFound.Environment;

namespace ProjectFound.Master
{


	public partial class RaycastMaster
	{
		public LineRaycaster<_T> AddLineRaycaster<_T>(
			RaycastMode mode, float maxDistance, int resolution, bool isEnabled = true )
			where _T : MonoBehaviour
		{
			LineRaycaster<_T> raycaster =
				new LineRaycaster<_T>( mode, maxDistance, resolution, isEnabled );

			Raycasters[mode] = raycaster;

			return raycaster;
		}

		public class LineRaycaster<_T> : Raycaster<_T>
			where _T : MonoBehaviour
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
				const int minResolution = 1;
				const int maxResolution = 8;

				int clampedResolution = Mathf.Clamp( resolution, minResolution, maxResolution );

				m_pixelResolution = 2 + ((maxResolution - clampedResolution) * 2);
			}

			public override void Cast( )
			{
				DelegateLineTracking( ref m_lineStart, ref m_lineEnd );

				//Debug.Log("LineStart: " + m_lineStart + " LineEnd: " + m_lineEnd);

				Vector3 lineVector = m_lineEnd - m_lineStart;
				Vector3 lineDirection = lineVector.normalized;

				float lineLength = lineVector.magnitude; // 48.0f
				float lineGap = 0f;
				 
				int numPoints = Mathf.RoundToInt( lineLength / m_pixelResolution );
				if ( numPoints == 0 )
				{
					numPoints = 1;
				}

				if ( numPoints > 1 )
				{
					numPoints = (numPoints > m_maxPoints) ? m_maxPoints : numPoints;
					// Either use numPoints to avoid raycasting at lineEnd or 
					// numPoints - 1 to do raycast at lineEnd
					
				}

				lineGap = lineLength / numPoints;

				//	float lineGap = lineLength / (numPoints - 1);

				//float lineGap = lineLength / m_pixelResolution; // 48.0f / 4 = 12.0f

				//	int numPoints = Mathf.RoundToInt( lineGap ) + 1; // Add one to ensure at least one
				//numPoints = (numPoints > m_maxPoints) ? m_maxPoints : numPoints;

				//Debug.Log( numPoints );

				_T lastComponentHit = null;

				if ( numPoints == 1)
				{ // Unroll a single point line raycaster for optimization
					//Debug.Log("Current mouse position: " + Input.mousePosition);
					Vector3 linePos = m_lineEnd;
					//Debug.Log("linePos: " + linePos);

					DelegateCasterAssignments( ref m_raycasters[0], ref linePos );
					lastComponentHit = PerformRaycast( ref m_raycasters[0] );
				}
				else 
				{ 
					for ( int i = 0; i < numPoints; ++i )
					{
						//Debug.Log("Current mouse position: " + Input.mousePosition);
						Vector3 linePos = m_lineStart + (lineDirection * lineGap * (i+1));
						//Debug.Log("linePos: " + linePos);

						DelegateCasterAssignments( ref m_raycasters[i], ref linePos );

						lastComponentHit = PerformRaycast( ref m_raycasters[i] );
					}
				}

				// Take any previously hit components out of the priority hit check if
				// they are not the last hit component.
				
				for (int i = 0; i < PriorityHitCheck.Count; ++i)
				{
					var pair = PriorityHitCheck.GetItem(i);
					_T componentHit = pair.Key;

					if (PreviousPriorityHitCheck.ContainsKey(componentHit))
					{
						if (lastComponentHit != componentHit)
						{
							// i-- compensates for moving all remaining indexes down
							PriorityHitCheck.RemoveAt(i--);
						}
					}
				}

				ProcessRaycastResults( );
				CycleHitCheck( );
			}

			protected _T PerformRaycast( ref Ray ray )
			{
				RaycastHit firstHit;

				bool success =
					Physics.Raycast( ray, out firstHit, MaxDistance, LayerMask );

				if ( success == true )
				{
					if ( WasBlockerHit( ref firstHit ) == true )
					{
						return null;
					}

					GameObject obj = firstHit.collider.gameObject;
					_T component = obj.GetComponentInParent<_T>( );

					Assert.IsNotNull( component,
						"Raycast found an object (" + obj + ") but it did not have a " + typeof( _T ) + " Component" );

					if ( component == null )
						return null;

					// We want the last hit (closest to final position)
					// Calling Remove on key that doesn't exist is safe
					PriorityHitCheck.Remove( component );
					PriorityHitCheck.Add( component, firstHit );
					//m_lastHit = obj.GetComponent<_T>( );

					return component;
				}

				return null;
			}

			protected void ProcessRaycastResults( )
			{
				//Debug.Log("1) Previous Priority Hit Count: " + PreviousPriorityHitCheck.Count);
				for ( int i = 0; i < PreviousPriorityHitCheck.Count; ++i )
				{
					var pair = PreviousPriorityHitCheck.GetItem( i );
					_T component = pair.Key;

					//var phc = PriorityHitCheck;
					//var pphc = PreviousPriorityHitCheck;

					if ( PriorityHitCheck.ContainsKey( component ) == false )
					{
						//Debug.Log( Mode + "Raycaster ObjectFocusLost() " + pair.Key );
						ObjectFocusLost( component );
					}
				}

				//Debug.Log("2) Priority Hit Count: " + PriorityHitCheck.Count);
				for ( int i = 0; i < PriorityHitCheck.Count; ++i )
				{
					var pair = PriorityHitCheck.GetItem( i );

					if ( PreviousPriorityHitCheck.ContainsKey( pair.Key ) == false )
					{
						//Debug.Log(Mode + "Raycaster ObjectFocusGained() " + pair.Key);
						ObjectFocusGained( pair );
					}
				}
			}

			protected bool WasBlockerHit( ref RaycastHit hit )
			{
				LayerID layer = (LayerID)hit.collider.gameObject.layer;

				return Blockers.Contains( layer );
			}
		}
	}


}