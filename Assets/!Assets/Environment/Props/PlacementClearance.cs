using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ProjectFound.Environment.Props
{


	public class PlacementClearance : MonoBehaviour
	{
		private Collider Collider { get; set; }
		private cakeslice.Outline Outline { get; set; }
		private MeshCollider MeshCollider { get; set; }
		private Rigidbody Rigidbody { get; set; }

		void Start( )
		{
			Outline = GetComponent<cakeslice.Outline>( );
			Collider = GetComponent<Collider>( );
			MeshCollider = gameObject.AddComponent<MeshCollider>( );
			Rigidbody = gameObject.AddComponent<Rigidbody>( );

			Collider.enabled = false;
			Rigidbody.isKinematic = true;
			Rigidbody.useGravity = false;
			MeshCollider.convex = true;
			MeshCollider.inflateMesh = false;
			MeshCollider.isTrigger = true;
		}

		void FixedUpdate( )
		{

		}
		/*
		public void SetClearanceMesh( Mesh clearanceMesh )
		{
			MeshCollider.sharedMesh = clearanceMesh;
		}
		*/
		public void Cleanup( )
		{
			Misc.SmartDestroy.Destroy( this );
			Misc.SmartDestroy.Destroy( MeshCollider );
			Misc.SmartDestroy.Destroy( Rigidbody );

			Collider.enabled = true;
			Outline.enabled = false;
			Outline.color = 1;
		}

		private void OnTriggerEnter( Collider other )
		{
			Outline.enabled = true;
			Outline.color = 0;
		}

		private void OnTriggerExit( Collider other )
		{
			Outline.enabled = false;
			Outline.color = 1;
		}
	}


}
