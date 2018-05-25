namespace ProjectFound.Master
{

	using mattmc3.dotmore.Collections.Generic;

	using ProjectFound.Environment;
	using ProjectFound.Environment.Occlusion;

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
		public LineRaycaster<Interactee> CursorSelectionRaycaster { get; private set; }
			= new LineRaycaster<Interactee>( RaycastMode.CursorSelection, 30f, 6 );

		public PointRaycaster<Interactee> CombatCursorSelectionRaycaster { get; private set; }
			= new PointRaycaster<Interactee>( RaycastMode.CombatCursorSelection, 30f, false );

		public PointRaycaster<Interactee> HoldToMoveRaycaster { get; private set; }
			= new PointRaycaster<Interactee>( RaycastMode.HoldToMove, 30f, false );

		public PointRaycaster<Interactee> PropPlacementRaycaster { get; private set; }
			= new PointRaycaster<Interactee>( RaycastMode.PropPlacement, 30f, false );

		public OcclusionRaycaster<Occludable> CameraOcclusionRaycaster { get; private set; }
			= new OcclusionRaycaster<Occludable>( RaycastMode.CameraOcclusion, 70f );

		public OrderedDictionary<RaycastMode, Raycaster> Raycasters { get; private set; }
			= new OrderedDictionary<RaycastMode, Raycaster>( );

		public Raycaster<Interactee> CurrentInteracteeRaycaster { get; set; }
		public Raycaster<Interactee> PreviousInteracteeRaycaster { get; set; }

		public Raycaster<Interactee>.RaycastReport Report
		{
			get { return CurrentInteracteeRaycaster.Report; }
		}
		//******************************************************************************************
		//******************************************************************************************
		// RaycastMaster Methods
		//******************************************************************************************
		public void Loop( )
		{
			var raycasters = Raycasters;
			int count = raycasters.Count;
			for ( int i = 0; i < count; ++i )
			{
				Raycaster raycaster = raycasters[i];

				if ( raycaster.IsEnabled == true )
				{
					raycaster.Cast( );
				}
			}
		}

		public void Setup( )
		{
			Raycasters.Add( RaycastMode.CursorSelection, CursorSelectionRaycaster );
			Raycasters.Add( RaycastMode.CombatCursorSelection, CombatCursorSelectionRaycaster );
			Raycasters.Add( RaycastMode.HoldToMove, HoldToMoveRaycaster );
			Raycasters.Add( RaycastMode.PropPlacement, PropPlacementRaycaster );
			Raycasters.Add( RaycastMode.CameraOcclusion, CameraOcclusionRaycaster );
		}

		public void Reset( )
		{
			CurrentInteracteeRaycaster.ClearHitChecks( );
			CurrentInteracteeRaycaster.ClearBlacklist( );
			CurrentInteracteeRaycaster.SetEnabled( false );

			PreviousInteracteeRaycaster = null;
			CurrentInteracteeRaycaster = null;
		}

		public Raycaster<Interactee> SwitchTo( Raycaster<Interactee> raycaster )
		{
			if ( CurrentInteracteeRaycaster != null )
			{
				PreviousInteracteeRaycaster = CurrentInteracteeRaycaster;
				PreviousInteracteeRaycaster.SetEnabled( false );
			}

			CurrentInteracteeRaycaster = raycaster;
			CurrentInteracteeRaycaster.SetEnabled( true );

			return CurrentInteracteeRaycaster;
		}

		public Raycaster<Interactee> SwitchToPrevious( )
		{
			if ( PreviousInteracteeRaycaster != null )
			{
				CurrentInteracteeRaycaster.SetEnabled( false );
				CurrentInteracteeRaycaster = PreviousInteracteeRaycaster;
				CurrentInteracteeRaycaster.SetEnabled( true );
			}

			return CurrentInteracteeRaycaster;
		}
		//******************************************************************************************
	}

}