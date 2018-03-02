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
			/*private bool m_isEnabled;
			public bool IsEnabled
			{
				get { return m_isEnabled; }
				set
				{
					if ( value == false )
					{
						PriorityHitCheck.Clear( );
						PreviousPriorityHitCheck.Clear( );
					}
					m_isEnabled = value;
				}
			}*/

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

			public bool IsEnabled { get; set; } = false;
			public RaycastMode Mode { get; private set; } = RaycastMode.Undefined;
			public float MaxDistance { get; set; }
			public int LayerMask { get; protected set; }
			public List<LayerID> Blockers { get; private set; }

			public Raycaster( RaycastMode mode, float maxDistance, bool isEnabled )
			{
				Mode = mode;
				MaxDistance = maxDistance;
				IsEnabled = isEnabled;
				Blockers = new List<LayerID>( );
			}

			public void AddBlocker( LayerID layer )
			{
				LayerMask |= (1 << (int)layer);
				Blockers.Add( layer );
			}

			//public KeyValuePair<_T,RaycastHit> GetLastHit<_T>( )
			//{
			//	return
			//}

			public abstract void Cast( );
			public abstract void AddPriority( LayerID layer );
			public abstract void RemovePriority( LayerID layer );
		}

		public abstract class Raycaster<_T> : Raycaster
			where _T : Component
		{
			public delegate void FocusGainedDelegate( KeyValuePair<_T,RaycastHit> pair );
			public delegate void FocusLostDelegate( _T component );

			public class LayerDetails
			{
				public FocusGainedDelegate DelegateFocusGained;
				public FocusLostDelegate DelegateFocusLost;
			}

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
				// This should be last because it can access the hit check dictionaries
				//IsEnabled = isEnabled;
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




			//{
			//RaycastHit firstHit;
			//bool success;

			//switch ( Mode )
			//{
			//case RaycastMode.CursorSelection:
			//case RaycastMode.PropPlacement:
			//case RaycastMode.HoldToMove:
			// Select first hit that matches any Priority layer
			//success =
			//Physics.Raycast( Caster, out firstHit, MaxDistance, LayerMask );
			//if ( success == true )
			//{
			//UpdateHitCheck( firstHit );
			/*if ( !Misc.Floater.Equal( firstHit.normal.y, 1.0f ) )
			{
				string x = firstHit.normal.x.ToString( "0.000" );
				string y = firstHit.normal.y.ToString( "0.000" );
				string z = firstHit.normal.z.ToString( "0.000" );
				Debug.Log( "(" + x + ", " + y + ", " + z + ")" );
			}*/
			//return;
			//}
			//UpdateHitCheck( null );
			//return;

			/*case RaycastMode.HoldToMove:
				// Select the closest hit of the highest Priority layer hit
				RaycastHit[] hits = Physics.RaycastAll( Caster, MaxDistance, LayerMask );
				if ( hits.Length == 0 || Priority.Count == 0 )
				{
					//PriorityHitCheck = null;
					UpdateHitCheck( null );
					return;
				}
				// TODO More elegant way to check top priority hit
				foreach ( Core.LayerID layer in Priority.Keys )
				{
					var matchedHits = new List<RaycastHit>( );
					foreach ( RaycastHit hit in hits )
					{
						GameObject hitObj = hit.collider.gameObject;
						if ( hitObj.layer == (int)layer )
						{
							matchedHits.Add( hit );
						}
					}
					int matchCount = matchedHits.Count;
					if ( matchCount > 0 )
					{
						Vector3 casterPos = Caster.origin;
						float closestDistance = float.MaxValue;
						int closestIndex = default( int );
						for ( int i = 0; i < matchCount; ++i )
						{
							float distance =
								(casterPos - matchedHits[i].transform.position).magnitude;
							if ( distance < closestDistance )
							{
								closestDistance = distance;
								closestIndex = i;
							}
						}
						UpdateHitCheck( matchedHits[closestIndex] );
						return;
					}
				}
				UpdateHitCheck( null );
				return;*/
			//}
			//}

			public ICollection<_T> GetPreviousHitComponents( )
			{
				return PreviousPriorityHitCheck.Keys;
			}

			public KeyValuePair<_T, RaycastHit> GetLastHit( )
			{
				int i = PreviousPriorityHitCheck.Keys.Count - 1;

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
				PreviousPriorityHitCheck.Duplicate( PriorityHitCheck );
				PriorityHitCheck.Clear( );
			}
		}
	}


}
