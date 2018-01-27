using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using ProjectFound.Master;

namespace ProjectFound.Core
{


	public abstract class ContextHandler : ScriptableObject
	{
		static protected RaycastMaster	RaycastMaster	{ get; private set; }
		static protected InputMaster	InputMaster		{ get; private set; }
		static protected PlayerMaster	PlayerMaster	{ get; private set; }
		static protected CameraMaster	CameraMaster	{ get; private set; }
		static protected UIMaster		UIMaster		{ get; private set; }
		static protected ShaderMaster	ShaderMaster	{ get; private set; }
		static protected CombatMaster	CombatMaster	{ get; private set; }

		static public void AssignMasters( RaycastMaster raycast, InputMaster input, PlayerMaster player,
			CameraMaster camera, UIMaster ui, ShaderMaster shader, CombatMaster combat )
		{
			RaycastMaster = raycast;
			InputMaster = input;
			PlayerMaster = player;
			CameraMaster = camera;
			UIMaster = ui;
			ShaderMaster = shader;
			CombatMaster = combat;
		}
	}


}
