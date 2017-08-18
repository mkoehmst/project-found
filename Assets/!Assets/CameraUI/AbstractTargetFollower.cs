using System;
using UnityEngine;

public abstract class AbstractTargetFollower : MonoBehaviour
{
	public enum UpdateType // The available methods of updating are:
	{
		FixedUpdate, // Update in FixedUpdate (for tracking rigidbodies).
		LateUpdate, // Update in LateUpdate. (for tracking objects that are moved in Update)
		ManualUpdate, // user must call to update camera
	}

	[SerializeField] protected Transform m_Target;            // The target object to follow
	[SerializeField] private UpdateType m_UpdateType;         // stores the selected update type

	protected Rigidbody targetRigidbody;

	public Transform Target
	{
		get { return m_Target; }
	}

	protected virtual void Start( )
	{
		// if auto targeting is used, find the object tagged "Player"
		// any class inheriting from this should call base.Start() to perform this action!
		if ( m_Target == null ) return;
		targetRigidbody = m_Target.GetComponent<Rigidbody>( );
	}


	private void FixedUpdate( )
	{
		if ( m_UpdateType == UpdateType.FixedUpdate )
		{
			FollowTarget( Time.deltaTime );
		}
	}


	private void LateUpdate( )
	{
		if ( m_UpdateType == UpdateType.LateUpdate )
		{
			FollowTarget( Time.deltaTime );
		}
	}


	public void ManualUpdate( )
	{
		if ( m_UpdateType == UpdateType.ManualUpdate )
		{
			FollowTarget( Time.deltaTime );
		}
	}

	protected abstract void FollowTarget( float deltaTime );

	public virtual void SetTarget( Transform newTransform )
	{
		m_Target = newTransform;
	}
}
