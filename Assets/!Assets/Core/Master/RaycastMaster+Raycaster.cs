namespace ProjectFound.Core.Master
{


	using System.Collections.Generic;
	using mattmc3.dotmore.Collections.Generic;

	using UnityEngine;

	using ProjectFound.Environment;

	public partial class RaycastMaster
	{
		public abstract class Raycaster
		{
			public class RaycastReport
			{
				public RaycastMode		Mode;
				public Vector3			HitPoint;
				public Vector3			HitNormal;
				public MonoBehaviour	Component;
				public GameObject		HitObject;
				public LayerID			HitLayerID;

				public void Duplicate( RaycastReport other )
				{
					Mode = other.Mode;
					HitPoint = other.HitPoint;
					HitNormal = other.HitNormal;
					Component = other.Component;
					HitObject = other.HitObject;
					HitLayerID = other.HitLayerID;
				}
			}

			public struct Blacklistee
			{
				public GameObject m_object;
				public int m_layer;

				public Blacklistee( GameObject obj )
				{
					m_object = obj;
					m_layer = obj.layer;
				}
			}

			public System.Func<bool> TestRaycastCondition;

			protected bool m_isEnabled = false;
			public bool IsEnabled
			{
				get { return m_isEnabled; }
				set { m_isEnabled = value; }
			}

			public RaycastMode Mode { get; set; } = RaycastMode.Undefined;
			public float MaxDistance { get; set; }
			public int LayerMask { get; set; }
			public List<LayerID> Blockers { get; set; } = new List<LayerID>( );
			public RaycastReport Report { get; set; } = new RaycastReport( );

			public Raycaster( RaycastMode mode, float maxDistance, bool isEnabled )
			{
				Mode = mode;
				MaxDistance = maxDistance;
				m_isEnabled = isEnabled;
			}

			public void AddBlocker( LayerID layer )
			{
				LayerMask |= (1 << (int)layer);
				Blockers.Add( layer );
			}

			public abstract void Cast( );
			public virtual void Clear( ) { }
			public abstract void AddPriority( LayerID layer );
			public abstract void RemovePriority( LayerID layer );

			protected bool WasBlockerHit( GameObject hitObject )
			{
				LayerID layer = (LayerID)hitObject.layer;

				return Blockers.Contains( layer );
			}
		}

		public abstract class Raycaster<_T> : Raycaster
			where _T : MonoBehaviour
		{
			public delegate void FocusGainedDelegate( _T component );
			public delegate void FocusLostDelegate( _T component );

			public class LayerDetails
			{
				public FocusGainedDelegate DelegateFocusGained;
				public FocusLostDelegate DelegateFocusLost;
			}

			public KeyValuePair<_T,RaycastHit> m_lastHit;

			public OrderedDictionary<_T,RaycastHit> PriorityHitCheck { get; protected set; }
			public OrderedDictionary<_T,RaycastHit> PreviousPriorityHitCheck { get; protected set; }
			public OrderedDictionary<Transform,Blacklistee> Blacklist { get; private set; }
			public OrderedDictionary<LayerID,LayerDetails> Priority { get; private set; }

			public Raycaster( RaycastMode mode, float maxDistance, bool isEnabled )
				: base( mode, maxDistance, isEnabled )
			{
				PriorityHitCheck = new OrderedDictionary<_T, RaycastHit>( );
				PreviousPriorityHitCheck = new OrderedDictionary<_T, RaycastHit>( );
				Blacklist = new OrderedDictionary<Transform, Blacklistee>( );
				Priority = new OrderedDictionary<LayerID, LayerDetails>( );
			}

			public override void AddPriority( LayerID layer )
			{
				Priority.Add( layer, new LayerDetails( ) );
				LayerMask |= (1 << (int)layer);
			}

			public override void RemovePriority( LayerID layer )
			{
				Priority.Remove( layer );
				LayerMask &= ~(1 << (int)layer);
			}

			public void SetLayerDelegates( LayerID layer, FocusGainedDelegate gained,
				FocusLostDelegate lost )
			{
				LayerDetails layerDetails = Priority[layer];

				if ( layerDetails != null )
				{
					layerDetails.DelegateFocusGained = gained;
					layerDetails.DelegateFocusLost = lost;
				}
			}

			public void SetEnabled( bool isEnabled )
			{
				if ( isEnabled == false && m_isEnabled == true )
				{
					// This is prior to the current frame raycast so PreviousPriorityHitCheck
					// has the latest hit
					PriorityHitCheck.Clear( );
					ClearBlacklist( );
				}

				m_isEnabled = isEnabled;
			}

			public void ClearHitChecks( )
			{
				PreviousPriorityHitCheck.Clear( );
				PriorityHitCheck.Clear( );
			}

			// TODO: How about a generalized Hierarchy Walker class that can take in a delegate
			// to execute on each GameObject?
			public void AddBlacklistee( _T component )
			{
				Transform parent = component.transform;
				Transform walker = parent;
				int childCount = parent.childCount;
				int childIndex = 0;

				while ( walker != null )
				{
					GameObject obj = walker.gameObject;

					Blacklist[walker] = new Blacklistee( obj );
					obj.layer = (int)LayerID.IgnoreRaycast;

					if ( childIndex < childCount )
					{
						walker = parent.GetChild( childIndex++ );
					}
					else
					{
						walker = null;
					}
				}
			}

			public void RemoveBlacklistee( _T component )
			{
				Transform parent = component.transform;
				Transform walker = parent;
				int childCount = parent.childCount;
				int childIndex = 0;

				while ( walker != null )
				{
					Blacklistee blacklistee = Blacklist[walker];
					blacklistee.m_object.layer = blacklistee.m_layer;

					Blacklist.Remove( walker );

					if ( childIndex < childCount )
					{
						walker = parent.GetChild( childIndex++ );
					}
					else
					{
						walker = null;
					}
				}
			}

			public void ClearBlacklist( )
			{
				int count = Blacklist.Count;
				for ( int i = 0; i < count; ++i )
				{
					Blacklistee blacklistee = Blacklist[i];
					blacklistee.m_object.layer = blacklistee.m_layer;
				}

				Blacklist.Clear( );
			}

			public KeyValuePair<_T, RaycastHit> GetLastHit( )
			{
				int i = PreviousPriorityHitCheck.Count - 1;

				return PreviousPriorityHitCheck.GetItem( i );
			}

			protected void ObjectFocusGained( KeyValuePair<_T, RaycastHit> pair )
			{
				GameObject obj = pair.Value.collider.gameObject;

				LayerDetails layerDetails = Priority.GetValue( (LayerID)obj.layer );

				layerDetails.DelegateFocusGained?.Invoke( pair.Key );
			}

			protected void ObjectFocusLost( KeyValuePair<_T, RaycastHit> pair )
			{
				GameObject obj = pair.Value.collider.gameObject;

				LayerDetails layerDetails = Priority.GetValue( (LayerID)obj.layer );

				layerDetails.DelegateFocusLost?.Invoke( pair.Key );
			}

			protected void CycleHitCheck( )
			{
				int count = PriorityHitCheck.Count;
				if ( count > 0 )
				{
					KeyValuePair<_T,RaycastHit> pair = PriorityHitCheck.GetItem( count - 1 );

					Report.Mode			= Mode;
					Report.Component	= pair.Key;
					Report.HitPoint		= pair.Value.point;
					Report.HitNormal	= pair.Value.normal;
					Report.HitObject	= pair.Value.collider.gameObject;
					Report.HitLayerID	= (LayerID)Report.HitObject.layer;

					PreviousPriorityHitCheck.Duplicate( PriorityHitCheck );
					PriorityHitCheck.Clear( );
				}
				else
				{
					Report.Mode = RaycastMode.Undefined;

					PreviousPriorityHitCheck.Clear( );
				}
			}
		}
	}


}
