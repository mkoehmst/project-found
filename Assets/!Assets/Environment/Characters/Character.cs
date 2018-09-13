namespace ProjectFound.Environment.Characters
{ 


	using System.Collections.Generic;
	
	using UnityEngine;
	using UnityEngine.AI;
	using Autelia.Serialization;

	using ProjectFound.Interaction;

	[RequireComponent(typeof(NavMeshAgent))]
	public class Character : Interactor 
	{
		[System.NonSerialized] 
		static private Vector3 LocalGroundPlane = new Vector3( 1f, 0f, 1f ).normalized;
		
		private NavMeshAgent _agent;

		[Header("Character Details")]

		[SerializeField] protected float _movementScore;
		public float MovementScore 
		{ 
			get { return _movementScore; }
			set { if ( _movementScore >= 0f ) _movementScore = value; }
		}

		protected Vector3? _movementTarget;
		public Vector3? MovementTarget
		{
			get { return _movementTarget; }
			set { _movementTarget = value; }
		}

		public float MovementSpeed
		{
			get { return _agent.velocity.sqrMagnitude; }
		}

		public float StoppingDistance
		{
			get { return _agent.stoppingDistance; }
		}

		new protected void Awake( )
		{
			base.Awake( );

			if ( Serializer.IsLoading ) return;

			_agent = GetComponent<NavMeshAgent>( );
		}

		public void TranslateMoveTarget( float h, float v, Transform relativeTo )
		{
			Vector3 forward = Vector3.Scale( relativeTo.forward, LocalGroundPlane ).normalized;
			Vector3 movement = v * forward + h * relativeTo.right;
			Vector3 destination = transform.position + movement;

			_movementTarget = destination;
			_agent.SetDestination( destination );
		}

		public void SetMovementTarget( ref Vector3 destination )
		{
			_movementTarget = destination;
			_agent.SetDestination( destination );
		}

		public void ResetMovementTarget( )
		{
			_movementTarget = null;
			_agent.SetDestination( transform.position );
			_agent.velocity = Vector3.zero;
		}
	}


}
