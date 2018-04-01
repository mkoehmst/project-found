using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using ProjectFound.Environment.Characters;

namespace ProjectFound.CameraUI
{


	[RequireComponent( typeof(TMPro.TextMeshProUGUI) )]
	public class CombatAlertUI : FadeInFadeOutTextUI
	{
		new void Start( )
		{
			base.Start( );

			CombatEncounter.singleton.DelegateEncounterBegin += OnCombatBegin;
		}

		public void OnCombatBegin( List<Combatant> combatants )
		{
			//m_tmPro.maxVisibleLines = 4;

			StartCoroutine( FadeInThenOut( 0f, 0.1f, 1f, 4f, 2f, 0f ) );
		}
	}


}
