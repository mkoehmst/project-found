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
		public OcclusionRaycaster<_T> AddOcclusionRaycaster<_T>(
			RaycastMode mode, float maxDistance, Transform target, bool isEnabled = true )
			where _T : MonoBehaviour
		{
			OcclusionRaycaster<_T> raycaster =
				new OcclusionRaycaster<_T>( mode, maxDistance, isEnabled );

			Raycasters[mode] = raycaster;

			raycaster.Target = target;

			return raycaster;
		}

		public class OcclusionRaycaster<_T> : PointRaycaster<_T>
			where _T : MonoBehaviour
		{
			public delegate void OcclusionToggle( _T component );

			private bool m_isOccluded;

			public OcclusionToggle DelegateOcclusionEnable { get; set; }
			public OcclusionToggle DelegateOcclusionDisable { get; set; }
			public Transform Target { get; set; }

			public OcclusionRaycaster( RaycastMode mode, float maxDistance, bool isEnabled = true )
				: base( mode, maxDistance, isEnabled )
			{
				m_isOccluded = false;
			}

			public override void Cast( )
			{
				DelegateCasterAssignment( ref m_caster );

				RaycastHit hit;
				bool success = Physics.Raycast( m_caster, out hit, MaxDistance, LayerMask );
				if ( success == true )
				{
					if ( m_isOccluded == false )
					{
						m_isOccluded = true;

						//for ( int i = 0; i < hits.Length; ++i )
						//{
						GameObject obj = hit.collider.gameObject;
						_T component = obj.GetComponentInParent<_T>( );

						Assert.IsNotNull( component,
							"Raycast found an object (" + obj + ") but it did not have a " + typeof( _T ) + " Component" );

						if ( component == null )
							return;

						PriorityHitCheck.Add( component, hit );
						//}

						DelegateOcclusionEnable( component );
					}
				}
				else
				{
					if ( m_isOccluded == true )
					{
						m_isOccluded = false;

						DelegateOcclusionDisable( PriorityHitCheck.GetItem( 0 ).Key );

						PriorityHitCheck.Clear( );
					}
				}
			}
		}
	}


}
