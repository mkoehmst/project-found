using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ProjectFound.Environment.Props
{


	public class ClearanceCapsule : MonoBehaviour
	{
		public CapsuleCollider CapsuleCollider { get; private set; }

		// Use this for initialization
		void Start( )
		{
			MeshFilter mesh = GetComponent<MeshFilter>( );
			Bounds bounds = mesh.sharedMesh.bounds;

			gameObject.GetComponent<Collider>( ).enabled = false;

			CapsuleCollider = gameObject.AddComponent<CapsuleCollider>( );

			CapsuleCollider.radius = Mathf.Max( bounds.extents.x, bounds.extents.z );
			CapsuleCollider.height = bounds.extents.y;
			//CapsuleCollider.isTrigger = true;
		}

		// Update is called once per frame
		void Update( )
		{

		}

		public void Cleanup( )
		{
			enabled = false;
			Component.Destroy( this );

			CapsuleCollider coll = GetComponent<CapsuleCollider>( );
			coll.enabled = false;
			Component.Destroy( coll );

			GetComponent<Collider>( ).enabled = true;
		}
	}


}
