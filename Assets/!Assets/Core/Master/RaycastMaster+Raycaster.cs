using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.EventSystems;

using mattmc3.dotmore.Collections.Generic;

using ProjectFound.Environment;

namespace ProjectFound.Master
{


	public partial class RaycastMaster
	{
		public abstract class Raycaster
		{
			public class RaycastReport
			{
				public RaycastMode		mode;
				public Vector3			hitPoint;
				public Vector3			hitNormal;
				public MonoBehaviour	component;
				public GameObject		gameObj;
				public LayerID			layerID;
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

			protected bool m_isEnabled = false;
			public bool IsEnabled
			{
				get { return m_isEnabled; }
			}

			public RaycastMode Mode { get; private set; } = RaycastMode.Undefined;
			public float MaxDistance { get; set; }
			public int LayerMask { get; protected set; }
			public List<LayerID> Blockers { get; private set; }
			
			public RaycastReport Report { get; private set; } = new RaycastReport( );

			public Raycaster( RaycastMode mode, float maxDistance, bool isEnabled )
			{
				Mode = mode;
				MaxDistance = maxDistance;
				Blockers = new List<LayerID>( );
				m_isEnabled = isEnabled;
			}

			public void AddBlocker( LayerID layer )
			{
				LayerMask |= (1 << (int)layer);
				Blockers.Add( layer );
			}

			public abstract void Cast( );
			public abstract void AddPriority( LayerID layer );
			public abstract void RemovePriority( LayerID layer );
		}

		public abstract class Raycaster<_T> : Raycaster
			where _T : MonoBehaviour
		{
			public delegate void FocusGainedDelegate( KeyValuePair<_T,RaycastHit> pair );
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
				LayerDetails layerDetails = Priority.GetValue( layer );

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

			public ICollection<_T> GetPreviousHitComponents( )
			{
				return PreviousPriorityHitCheck.Keys;
			}

			public KeyValuePair<_T, RaycastHit> GetLastHit( )
			{
				int i = PreviousPriorityHitCheck.Count - 1;

				return PreviousPriorityHitCheck.GetItem( i );
			}

			public _T GetFirstHitComponent( )
			{
				return PriorityHitCheck.GetItem( 0 ).Key;
			}

			protected void ObjectFocusGained( KeyValuePair<_T, RaycastHit> pair )
			{
				GameObject obj = pair.Key.gameObject;

				LayerDetails layerDetails = Priority.GetValue( (LayerID)obj.layer );

				layerDetails.DelegateFocusGained?.Invoke( pair );
			}

			protected void ObjectFocusLost( _T component )
			{
				GameObject obj = component.gameObject;

				LayerDetails layerDetails = Priority.GetValue( (LayerID)obj.layer );

				layerDetails.DelegateFocusLost?.Invoke( component );
			}

			protected void CycleHitCheck( )
			{
				int count = PriorityHitCheck.Count;
				if ( count > 0 )
				{ 
					KeyValuePair<_T,RaycastHit> pair = PriorityHitCheck.GetItem( count - 1 );

					Report.mode			= Mode;
					Report.component	= pair.Key;
					Report.hitPoint		= pair.Value.point;
					Report.hitNormal	= pair.Value.normal;
					Report.gameObj		= pair.Value.collider.gameObject;
					Report.layerID		= (LayerID)Report.gameObj.layer;

					PreviousPriorityHitCheck.Duplicate( PriorityHitCheck );
					PriorityHitCheck.Clear( );
				}
				else
				{
					Report.mode = RaycastMode.Undefined;

					PreviousPriorityHitCheck.Clear( );
				}
			}
		}
	}


}
