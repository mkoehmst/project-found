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
			public delegate void CursorFocus( GameObject obj );
			public CursorFocus DelegateCursorFocusGained;
			public CursorFocus DelegateCursorFocusLost;
		}

		public enum RaycastMode
		{
			Undefined,
			CursorSelection,
			HoldToMove,
			PropPlacement,
			CameraOcclusion
		}

		public class OcclusionRaycaster : Raycaster
		{
			public delegate void HitsFound( ref RaycastHit[] hits );

			private bool m_isOccluded;
			private RaycastHit[] m_raycastHits;

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
				switch ( Mode )
				{
					case RaycastMode.CameraOcclusion:
						RaycastHit[] hits = Physics.RaycastAll( Caster, MaxDistance, LayerMask );
						if ( hits.Length > 0 )
						{
							Debug.Log( "Camera Occlusion Cast: TRUE!" );

							if ( m_isOccluded == false )
							{
								m_isOccluded = true;
								m_raycastHits = hits;

								DelegateHitsFound( ref hits );
							}

							return;
						}
						else
						{
							if ( m_isOccluded == true )
							{
								m_isOccluded = false;

								DelegateHitsNotFound( ref m_raycastHits );
							}

							return;
						}
				}
			}
		}

		public class Raycaster
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
						PriorityHitCheck = null;
						PreviousPriorityHitCheck = null;
					}
					m_isEnabled = value;
				}
			}

			public RaycastMode Mode { get; private set; }
			public float MaxDistance { get; set; }
			public int LayerMask { get; private set; }
			public OrderedDictionary<Core.LayerID,LayerDetails> Priority { get; private set; }
			public RaycastHit? PriorityHitCheck { get; set; }
			public RaycastHit? PreviousPriorityHitCheck { get; private set; }
			public Ray Caster { get; set; }

			public Dictionary<GameObject,Blacklistee> Blacklist { get; private set; }

			public Raycaster( RaycastMode mode, float maxDistance, bool isEnabled )
			{
				IsEnabled	= isEnabled;
				Mode		= mode;
				MaxDistance	= maxDistance;
				Priority	= new OrderedDictionary<Core.LayerID,LayerDetails>( );
				Blacklist	= new Dictionary<GameObject,Blacklistee>( );
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
				LayerDetails.CursorFocus gained, LayerDetails.CursorFocus lost )
			{
				LayerDetails layerDetails = Priority.GetValue( layer );

				if ( layerDetails != null )
				{
					layerDetails.DelegateCursorFocusGained = gained;
					layerDetails.DelegateCursorFocusLost = lost;
				}
			}

			public virtual void Cast( )
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
							Physics.Raycast( Caster, out firstHit, MaxDistance, LayerMask );
						if ( success == true )
						{
							UpdateHitCheck( firstHit );
							/*if ( !Misc.Floater.Equal( firstHit.normal.y, 1.0f ) )
							{
								string x = firstHit.normal.x.ToString( "0.000" );
								string y = firstHit.normal.y.ToString( "0.000" );
								string z = firstHit.normal.z.ToString( "0.000" );
								Debug.Log( "(" + x + ", " + y + ", " + z + ")" );
							}*/
							return;
						}
						UpdateHitCheck( null );
						return;

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
				}
			}

			public void UpdateHitCheck( RaycastHit? hit )
			{
				PreviousPriorityHitCheck = PriorityHitCheck;
				PriorityHitCheck = hit;

				CheckForFocusChange( );
			}

			public GameObject GetPreviousHitObject( )
			{
				return PreviousPriorityHitCheck?.collider.gameObject;
			}

			private void CheckForFocusChange( )
			{
				GameObject prevObj = PreviousPriorityHitCheck?.collider.gameObject;
				GameObject curObj = PriorityHitCheck?.collider.gameObject;

				if ( prevObj != curObj )
				{
					ObjectFocusLost( prevObj );
					ObjectFocusGained( curObj );
				}

				/*
				if ( PreviousPriorityHitCheck.HasValue == true && PriorityHitCheck.HasValue == true )
				{
					GameObject prevObj = PreviousPriorityHitCheck.Value.collider.gameObject;
					GameObject curObj = PriorityHitCheck.Value.collider.gameObject;

					if ( prevObj != curObj )
					{
						ObjectFocusLost( prevObj );
						ObjectFocusGained( curObj );
					}
				}
				else if (
					PreviousPriorityHitCheck.HasValue == false && PriorityHitCheck.HasValue == true )
				{
					ObjectFocusGained( PriorityHitCheck.Value.collider.gameObject );
				}
				else if (
					PreviousPriorityHitCheck.HasValue == true && PriorityHitCheck.HasValue == false )
				{
					ObjectFocusLost( PreviousPriorityHitCheck.Value.collider.gameObject );
				}
				*/
			}

			private void ObjectFocusGained( GameObject obj )
			{
				if ( obj != null )
				{
					LayerDetails layerDetails = Priority.GetValue( (Core.LayerID)obj.layer );

					if ( layerDetails.DelegateCursorFocusGained != null )
						layerDetails.DelegateCursorFocusGained( obj );
				}
			}

			private void ObjectFocusLost( GameObject obj )
			{
				if ( obj != null )
				{
					LayerDetails layerDetails = Priority.GetValue( (Core.LayerID)obj.layer );

					if ( layerDetails.DelegateCursorFocusLost != null )
						layerDetails.DelegateCursorFocusLost( obj );
				}
			}
		}

		private Camera Cam { get; set; }

		public Dictionary<RaycastMode, Raycaster> Raycasters { get; private set; }
		public Raycaster CurrentRaycaster { get; set; }
		public EventSystem EventSystem { get; private set; }
		public Rect ScreenRect { get; private set; }
		public Vector3 ScreenCenter { get; private set; }
		public InputMaster.InputDevice CursorDevice { get; set; }

		public RaycastMaster( )
		{
			Raycasters = new Dictionary<RaycastMode, Raycaster>( );
			EventSystem = GameObject.FindObjectOfType<EventSystem>( );
			ScreenRect = new Rect( 0, 0, Screen.width, Screen.height );
			ScreenCenter = new Vector3( Screen.width / 2, Screen.height / 2, 0f );
		}

		public void Loop( )
		{
			if ( Cam == null )
				Cam = Camera.main;

			foreach ( Raycaster raycaster in Raycasters.Values )
			{
				if ( raycaster.IsEnabled == true )
				{
					CastCheck( raycaster );
				}
			}
		}

		public virtual void CastCheck( Raycaster raycaster )
		{
			if ( raycaster.Mode == RaycastMode.CameraOcclusion )
			{
				OcclusionRaycaster occlusionRaycaster = raycaster as OcclusionRaycaster;

				Ray ray = new Ray( );

				ray.origin = Cam.transform.position;
				ray.direction = (occlusionRaycaster.Target.position - ray.origin).normalized;

				occlusionRaycaster.Caster = ray;

			}
			else if ( CursorDevice == InputMaster.InputDevice.MouseAndKeyboard )
			{
				/*if ( IsOverUIElement( ) )
				{
					raycaster.ClearBlacklist( );
					raycaster.IsEnabled = false;
					CurrentRaycaster = Raycasters[RaycastMode.CursorSelection];
					CurrentRaycaster.IsEnabled = true;
					return ;
				}*/

				raycaster.Caster = Cam.ScreenPointToRay( Input.mousePosition );
			}
			else if ( CursorDevice == InputMaster.InputDevice.Gamepad )
			{
				raycaster.Caster = Cam.ScreenPointToRay( ScreenCenter );
			}

			raycaster.Cast( );
		}

		public OcclusionRaycaster AddOcclusionRaycaster
			( RaycastMode mode, float maxDistance, Transform target, bool isEnabled = true)
		{
			OcclusionRaycaster raycaster = new OcclusionRaycaster( mode, maxDistance, isEnabled );
			raycaster.Target = target;

			Raycasters[mode] = raycaster;

			return raycaster;
		}

		public Raycaster AddRaycaster( RaycastMode mode, float maxDistance, bool isEnabled = true )
		{
			Raycaster raycaster = new Raycaster( mode, maxDistance, isEnabled );
			Raycasters[mode] = raycaster;

			return raycaster;
		}

		private bool IsOverUIElement( )
		{
			return EventSystem.current.IsPointerOverGameObject( );
		}
	}


}