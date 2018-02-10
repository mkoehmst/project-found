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

		private Vector3 StartingPosition { get; set; }
		private Quaternion StartingRotation { get; set; }
		private bool DoRejectPlacement { get; set; }
		private bool IsLerpActive { get; set; } = false;

		private cakeslice.Outline Outline { get; set; }
		private Collider ExistingCollider { get; set; }
		private MeshCollider PlacementCollider { get; set; }
		private Rigidbody Rigidbody { get; set; }

		//public Vector3 Offset { get; private set; }

		void Awake( )
		{
			Outline = GetComponent<cakeslice.Outline>( );
			ExistingCollider = GetComponent<Collider>( );
			PlacementCollider = gameObject.AddComponent<MeshCollider>( );
			Rigidbody = gameObject.AddComponent<Rigidbody>( );
		}

		void Start( )
		{
			ExistingCollider.enabled = false;
			Rigidbody.isKinematic = true;
			Rigidbody.useGravity = false;
			PlacementCollider.convex = true;
			PlacementCollider.inflateMesh = false;
			PlacementCollider.isTrigger = true;

			StartingPosition = transform.position;
			StartingRotation = transform.rotation;
		}

		void Update( )
		{
			if ( IsLerpActive )
			{
				float distanceToMove = m_lerpMoveSpeed * Time.deltaTime;
				float distanceRatio = distanceToMove / m_dragDistance;
				m_ratioTraversed += distanceRatio;

				transform.position = Vector3.MoveTowards(
					transform.position, StartingPosition, distanceToMove );

				transform.localRotation = Quaternion.RotateTowards(
					transform.localRotation, StartingRotation, m_lerpRotateSpeed );

				if ( Misc.Floater.Equal(
					(transform.position - StartingPosition).magnitude, 0f ) )
				{
					IsLerpActive = false;
					Cleanup( );
				}
			}
		}

		public void Place( ref RaycastHit hit )
		{
			m_placementPosition = hit.point;

			transform.localPosition = new Vector3( m_placementPosition.x,
				m_placementPosition.y + m_placementElevation, m_placementPosition.z );

			CheckAngle( ref hit );
		}

		public void CheckAngle( ref RaycastHit hit )
		{
			if ( !Misc.Floater.GreaterThan( hit.normal.y, m_minYNormal ) )
			{
				// Bad state: Angle too steep
				DoRejectPlacement = true;

				Outline.color = 0;
				Outline.enabled = true;
			}
			else
			{
				DoRejectPlacement = false;
				Outline.enabled = false;
			}

			var rotationAdjustment = Quaternion.FromToRotation( transform.up, hit.normal );
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
			m_dragDistance = (transform.position - StartingPosition).magnitude;

			if ( DoRejectPlacement == true )
			{
				IsLerpActive = true;
			}
			else
			{
				transform.localPosition -= new Vector3( 0f, m_placementElevation, 0f );

				Cleanup( );
			}
		}

		void OnTriggerEnter( Collider other )
		{
			// Bad state: Clearance radius collision
			DoRejectPlacement = true;

			Outline.enabled = true;
			Outline.color = 0;
		}

		void OnTriggerExit( Collider other )
		{
			DoRejectPlacement = false;

			Outline.enabled = false;
			Outline.color = 1;
		}

		private void Cleanup( )
		{
			Misc.SmartDestroy.Destroy( this );
			Misc.SmartDestroy.Destroy( PlacementCollider );
			Misc.SmartDestroy.Destroy( Rigidbody );

			ExistingCollider.enabled = true;
			Outline.enabled = false;
			Outline.color = 1;
		}
	}


}
