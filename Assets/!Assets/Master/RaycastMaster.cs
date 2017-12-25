using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using UnityEngine;
using UnityEngine.EventSystems;

using mattmc3.dotmore.Collections.Generic;

namespace ProjectFound.Master {


	public class RaycastMaster
	{
		public class LayerDetails
		{
			public delegate void CursorFocusGainedDelegate(
				KeyValuePair<GameObject,RaycastHit> pair );

			public delegate void CursorFocusLostDelegate( GameObject obj );

			public CursorFocusGainedDelegate DelegateCursorFocusGained;
			public CursorFocusLostDelegate DelegateCursorFocusLost;
		}

		public enum RaycastMode
		{
			Undefined,
			CursorSelection,
			HoldToMove,
			PropPlacement,
			CameraOcclusion
		}

		public class LineRaycaster : Raycaster
		{
			private const int m_maxPoints = 24;

			public delegate void LineTrackingDelegate( ref Vector3 start, ref Vector3 end );
			public delegate void CasterAssignmentsDelegate( ref Ray ray, ref Vector3 screenPos );

			private int m_pixelResolution;
			private Vector3 m_lineStart;
			private Vector3 m_lineEnd;

			private Ray[] m_raycasters = new Ray[m_maxPoints];

			public LineTrackingDelegate DelegateLineTracking { get; set; }
			public CasterAssignmentsDelegate DelegateCasterAssignments { get; set; }

			public LineRaycaster(
				RaycastMode mode, float maxDistance, int resolution, bool isEnabled = true )
					:	base( mode, maxDistance, isEnabled )
			{
				m_pixelResolution = 48 / Mathf.Clamp( resolution, 1, 4 );
			}

			public override void Cast( )
			{
				DelegateLineTracking( ref m_lineStart, ref m_lineEnd );

				Vector3 lineVector = m_lineEnd - m_lineStart;
				Vector3 lineDirection = lineVector.normalized;

				float lineLength = lineVector.magnitude;
				float lineGap = lineLength / m_pixelResolution;

				int numPoints = Mathf.RoundToInt( lineGap ) + 1; // Add one to ensure at least one
				numPoints = (numPoints > m_maxPoints) ? m_maxPoints : numPoints;

				Debug.Log( numPoints );

				for ( int i = 0; i < numPoints; ++i )
				{
					Vector3 linePos = m_lineStart + (lineDirection * lineGap * i);

					DelegateCasterAssignments( ref m_raycasters[i], ref linePos );

					PerformRaycast( ref m_raycasters[i] );
				}

				ProcessRaycastResults( );
				CycleHitCheck( );
			}

			protected void PerformRaycast( ref Ray ray )
			{
				RaycastHit firstHit;

				bool success =
					Physics.Raycast( ray, out firstHit, MaxDistance, LayerMask );

				if ( success == true )
				{
					GameObject obj = firstHit.collider.gameObject;

					if ( PriorityHitCheck.ContainsKey( obj ) == false )
					{
						PriorityHitCheck.Add( obj, firstHit );
					}
				}
			}

			protected void ProcessRaycastResults( )
			{
				for ( int i = 0; i < PriorityHitCheck.Count; ++i )
				{
					var pair = PriorityHitCheck.GetItem( i );

					if ( PreviousPriorityHitCheck.ContainsKey( pair.Key ) == false )
					{
						ObjectFocusGained( pair );
					}
				}

				for ( int i = 0; i < PreviousPriorityHitCheck.Count; ++i )
				{
					var pair = PreviousPriorityHitCheck.GetItem( i );

					if( PriorityHitCheck.ContainsKey( pair.Key ) == false )
					{
						ObjectFocusLost( pair );
					}
				}
			}
		}

		public class OcclusionRaycaster : PointRaycaster
		{
			public delegate void HitsFound( ICollection<GameObject> hits );

			private bool m_isOccluded;

			public HitsFound DelegateHitsFound { get; set; }
			public HitsFound DelegateHitsNotFound { get; set; }
			public Transform Target { get; set; }

			public OcclusionRaycaster( RaycastMode mode, float maxDistance, bool isEnabled = true )
				:	base( mode, maxDistance, isEnabled )
			{
				m_isOccluded = false;
			}

			public override void Cast( )
			{
				DelegateCasterAssignment( ref m_caster );

				RaycastHit[] hits = Physics.RaycastAll( m_caster, MaxDistance, LayerMask );
				if ( hits.Length > 0 )
				{
					Debug.Log( "Camera Occlusion Cast: TRUE!" );

					if ( m_isOccluded == false )
					{
						m_isOccluded = true;

						for ( int i = 0; i < hits.Length; ++i )
						{
							PriorityHitCheck.Add( hits[i].collider.gameObject, hits[i] );
						}

						DelegateHitsFound( PriorityHitCheck.Keys );
					}
				}
				else
				{
					if ( m_isOccluded == true )
					{
						m_isOccluded = false;

						DelegateHitsNotFound( PriorityHitCheck.Keys );

						PriorityHitCheck.Clear( );
					}
				}
			}
		}

		public class PointRaycaster : Raycaster
		{
			public delegate void CasterAssignmentDelegate( ref Ray caster );

			protected Ray m_caster;

			public CasterAssignmentDelegate DelegateCasterAssignment { get; set; }

			public PointRaycaster( RaycastMode mode, float maxDistance, bool isEnabled = true )
				: base( mode, maxDistance, isEnabled )
			{}

			public override void Cast( )
			{
				DelegateCasterAssignment( ref m_caster );

				PerformRaycast( );
				ProcessRaycastResults( );
				CycleHitCheck( );
			}

			private void PerformRaycast( )
			{
				RaycastHit firstHit;
				bool success;

				switch ( Mode )
				{
					case RaycastMode.CursorSelection:
					case RaycastMode.PropPlacement:
					case RaycastMode.HoldToMove:
						// Select first hit that matches any Priority layer
						success =
							Physics.Raycast( m_caster, out firstHit, MaxDistance, LayerMask );
						if ( success == true )
						{
							PriorityHitCheck.Add( firstHit.collider.gameObject, firstHit );
							/*if ( !Misc.Floater.Equal( firstHit.normal.y, 1.0f ) )
							{
								string x = firstHit.normal.x.ToString( "0.000" );
								string y = firstHit.normal.y.ToString( "0.000" );
								string z = firstHit.normal.z.ToString( "0.000" );
								Debug.Log( "(" + x + ", " + y + ", " + z + ")" );
							}*/
							return;
						}
						return;
				}
			}

			private void ProcessRaycastResults( )
			{
				for ( int i = 0; i < PriorityHitCheck.Count; ++i )
				{
					var pair = PriorityHitCheck.GetItem( i );

					if ( PreviousPriorityHitCheck.ContainsKey( pair.Key ) == false )
					{
						ObjectFocusGained( pair );
					}
				}

				for ( int i = 0; i < PreviousPriorityHitCheck.Count; ++i )
				{
					var pair = PreviousPriorityHitCheck.GetItem( i );

					if ( PriorityHitCheck.ContainsKey( pair.Key ) == false )
					{
						ObjectFocusLost( pair );
					}
				}
			}
		}

		public abstract class Raycaster
		{
			public struct Blacklistee
			{
				public GameObject Object { get; private set; }
				public int Layer { get; private set; }

				public Blacklistee( GameObject obj )
				{
					Object = obj;
					Layer = obj.layer;
				}
			}

			private bool m_isEnabled;
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
			}

			public RaycastMode Mode { get; private set; }
			public float MaxDistance { get; set; }
			public int LayerMask { get; private set; }
			public OrderedDictionary<Core.LayerID,LayerDetails> Priority { get; private set; }
			public OrderedDictionary<GameObject,RaycastHit> PriorityHitCheck { get; protected set; }
			public OrderedDictionary<GameObject,RaycastHit> PreviousPriorityHitCheck { get; protected set; }

			public Dictionary<GameObject,Blacklistee> Blacklist { get; private set; }

			public Raycaster( RaycastMode mode, float maxDistance, bool isEnabled )
			{
				Mode						= mode;
				MaxDistance					= maxDistance;
				Priority					= new OrderedDictionary<Core.LayerID,LayerDetails>( );
				PriorityHitCheck			= new OrderedDictionary<GameObject,RaycastHit>( );
				PreviousPriorityHitCheck	= new OrderedDictionary<GameObject,RaycastHit>( );
				Blacklist					= new Dictionary<GameObject,Blacklistee>( );

				// This should be last because it can access the hit check dictionaries
				IsEnabled = isEnabled;
			}

			public void AddPriority( Core.LayerID layer )
			{
				Priority.Add( layer, new LayerDetails( ) );
				LayerMask |= (1 << (int)layer);
			}

			public void RemovePriority( Core.LayerID layer )
			{
				Priority.Remove( layer );
				LayerMask &= ~(1 << (int)layer);
			}

			public void AddBlacklistee( GameObject obj )
			{
				Blacklist[obj] = new Blacklistee( obj );
				obj.layer = (int)Core.LayerID.IgnoreRaycast;
			}

			public void RemoveBlacklistee( GameObject obj )
			{
				obj.layer = Blacklist[obj].Layer;
				Blacklist.Remove( obj );
			}

			public void ClearBlacklist( )
			{
				foreach ( Blacklistee blacklistee in Blacklist.Values )
				{
					blacklistee.Object.layer = blacklistee.Layer;
				}

				Blacklist.Clear( );
			}

			public void SetLayerDelegates( Core.LayerID layer,
				LayerDetails.CursorFocusGainedDelegate gained,
				LayerDetails.CursorFocusLostDelegate lost )
			{
				LayerDetails layerDetails = Priority.GetValue( layer );

				if ( layerDetails != null )
				{
					layerDetails.DelegateCursorFocusGained = gained;
					layerDetails.DelegateCursorFocusLost = lost;
				}
			}

			public abstract void Cast( );
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

			public ICollection<GameObject> GetPreviousHitObjects( )
			{
				return PreviousPriorityHitCheck.Keys;
			}

			public KeyValuePair<GameObject,RaycastHit> GetLastHit( )
			{
				int i = PreviousPriorityHitCheck.Keys.Count - 1;

				return PreviousPriorityHitCheck.GetItem( i );
			}

			public GameObject GetFirstHitObject( )
			{
				return PriorityHitCheck.GetItem( 0 ).Key;
			}

			protected void ObjectFocusGained( KeyValuePair<GameObject,RaycastHit> pair )
			{
				LayerDetails layerDetails = Priority.GetValue( (Core.LayerID)pair.Key.layer );

				if ( layerDetails.DelegateCursorFocusGained != null )
					layerDetails.DelegateCursorFocusGained( pair );
			}

			protected void ObjectFocusLost( KeyValuePair<GameObject,RaycastHit> pair )
			{
				GameObject obj = pair.Key;

				LayerDetails layerDetails = Priority.GetValue( (Core.LayerID)obj.layer );

				if ( layerDetails.DelegateCursorFocusLost != null )
					layerDetails.DelegateCursorFocusLost( obj );
			}

			protected void CycleHitCheck( )
			{
				PreviousPriorityHitCheck.Duplicate( PriorityHitCheck );
				PriorityHitCheck.Clear( );
			}
		}

		public Dictionary<RaycastMode, Raycaster> Raycasters { get; private set; }
		public Raycaster CurrentRaycaster { get; set; }
		//public EventSystem EventSystem { get; private set; }
		public InputMaster.InputDevice CursorDevice { get; set; }

		public RaycastMaster( )
		{
			Raycasters = new Dictionary<RaycastMode, Raycaster>( );
			//EventSystem = GameObject.FindObjectOfType<EventSystem>( );
		}

		public void Loop( )
		{
			foreach ( Raycaster raycaster in Raycasters.Values )
			{
				if ( raycaster.IsEnabled == true )
				{
					raycaster.Cast( );
				}
			}
		}

		public LineRaycaster AddLineRaycaster
			( RaycastMode mode, float maxDistance, int resolution, bool isEnabled = true )
		{
			LineRaycaster raycaster = new LineRaycaster( mode, maxDistance, resolution, isEnabled );

			Raycasters[mode] = raycaster;

			return raycaster;
		}

		public OcclusionRaycaster AddOcclusionRaycaster
			( RaycastMode mode, float maxDistance, Transform target, bool isEnabled = true)
		{
			OcclusionRaycaster raycaster = new OcclusionRaycaster( mode, maxDistance, isEnabled );
			raycaster.Target = target;

			Raycasters[mode] = raycaster;

			return raycaster;
		}

		public PointRaycaster AddPointRaycaster
			( RaycastMode mode, float maxDistance, bool isEnabled = true )
		{
			PointRaycaster raycaster = new PointRaycaster( mode, maxDistance, isEnabled );
			Raycasters[mode] = raycaster;

			return raycaster;
		}

		//private bool IsOverUIElement( )
		//{
		//	return EventSystem.current.IsPointerOverGameObject( );
		//}
	}


}