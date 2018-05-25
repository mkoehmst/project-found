using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using ProjectFound.Misc;

namespace ProjectFound.Environment.Props
{


	public class Placement : MonoBehaviour
	{
		private const float m_placementElevation = .005f;
		private const float m_minYNormal = 0.8f;
		private const float m_lerpMoveSpeed = 10f; // 10 meters / s
		private const float m_lerpRotateSpeed = 60f; // 60 degrees / s

		private Vector3 m_placementPosition;
		private float m_dragDistance;
		private float m_ratioTraversed;

		private Vector3 m_startingPosition;
		private Quaternion m_startingRotation;
		//private Vector3 StartingPosition { get; set; }
		//private Quaternion StartingRotation { get; set; }
		private bool DoRejectPlacement { get; set; }
		private bool IsLerpActive { get; set; } = false;

		private cakeslice.Outline Outline { get; set; }
		private Collider ExistingCollider { get; set; }
		//private MeshCollider PlacementCollider { get; set; }
		private Rigidbody Rigidbody { get; set; }

		//public Vector3 Offset { get; private set; }

		void Awake( )
		{
			Outline = GetComponent<cakeslice.Outline>( );
			ExistingCollider = GetComponent<Collider>( );
			//PlacementCollider = gameObject.AddComponent<MeshCollider>( );
			Rigidbody = gameObject.AddComponent<Rigidbody>( );
		}

		void Start( )
		{
			//ExistingCollider.enabled = false;
			ExistingCollider.isTrigger = true;
			Rigidbody.isKinematic = true;
			Rigidbody.useGravity = false;
			//PlacementCollider.convex = true;
			//PlacementCollider.inflateMesh = false;
			//PlacementCollider.isTrigger = true;

			//StartingPosition = transform.position;
			//StartingRotation = transform.rotation;
		}

		void LateUpdate( )
		{
			if ( IsLerpActive )
			{
				float distanceToMove = m_lerpMoveSpeed * Time.deltaTime;
				float distanceRatio = distanceToMove / m_dragDistance;
				m_ratioTraversed += distanceRatio;

				transform.position = Vector3.MoveTowards(
					transform.position, m_startingPosition, distanceToMove );

				transform.localRotation = Quaternion.RotateTowards(
					transform.localRotation, m_startingRotation, m_lerpRotateSpeed );

				if ( Misc.Floater.Equal(
					(transform.position - m_startingPosition).magnitude, 0f ) )
				{
					IsLerpActive = false;
					Cleanup( );
				}
			}
		}

		public void SetStartingTransform( ref Vector3 startingPosition, 
			ref Quaternion startingRotation )
		{
			m_startingPosition = startingPosition;
			m_startingRotation = startingRotation;
		}

		public void Place( ref Vector3 hitPoint, ref Vector3 hitNormal )
		{
			m_placementPosition = hitPoint;

			transform.localPosition = new Vector3( m_placementPosition.x,
				m_placementPosition.y + m_placementElevation, m_placementPosition.z );

			CheckAngle( ref hitNormal );
		}

		public void CheckAngle( ref Vector3 hitNormal )
		{
			if ( !Misc.Floater.GreaterThan( hitNormal.y, m_minYNormal ) )
			{
				// Bad state: Angle too steep
				DoRejectPlacement = true;

				Outline.color = 0;
				//Outline.ToggleOutline( );
				//Outline.enabled = true;
			}
			else
			{
				DoRejectPlacement = false;
				//Outline.ToggleOutline( );
				//Outline.enabled = false;
			}

			var rotationAdjustment = Quaternion.FromToRotation( transform.up, hitNormal );
			var correctRotation = rotationAdjustment * transform.rotation;
			transform.rotation = Quaternion.RotateTowards( transform.rotation, correctRotation, 180f );
		}

		//public void RecordCursorOffset( ref RaycastHit hit )
		//{
		//	Offset = new Vector3(
		//		transform.position.x - hit.point.x, transform.position.y - hit.point.y, transform.position.z - hit.point.z );
		//}

		public void ValidatePlacement( )
		{
			m_dragDistance = (transform.position - m_startingPosition).magnitude;

			if ( DoRejectPlacement == true )
			{
				IsLerpActive = true;
			}
			else
			{
				IsLerpActive = false;

				transform.localPosition -= new Vector3( 0f, m_placementElevation, 0f );

				Cleanup( );
			}
		}

		void OnTriggerEnter( Collider other )
		{ 
			DoRejectPlacement = true;

			Outline.color = 0;
		}

		void OnTriggerExit( Collider other )
		{
			DoRejectPlacement = false;

			Outline.color = 1;
		}

		private void Cleanup( )
		{
			Misc.SmartDestroy.Destroy( this );
			//Misc.SmartDestroy.Destroy( PlacementCollider );
			Misc.SmartDestroy.Destroy( Rigidbody );

			//ExistingCollider.enabled = true;
			ExistingCollider.isTrigger = false;
			//Outline.ToggleOutline( );
			//Outline.enabled = false;
			Outline.color = 1;
		}
	}


}
