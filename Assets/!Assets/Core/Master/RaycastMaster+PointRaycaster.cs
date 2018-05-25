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
		public PointRaycaster<_T> AddPointRaycaster<_T>(
			RaycastMode mode, float maxDistance, bool isEnabled = true )
			where _T : MonoBehaviour
		{
			PointRaycaster<_T> raycaster =
				new PointRaycaster<_T>( mode, maxDistance, isEnabled );

			Raycasters[mode] = raycaster;

			return raycaster;
		}

		public class PointRaycaster<_T> : Raycaster<_T>
			where _T : MonoBehaviour
		{
			public delegate void CasterAssignmentDelegate( ref Ray caster );
			public delegate void HitFoundDelegate( ref RaycastHit hit );

			protected Ray m_caster;

			public CasterAssignmentDelegate DelegateCasterAssignment { get; set; }
			public HitFoundDelegate DelegateHitFound { get; set; }

			public PointRaycaster( RaycastMode mode, float maxDistance, bool isEnabled = true )
				: base( mode, maxDistance, isEnabled )
			{ }

			public override void Cast( )
			{
				DelegateCasterAssignment( ref m_caster );

				PerformRaycast( );
				ProcessRaycastResults( );
				CycleHitCheck( );
			}

			private void PerformRaycast( )
			{
				RaycastHit firstHit;
				bool success;

				switch ( Mode )
				{
					case RaycastMode.CursorSelection:
					case RaycastMode.CombatCursorSelection:
					case RaycastMode.PropPlacement:
					case RaycastMode.HoldToMove:
						// Select first hit that matches any Priority layer
						success =
							Physics.Raycast( m_caster, out firstHit, MaxDistance, LayerMask );
						if ( success == true )
						{
							GameObject obj = firstHit.collider.gameObject;
							_T component = obj.GetComponentInParent<_T>( );

							/*Assert.IsNotNull( component,
								"Raycast found an object (" + obj + ") but it did not have a " + typeof( _T ) + " Component" );*/

							if ( component == null )
								return;

							DelegateHitFound?.Invoke( ref firstHit );
							PriorityHitCheck.Add( component, firstHit );
							/*if ( !Misc.Floater.Equal( firstHit.normal.y, 1.0f ) )
							{
								string x = firstHit.normal.x.ToString( "0.000" );
								string y = firstHit.normal.y.ToString( "0.000" );
								string z = firstHit.normal.z.ToString( "0.000" );
								Debug.Log( "(" + x + ", " + y + ", " + z + ")" );
							}*/
							return;
						}
						return;
				}
			}

			private void ProcessRaycastResults( )
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
