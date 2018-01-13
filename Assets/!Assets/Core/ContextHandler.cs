using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using ProjectFound.Master;

namespace ProjectFound.Core
{


	public abstract class ContextHandler : ScriptableObject
	{
		protected RaycastMaster	RaycastMaster	{ get; private set; }
		protected InputMaster	InputMaster		{ get; private set; }
		protected PlayerMaster	PlayerMaster	{ get; private set; }
		protected CameraMaster	CameraMaster	{ get; private set; }
		protected UIMaster		UIMaster		{ get; private set; }
		protected ShaderMaster	ShaderMaster	{ get; private set; }
		protected CombatMaster	CombatMaster	{ get; private set; }

		public void AssignMasters( RaycastMaster raycast, InputMaster input, PlayerMaster player,
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
