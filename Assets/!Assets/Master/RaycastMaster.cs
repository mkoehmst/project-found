using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.EventSystems;

using mattmc3.dotmore.Collections.Generic;

using ProjectFound.Environment;
using ProjectFound.Environment.Occlusion;

namespace ProjectFound.Master {


	public partial class RaycastMaster
	{
		//******************************************************************************************
		// RaycastMaster Enums
		//******************************************************************************************
		public enum RaycastMode
		{
			Undefined,
			CursorSelection,
			CombatCursorSelection,
			HoldToMove,
			PropPlacement,
			CameraOcclusion
		}
		//******************************************************************************************
		//******************************************************************************************
		// RaycastMaster Sub-Classes
		// TODO: Make them "collider mesh aware" so that when traversing the child hierarchy, only
		// GameObjects with Collider components are used
		//******************************************************************************************
		//******************************************************************************************
		//******************************************************************************************
		// RaycastMaster Properties
		//******************************************************************************************
		public OrderedDictionary<RaycastMode,Raycaster> Raycasters { get; private set; }
			= new OrderedDictionary<RaycastMode,Raycaster>( );

		public LineRaycaster<Interactee> CursorSelectionRaycaster { get; private set; }
			= new LineRaycaster<Interactee>( RaycastMode.CursorSelection, 30f, 3 );

		public PointRaycaster<Interactee> CombatCursorSelectionRaycaster { get; private set; }
			= new PointRaycaster<Interactee>( RaycastMode.CombatCursorSelection, 30f, false );

		public PointRaycaster<Interactee> HoldToMoveRaycaster { get; private set; }
			= new PointRaycaster<Interactee>( RaycastMode.HoldToMove, 30f, false );

		public PointRaycaster<Interactee> PropPlacementRaycaster { get; private set; }
			= new PointRaycaster<Interactee>( RaycastMode.PropPlacement, 30f, false );

		public OcclusionRaycaster<Occludable> CameraOcclusionRaycaster { get; private set; }
			= new OcclusionRaycaster<Occludable>( RaycastMode.CameraOcclusion, 70f );

		public Raycaster<Interactee> CurrentInteracteeRaycaster { get; set; }
		public InputMaster.InputDevice CursorDevice { get; set; }
		//******************************************************************************************
		//******************************************************************************************
		// RaycastMaster Methods
		//******************************************************************************************
		public void Loop( )
		{
			int count = Raycasters.Values.Count;
			for ( int i = 0; i < count; ++i )
			{
				Raycaster raycaster = Raycasters[i];
				if ( raycaster.IsEnabled == true )
				{
					raycaster.Cast( );
				}
			}
		}

		public void Initialize( )
		{
			Raycasters.Add( RaycastMode.CursorSelection, CursorSelectionRaycaster );
			Raycasters.Add( RaycastMode.CombatCursorSelection, CombatCursorSelectionRaycaster );
			Raycasters.Add( RaycastMode.HoldToMove, HoldToMoveRaycaster );
			Raycasters.Add( RaycastMode.PropPlacement, PropPlacementRaycaster );
			Raycasters.Add( RaycastMode.CameraOcclusion, CameraOcclusionRaycaster );
		}
		//******************************************************************************************
	}


}