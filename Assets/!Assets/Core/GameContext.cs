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

		private InputMaster.InputDevice m_inputDevice;
		public InputMaster.InputDevice Device
		{
			get
			{
				return m_inputDevice;
			}

			set
			{
				RaycastMaster.CursorDevice = value;
				m_inputDevice = value;
			}
		}

		public Desc Description { get; set; }

		public RaycastMaster RaycastMaster { get; private set; }
		public InputMaster InputMaster { get; private set; }
		public PlayerMaster PlayerMaster { get; private set; }
		public CameraMaster CameraMaster { get; private set; }
		public UIMaster UIMaster { get; private set; }
		public ShaderMaster ShaderMaster { get; private set; }

		public GameContext( PlayerMaster playerMaster )
		{
			RaycastMaster = new RaycastMaster( );
			InputMaster = new InputMaster( );
			CameraMaster = new CameraMaster( );
			UIMaster = new UIMaster( );
			ShaderMaster = new ShaderMaster( );
			PlayerMaster = playerMaster;

			SetupRaycasters( );
			SetInputTracker( );
			LoadInputMappings( );
		}

		public virtual void Loop( )
		{
			InputMaster.Loop( );
			RaycastMaster.Loop( );
			PlayerMaster.Loop( );
			CameraMaster.Loop( );
		}

		protected void LoadInputMappings( )
		{
			LoadMouseAndKeyboardMappings( );
			//LoadGamepadMappings( );
		}

		protected abstract void SetupRaycasters( );
		protected abstract void SetInputTracker( );
		protected abstract void LoadMouseAndKeyboardMappings( );
		protected abstract void LoadGamepadMappings( );
	}

}
