using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using ProjectFound.Master;

namespace ProjectFound.Core {

public class CombatContext : GameplayContext
{
	public CombatContext( PlayerMaster playerMaster )
		: base( playerMaster )
	{
		Description = Desc.Combat;
	}

	protected override void LoadMouseAndKeyboardMappings( )
	{
		base.LoadMouseAndKeyboardMappings( );
	}
}

}