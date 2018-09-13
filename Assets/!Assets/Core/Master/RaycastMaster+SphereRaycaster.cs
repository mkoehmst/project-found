namespace ProjectFound.Core.Master
{ 


	using UnityEngine;
	using System.Collections;

	using ProjectFound.Interaction;

	public partial class RaycastMaster
	{
		public class SphereRaycaster<_T> : Raycaster<_T>
			where _T : MonoBehaviour
		{
			public delegate void CasterAssignmentDelegate( out Ray caster );
			public delegate void HitFoundDelegate( ref RaycastHit hit, _T component );

			Ray _caster;

			public CasterAssignmentDelegate DelegateCasterAssignment { get; set; }
			public HitFoundDelegate DelegateHitFound { get; set; }

			public bool HasDetection
			{
				get { return PreviousPriorityHitCheck.Count > 0; }
			}

			public SphereRaycaster( RaycastMode mode, float maxDistance, bool isEnabled )
				: base( mode, maxDistance, isEnabled )
			{}

			public override void Cast( )
			{
				DelegateCasterAssignment( out _caster );

				PerformRaycast( );
				ProcessRaycastResults( );
				CycleHitCheck( );
			}

			public override void Clear( )
			{
				PriorityHitCheck.Clear( );
				ProcessRaycastResults( );
				PreviousPriorityHitCheck.Clear( );
			}

			private bool PerformRaycast( )
			{
				RaycastHit firstHit;

				bool didFindHit = 
					Physics.BoxCast( _caster.origin, new Vector3( .6f, 2f, 1f ), _caster.direction, 
						out firstHit, Quaternion.identity, MaxDistance, LayerMask );
	
				if ( !didFindHit )
					return false;

				GameObject obj = firstHit.collider.gameObject;
				if ( WasBlockerHit( obj ) )
					return false;
				
				_T component = obj.GetComponentInParent<_T>( );
				if ( component == null )
					return false;

				PriorityHitCheck.Add( component, firstHit );
	
				return true;
			}

			private void ProcessRaycastResults( )
			{

				//Debug.Log("1) Previous Priority Hit Count: " + PreviousPriorityHitCheck.Count);
				for ( int i = 0; i < PreviousPriorityHitCheck.Count; ++i )
				{
					var pair = PreviousPriorityHitCheck.GetItem( i );

					if ( PriorityHitCheck.ContainsKey( pair.Key ) == false )
					{
						//Debug.Log( Mode + "Raycaster ObjectFocusLost() " + pair.Key );
						ObjectFocusLost( pair );
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
		}
	}


}