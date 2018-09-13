namespace ProjectFound.Interaction
{


	using UnityEngine;
	using UnityEngine.Assertions;

	using ProjectFound.Environment.Props;
	using ProjectFound.Interaction;

	using Autelia.Serialization;

	public class Placeable : Attributable
	{
		private const float m_placementElevation = .005f;
		private const float m_minYNormal = 0.8f;
		private const float m_lerpMoveSpeed = 10f; // 10 meters / s
		private const float m_lerpRotateSpeed = 60f; // 60 degrees / s

		private float _ratioTraversed;
		private float _dragDistance;

		private Vector3 _startingPosition;
		private Quaternion _startingRotation;
		private Vector3 m_placementPosition;

		private bool _doRejectPlacement;
		private bool _isLerpActive = false;

		private cakeslice.Outline[] Outlines { get; set; }
		private Collider ExistingCollider { get; set; }
		//private MeshCollider PlacementCollider { get; set; }
		private Rigidbody Rigidbody { get; set; }

		private bool _didAddRigidbody = false;
		private bool _prevRigidbodyIsKinematic;
		private bool _prevRigidbodyUseGravity;
		private bool _prevColliderIsTrigger;

		[SerializeField] HandlerChain _placementChain;
		
		[Header("Optional")]
		[SerializeField] Transform _placementPivot;
		
		new void Awake( )
		{
			base.Awake( );

			if (Serializer.IsLoading) return;

			Assert.IsNotNull( _placementChain );

			Outlines = GetComponentsInChildren<cakeslice.Outline>( );
			Assert.IsNotNull( Outlines, gameObject.ToString( ) );


			ExistingCollider = GetComponentInChildren<Collider>( );
			//PlacementCollider = gameObject.AddComponent<MeshCollider>( );
		}

		void Start( )
		{
			if ( Serializer.IsLoading ) return;

			Interactee.WindowChain = _placementChain;
			Interactee.WindowReleaseChain = _placementChain;
			Interactee.HoldingChain = _placementChain;
			Interactee.HoldingReleaseChain = _placementChain;
		}

		void LateUpdate( )
		{
			if ( Serializer.IsLoading ) return;

			if ( _isLerpActive )
			{
				float distanceToMove = m_lerpMoveSpeed * Time.deltaTime;
				float distanceRatio = distanceToMove / _dragDistance;
				_ratioTraversed += distanceRatio;

				transform.position = Vector3.MoveTowards(
					transform.position, _startingPosition, distanceToMove );

				transform.localRotation = Quaternion.RotateTowards(
					transform.localRotation, _startingRotation, m_lerpRotateSpeed );

				if ( Misc.Floater.Equal(
					(transform.position - _startingPosition).magnitude, 0f ) )
				{
					_isLerpActive = false;
					Cleanup( );
				}
			}
		}

		public void PreparePlacement( )
		{
			Rigidbody = ExistingCollider.GetComponent<Rigidbody>( );
			if ( Rigidbody == null )
			{
				Rigidbody = ExistingCollider.gameObject.AddComponent<Rigidbody>( );
				Rigidbody.isKinematic = true;
				_didAddRigidbody = true;
			}
			else
			{
				_prevRigidbodyIsKinematic = Rigidbody.isKinematic;
				_prevRigidbodyUseGravity = Rigidbody.useGravity;
			}

			_prevColliderIsTrigger = ExistingCollider.isTrigger;

			ExistingCollider.isTrigger = true;
			Rigidbody.isKinematic = true;
			Rigidbody.useGravity = false;
			//PlacementCollider.convex = true;
			//PlacementCollider.inflateMesh = false;
			//PlacementCollider.isTrigger = true;

			_startingPosition = transform.position;
			_startingRotation = transform.rotation;
		}

		public void Place( ref Vector3 hitPoint, ref Vector3 hitNormal )
		{
			if ( _placementPivot != null )
			{ 
				m_placementPosition = new Vector3( 
					hitPoint.x - (_placementPivot.localPosition.x * transform.localScale.x),
					hitPoint.y - (_placementPivot.localPosition.y * transform.localScale.y)
						+ m_placementElevation,
					hitPoint.z - (_placementPivot.localPosition.z * transform.localScale.z) );
			}
			else
			{
				m_placementPosition = new Vector3(
					hitPoint.x,
					hitPoint.y + m_placementElevation,
					hitPoint.z );
			}

			transform.position = m_placementPosition;

			CheckAngle( ref hitNormal );
		}

		public void CheckAngle( ref Vector3 hitNormal )
		{
			if ( !Misc.Floater.GreaterThan( hitNormal.y, m_minYNormal ) )
			{
				// Bad state: Angle too steep
				_doRejectPlacement = true;
			}
			else
			{
				_doRejectPlacement = false;
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
			_dragDistance = (transform.position - _startingPosition).magnitude;

			if ( _doRejectPlacement == true )
			{
				_isLerpActive = true;
			}
			else
			{
				_isLerpActive = false;

				transform.localPosition -= new Vector3( 0f, m_placementElevation, 0f );

				Cleanup( );
			}
		}

		void OnTriggerEnter( Collider other )
		{ 
			_doRejectPlacement = true;

			foreach ( var outline in Outlines )
			{ 
				outline.color = 0;
			}
		}

		void OnTriggerExit( Collider other )
		{
			_doRejectPlacement = false;

			foreach ( var outline in Outlines )
			{ 
				outline.color = 1;
			}
		}

		private void Cleanup( )
		{
			if ( _didAddRigidbody )
			{ 
				Misc.SmartDestroy.Destroy( Rigidbody );
			}
			else
			{
				Rigidbody.isKinematic = _prevRigidbodyIsKinematic;
				Rigidbody.useGravity = _prevRigidbodyUseGravity;
			}

			ExistingCollider.isTrigger = _prevColliderIsTrigger;

			foreach ( var outline in Outlines )
			{
				outline.color = 1;
			}
		}
	}


}
