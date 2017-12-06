/*
    Original author: scheichs, http://forum.unity3d.com/members/scheichs.401530/
    Modified by: Peter Matejovsky, Vertex Soup
 
    Version: 1.02
    Fix: Null reference exception on update last position call.
 
    Version: 1.01
    Added: Use SEGI Follow Transform mode.
    Added: Use Half Voxel Space Size override for distance check.
    Note: Increased range slider for distance to 500 units.
*/
 
using UnityEngine;
 
 
/// <summary>
/// Helper class for SEGI enabling it to update GI either after set time interval or camera move distance,
/// whichever is triggered first.
/// </summary>
public class SEGIHelper : MonoBehaviour
{
 
    [Tooltip("Enabling this option will use SEGI \"Follow Transform\" instead of this one.\nIf there is no Transform assigned on SEGI, distance check will NOT be performed at all.")]
    public bool useSEGIFollowTransform = true;
 
    [Tooltip("Enabling this option will set update distance to half od SEGI Voxel Space Size.\nThis oprion will ignore \"Update Distance\" value.")]
    public bool useHalfVoxelSpaceSize = true;
 
    [Range(0f, 10f)]
    [Tooltip("Regular update interval in seconds.")]
    public float updateInterval = 1f;
 
    [Range(0f, 500f)]
    [Tooltip("Update distance in meters after SEGI is triggered to recalculate GI.")]
    public float updateDistance = 10f;
 
    internal SEGI _segi;
    internal float _nextUpdate = 1f;
    internal bool _warmUp = true;
    internal Vector3 _lastPosition;
    internal float _updateDistanceSquared; // using cached square of distance for some performance gain
 
    void Start()
    {
        if (!_segi)
        {
            _segi = (SEGI)GameObject.FindObjectOfType(typeof(SEGI));
 
            if (!_segi)
            {
                Debug.Log("<color=red>No SEGI in scene to use!</color>");
                return;
            }
        }
 
        SetUpdateDistance(updateDistance);
        SetLastPosition();
    }
 
    void Update()
    {
        if (!_segi)
        {
            return;
        }
 
        CheckUpdateDistance();
 
        if (_segi.updateGI && !_warmUp)
        {
            _segi.updateGI = false;
        }
 
        _nextUpdate -= Time.deltaTime;
 
        if (_nextUpdate < 0)
        {
            RefreshSEGI();
        }
    }
 
    void OnDisable()
    {
        _segi.updateGI = true;
    }
 
    /// <summary>
    /// Enable to dynamicaly set update distance from other scripts.
    /// </summary>
    /// <param name="distance">Distance in world units (Meters).</param>
    public void SetUpdateDistance(float distance)
    {
        updateDistance = (useHalfVoxelSpaceSize) ? Mathf.Min(_segi.voxelSpaceSize * .5f, distance) : distance;
        _updateDistanceSquared = updateDistance * updateDistance;
    }
 
    /// <summary>
    /// Assign target SEGI from external call.
    /// </summary>
    /// <param name="target">SEGI MonoBehaviour</param>
    public void SetTargetSEGI(SEGI target)
    {
        _segi = target;
        SetLastPosition();
    }
 
    internal void CheckUpdateDistance()
    {
        if (useSEGIFollowTransform )
        {
            if (_segi.followTransform == null) return;
        }
 
        float distance;
        distance = Vector3.SqrMagnitude(_lastPosition - ((useSEGIFollowTransform) ? _segi.followTransform.position : _segi.transform.TransformPoint(_segi.transform.position)));
 
        if ( distance > _updateDistanceSquared)
        {
            SetLastPosition();
            RefreshSEGI();
        }
    }
 
    internal void SetLastPosition()
    {
        _lastPosition = (_segi.followTransform != null && useSEGIFollowTransform) ? _segi.followTransform.position : _segi.transform.TransformPoint(_segi.transform.position);
    }
 
    internal void RefreshSEGI()
    {
        _segi.updateGI = true;
        _nextUpdate = updateInterval;
        _warmUp = false;
    }
}
 