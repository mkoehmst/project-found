using System;
using System.Collections.Generic;
using UnityEngine;

namespace ProjectFound.Environment.Characters {


	[RequireComponent( typeof( UnityEngine.AI.NavMeshAgent ) )]
	public class CharacterMovement : MonoBehaviour
	{
		[SerializeField] float m_movingTurnSpeed = 360;
		[SerializeField] float m_stationaryTurnSpeed = 180;
		[SerializeField] float m_moveSpeedMultiplier = 1f;
		[SerializeField] float m_animSpeedMultiplier = 1f;

		private Rigidbody m_rigidBody;
		private Animator m_animator;

		private float m_turnAmount;
		private float m_forwardAmount;

		public UnityEngine.AI.NavMeshAgent Agent { get; private set; }

		void Start( )
		{
			Agent = GetComponent<UnityEngine.AI.NavMeshAgent>( );
			Agent.updateRotation = false;
			Agent.updatePosition = true;
			Agent.SetDestination( transform.position );

			m_animator = GetComponent<Animator>( );
			m_rigidBody = GetComponent<Rigidbody>( );
			m_rigidBody.constraints = RigidbodyConstraints.FreezeRotation;

			CombatEncounter.singleton.DelegateEncounterBegin += OnCombatEncounterBegin;
		}

		void Update( )
		{
			m_animator.speed = m_animSpeedMultiplier;

			if ( Agent.remainingDistance > Agent.stoppingDistance )
			{
				Move( Agent.desiredVelocity );
			}
			else
			{
				Move( Vector3.zero );
			}
		}

		public void SetMoveTarget( Vector3 destination )
		{
			Agent.SetDestination( destination );
		}

		public void ResetMoveTarget( )
		{
			Agent.SetDestination( transform.position );
		}

		public void TranslateMoveTarget( float h, float v )
		{
			if ( h != 0f || v != 0f )
			{
				Transform xform = Camera.main.transform;

				Vector3 camForward = Vector3.Scale( xform.forward, new Vector3( 1, 0, 1 ) ).normalized;
				Vector3 movement = v * camForward + h * xform.right;

				SetMoveTarget( transform.position + movement );
			}
		}

		private void Move( Vector3 movement )
		{
			SetTransform( movement );
			ApplyExtraTurnRotation( );
			UpdateAnimator( );
		}

		private void SetTransform( Vector3 worldSpaceMovement )
		{
			if ( worldSpaceMovement.magnitude > 1f )
			{
				worldSpaceMovement.Normalize( );
			}

			Vector3 localSpaceMovement = transform.InverseTransformDirection( worldSpaceMovement );
			m_turnAmount = Mathf.Atan2( localSpaceMovement.x, localSpaceMovement.z);
			m_forwardAmount = localSpaceMovement.z;
		}

		private void ApplyExtraTurnRotation( )
		{
			float turnSpeed =
				Mathf.Lerp( m_stationaryTurnSpeed, m_movingTurnSpeed, m_forwardAmount );

			transform.Rotate( 0, m_turnAmount * turnSpeed * Time.deltaTime, 0 );
		}

		private void UpdateAnimator( )
		{
			m_animator.SetFloat( "Forward", m_forwardAmount, 0.1f, Time.deltaTime );
			m_animator.SetFloat( "Turn", m_turnAmount, 0.1f, Time.deltaTime );
		}

		private void OnCombatEncounterBegin( List<Combatant> combatants )
		{
			ResetMoveTarget( );
		}

		private void OnAnimatorMove( )
		{
			if ( Time.deltaTime > 0 )
			{
				Vector3 velocity =
					(m_animator.deltaPosition * m_moveSpeedMultiplier) / Time.deltaTime;

				velocity.y = m_rigidBody.velocity.y;
				m_rigidBody.velocity = velocity;
			}
		}

		private void OnDrawGizmos( )
		{
			if ( Agent && Agent.hasPath )
			{
				Gizmos.color = Color.black;
				Gizmos.DrawLine( transform.position, Agent.destination );
				Gizmos.DrawSphere( Agent.destination, 0.08f );
			}
		}
	}


}