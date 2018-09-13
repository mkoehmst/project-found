namespace ProjectFound.Core
{


	using UnityEngine;

	using ProjectFound.Core.Master;

	public class ContextHandler : ScriptableObject
	{
		static protected GameContext GameContext { get; private set; }

		static protected RaycastMaster		RaycastMaster		{ get; private set; }
		static protected InputMaster		InputMaster			{ get; private set; }
		static protected PlayerMaster		PlayerMaster		{ get; private set; }
		static protected CameraMaster		CameraMaster		{ get; private set; }
		static protected UIMaster			UIMaster			{ get; private set; }
		static protected ShaderMaster		ShaderMaster		{ get; private set; }
		static protected CombatMaster		CombatMaster		{ get; private set; }
		//static protected InteractionMaster	InteractionMaster	{ get; private set; }

		static public void AssignContext( GameContext context )
		{
			GameContext = context;

			RaycastMaster		= context.RaycastMaster;
			InputMaster			= context.InputMaster;
			PlayerMaster		= context.PlayerMaster;
			CameraMaster		= context.CameraMaster;
			UIMaster			= context.UIMaster;
			ShaderMaster		= context.ShaderMaster;
			CombatMaster		= context.CombatMaster;
			//InteractionMaster	= context.InteractionMaster;
		}
	}


}
