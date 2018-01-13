using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using ProjectFound.Master;

namespace ProjectFound.Core {

	public abstract class GameContext
	{
		public enum Desc
		{
			Noncombat,
			Combat//,
				  //Dialogue,
				  //Inventory,
				  //Menu,
				  //Unknown
		}

		public Desc Description { get; set; }

		public RaycastMaster RaycastMaster { get; private set; }
		public InputMaster InputMaster { get; private set; }
		public PlayerMaster PlayerMaster { get; private set; }
		public CameraMaster CameraMaster { get; private set; }
		public UIMaster UIMaster { get; private set; }
		public ShaderMaster ShaderMaster { get; private set; }
		public CombatMaster CombatMaster { get; private set; } = new CombatMaster( );

		public GameContext( PlayerMaster playerMaster )
		{
			RaycastMaster = new RaycastMaster( );
			InputMaster = new InputMaster( );
			CameraMaster = new CameraMaster( );
			UIMaster = new UIMaster( );
			ShaderMaster = new ShaderMaster( );
			PlayerMaster = playerMaster;

			Setup( );
			//SetupConductBar( );
			SetupRaycasters( );
			SetInputTracker( );
			SetCombatDelegates( );
			LoadInputMappings( );
		}

		public virtual void Loop( )
		{
			InputMaster.Loop( );
			RaycastMaster.Loop( );
			PlayerMaster.Loop( );
			CameraMaster.Loop( );
			CombatMaster.Loop( );
		}

		protected void LoadInputMappings( )
		{
			LoadMouseAndKeyboardMappings( );
			LoadGamepadMappings( );
		}

		protected abstract void Setup( );

		//protected abstract void SetupSkills( );
		//protected abstract void SetupConductBar( );
		protected abstract void SetupRaycasters( );
		protected abstract void SetInputTracker( );
		protected abstract void SetCombatDelegates( );
		protected abstract void LoadMouseAndKeyboardMappings( );
		protected abstract void LoadGamepadMappings( );
	}

}
