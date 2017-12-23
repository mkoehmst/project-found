using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ProjectFound.Environment.Props
{


	public class Placement : MonoBehaviour
	{
		private const float m_placementElevation = .005f;
		private const float m_minYNormal = 0.8f;

		private Vector3 StartingPosition { get; set; }
		private Quaternion StartingRotation { get; set; }
		private bool DoRejectPlacement { get; set; }

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

		public void Place( ref RaycastHit hit )
		{
			Vector3 offsetPoint = hit.point;

			transform.localPosition =
				new Vector3( offsetPoint.x, offsetPoint.y + m_placementElevation, offsetPoint.z );

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

		public void Cleanup( )
		{
			if ( DoRejectPlacement == true )
			{
				transform.position = StartingPosition;
				transform.rotation = StartingRotation;
			}
			else
			{
				transform.localPosition -= new Vector3( 0f, m_placementElevation, 0f );
			}

			Misc.SmartDestroy.Destroy( this );
			Misc.SmartDestroy.Destroy( PlacementCollider );
			Misc.SmartDestroy.Destroy( Rigidbody );

			ExistingCollider.enabled = true;
			Outline.enabled = false;
			Outline.color = 1;
		}

		private void OnTriggerEnter( Collider other )
		{
			// Bad state: Clearance radius collision
			DoRejectPlacement = true;

			Outline.enabled = true;
			Outline.color = 0;
		}

		private void OnTriggerExit( Collider other )
		{
			DoRejectPlacement = false;

			Outline.enabled = false;
			Outline.color = 1;
		}
	}


}
