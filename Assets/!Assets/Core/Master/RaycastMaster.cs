namespace ProjectFound.Core.Master
{


	using mattmc3.dotmore.Collections.Generic;

	using ProjectFound.Interaction;
	using ProjectFound.Environment.Surfaces;

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
			PropProximity,
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

		//public PointRaycaster<Interactee> CombatCursorSelectionRaycaster { get; private set; }
			//= new PointRaycaster<Interactee>( RaycastMode.CombatCursorSelection, 30f, false );

		public PointRaycaster<Interactee> HoldToMoveRaycaster { get; private set; }
			= new PointRaycaster<Interactee>( RaycastMode.HoldToMove, 30f, false );

		public PointRaycaster<Interactee> PropPlacementRaycaster { get; private set; }
			= new PointRaycaster<Interactee>( RaycastMode.PropPlacement, 30f, false );

		public SphereRaycaster<Interactee> PropProximityRaycaster { get; private set; }
			= new SphereRaycaster<Interactee>( RaycastMode.PropProximity, 2.8f, true );

		public OcclusionRaycaster<OccludableSurface> CameraOcclusionRaycaster { get; private set; }
			= new OcclusionRaycaster<OccludableSurface>( RaycastMode.CameraOcclusion, 200f );

		public OrderedDictionary<RaycastMode, Raycaster> Raycasters { get; private set; }
			= new OrderedDictionary<RaycastMode, Raycaster>( );

		public Raycaster<Interactee> CurrentInteracteeRaycaster { get; private set; }
		public Raycaster<Interactee> PreviousInteracteeRaycaster { get; private set; }

		public Raycaster<Interactee>.RaycastReport Report
		{
			get { return CurrentInteracteeRaycaster.Report; }
			set { CurrentInteracteeRaycaster.Report = value; }
		}
		//******************************************************************************************
		//******************************************************************************************
		// RaycastMaster Methods
		//******************************************************************************************			
		public void Loop( )
		{
			int count = Raycasters.Count;
			for ( int i = 0; i < count; ++i )
			{
				Raycaster raycaster = Raycasters[i];

				if ( raycaster.IsEnabled )
				{
					if ( raycaster.TestRaycastCondition != null 
						&& !raycaster.TestRaycastCondition( ) )
					{
						raycaster.IsEnabled = false;
					}
				}
				else
				{
					if ( raycaster.TestRaycastCondition != null 
						&& raycaster.TestRaycastCondition( ) )
					{
						raycaster.IsEnabled = true;
					}
				}

				if ( raycaster.IsEnabled )
				{
					raycaster.Cast( );
				}
				else
				{
					raycaster.Clear( );
				}
			}
		}

		public void Setup( )
		{
			Raycasters.Add( RaycastMode.CursorSelection, CursorSelectionRaycaster );
			//Raycasters.Add( RaycastMode.CombatCursorSelection, CombatCursorSelectionRaycaster );
			Raycasters.Add( RaycastMode.HoldToMove, HoldToMoveRaycaster );
			Raycasters.Add( RaycastMode.PropPlacement, PropPlacementRaycaster );
			Raycasters.Add( RaycastMode.PropProximity, PropProximityRaycaster );
			Raycasters.Add( RaycastMode.CameraOcclusion, CameraOcclusionRaycaster );
		}

		public void Clear( )
		{
			CurrentInteracteeRaycaster.ClearHitChecks( );
			CurrentInteracteeRaycaster.ClearBlacklist( );
		}

		public void Reset( )
		{
			CurrentInteracteeRaycaster.SetEnabled( false );

			PreviousInteracteeRaycaster = null;
			CurrentInteracteeRaycaster = null;
		}

		public Raycaster<Interactee> SwitchTo( 
			Raycaster<Interactee> raycaster, bool doCopyReport = true )
		{
			if ( CurrentInteracteeRaycaster != null )
			{
				PreviousInteracteeRaycaster = CurrentInteracteeRaycaster;
				PreviousInteracteeRaycaster.SetEnabled( false );

				if ( doCopyReport == true )
				{ 
					raycaster.Report.Duplicate( CurrentInteracteeRaycaster.Report );
				}
			}

			CurrentInteracteeRaycaster = raycaster;
			CurrentInteracteeRaycaster.SetEnabled( true );

			return CurrentInteracteeRaycaster;
		}

		public Raycaster<Interactee> SwitchToPrevious( bool doCopyReport = true )
		{
			if ( PreviousInteracteeRaycaster != null )
			{
				if ( doCopyReport == true )
				{ 
					PreviousInteracteeRaycaster.Report.Duplicate( 
						CurrentInteracteeRaycaster.Report );
				}
		
				CurrentInteracteeRaycaster.SetEnabled( false );
				CurrentInteracteeRaycaster = PreviousInteracteeRaycaster;
				CurrentInteracteeRaycaster.SetEnabled( true );

				PreviousInteracteeRaycaster = null;
			}

			return CurrentInteracteeRaycaster;
		}
		//******************************************************************************************
	}

}