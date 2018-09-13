namespace ProjectFound.Core.Master
{


	using UnityEngine;
	using UnityEngine.Assertions;

	using ProjectFound.Environment;

	public partial class RaycastMaster
	{
		/*public OcclusionRaycaster<_T> AddOcclusionRaycaster<_T>(
			RaycastMode mode, float maxDistance, Transform target, bool isEnabled = true )
			where _T : MonoBehaviour
		{
			OcclusionRaycaster<_T> raycaster =
				new OcclusionRaycaster<_T>( mode, maxDistance, isEnabled );

			Raycasters[mode] = raycaster;

			raycaster.Target = target;

			return raycaster;
		}*/

		//public class OcclusionRaycaster<_T> : Raycaster<_T>
		public class OcclusionRaycaster<_T> : Raycaster<_T>
			where _T : MonoBehaviour
		{
			public delegate void RayAssignmentsDelegate( ref Ray ray, int i );
			public delegate void OcclusionToggle( _T component );

			private const int _rayCount = 9;
			private const int _occlusionCountThreshold = 5;

			private Ray[] _rays = new Ray[_rayCount];
			private bool _isOccluded;

			public OcclusionToggle DelegateOcclusionEnable { get; set; }
			public OcclusionToggle DelegateOcclusionDisable { get; set; }
			public RayAssignmentsDelegate DelegateRayAssignments { get; set; }

			public OcclusionRaycaster( RaycastMode mode, float maxDistance, bool isEnabled = true )
				: base( mode, maxDistance, isEnabled )
			{
				_isOccluded = false;
			}

			public override void Cast( )
			{
				RaycastHit hit = new RaycastHit( );
				
				int noOcclusionCount = 0;
				int yesOcclusionCount = 0;

				int count = _rays.Length;
				for ( int i = 0; i < count; ++i )
				{
					DelegateRayAssignments( ref _rays[i], i );

					bool success = Physics.Raycast( _rays[i], out hit, MaxDistance, LayerMask );

					if ( success == true )
					{
						if ( Blockers.Contains( (LayerID)hit.collider.gameObject.layer ) )
						{
							success = false;
						}
						else
						{
							if ( ++yesOcclusionCount == _occlusionCountThreshold )
							{
								if ( _isOccluded == false )
								{ 
									_isOccluded = true;

									GameObject hitObject = hit.collider.gameObject;
									
									_T hitComponent = hitObject.GetComponentInParent<_T>( );
									Assert.IsNotNull( hitComponent );
									
									if ( hitComponent == null )
										return;

									PriorityHitCheck.Add( hitComponent, hit );

									DelegateOcclusionEnable( hitComponent );
								}

								return;
							}
						}
					}

					if ( success == false )
					{ 
						if ( ++noOcclusionCount == _occlusionCountThreshold )
						{
							if ( _isOccluded == true )
							{ 
								_isOccluded = false;

								DelegateOcclusionDisable( PriorityHitCheck.GetItem( 0 ).Key );

								PriorityHitCheck.Clear( );
							}

							return;
						}
					}
				}
			}
		}
	}


}
