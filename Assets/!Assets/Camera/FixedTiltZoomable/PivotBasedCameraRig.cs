namespace ProjectFound.CameraUI 
{


	using System;

	using UnityEngine;
	using Autelia.Serialization;

	public abstract class PivotBasedCameraRig : AbstractTargetFollower
	{
		protected Transform _cameraTransform; // the transform of the camera
		protected Transform _pivotTransform; // the point at which the camera pivots around
		protected Vector3 m_LastTargetPosition;

		protected virtual void Awake( )
		{
			if (Serializer.IsLoading) return;
			// find the camera in the object hierarchy
			_cameraTransform = GetComponentInChildren<Camera>( ).transform;
			_pivotTransform = _cameraTransform.parent;
		}
	}


}

