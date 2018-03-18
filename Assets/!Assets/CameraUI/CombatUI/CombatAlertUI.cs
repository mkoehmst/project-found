using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using ProjectFound.Environment.Characters;

namespace ProjectFound.CameraUI
{

	[RequireComponent( typeof(TMPro.TextMeshProUGUI) )]
	public class CombatAlertUI : MonoBehaviour
	{
		private TMPro.TextMeshProUGUI m_tmPro;

		private bool m_isFadingOut = false;
		private float m_fadeOutAlpha = 0f;

		private bool m_isFadingIn = false;
		private float m_fadeInAlpha = 1f;

		void Awake( )
		{
			m_tmPro = GetComponent<TMPro.TextMeshProUGUI>( );
		}

		void Start( )
		{
			CombatEncounter.singleton.DelegateEncounterBegin += OnCombatBegin;
		}

		void OnEnable( )
		{
			m_tmPro.maxVisibleLines = 0;
			m_tmPro.CrossFadeAlpha( 0f, 0f, true );
		}

		void Update( )
		{
			if ( m_tmPro.maxVisibleLines != 0 )
			{
				if ( m_isFadingOut == false )
				{
					m_isFadingOut = true;
				}
				else if ( Misc.Floater.Equal( m_tmPro.alpha, m_fadeOutAlpha ) )
				{
					m_isFadingOut = false;
					m_tmPro.maxVisibleLines = 0;
				}
			}
		}

		public void OnCombatBegin( List<Combatant> combatants )
		{
			m_tmPro.maxVisibleLines = 4;

			StartCoroutine( FadeInThenOut( 0f, 0.1f, 1f, 4f, 2f, 0f ) );
		}

		private IEnumerator FadeInThenOut( float inStagnation, float inSeconds, float finalInAlpha,
			float outStagnation, float outSeconds, float finalOutAlpha )
		{
			yield return FadeIn( inStagnation, inSeconds, finalInAlpha );
			yield return new WaitForSeconds( inSeconds );
			yield return FadeOut( outStagnation, outSeconds, finalOutAlpha );
		}

		private IEnumerator FadeIn( float stagnation, float seconds, float finalAlpha = 1f )
		{
			m_fadeInAlpha = finalAlpha;

			if ( stagnation != 0f )
			{
				yield return new WaitForSeconds( stagnation );
			}

			m_tmPro.CrossFadeAlpha( finalAlpha, seconds, false );

			yield break;
		}

		private IEnumerator FadeOut( float stagnation, float seconds, float finalAlpha = 0f )
		{
			m_fadeOutAlpha = finalAlpha;

			if ( stagnation != 0f )
			{
				yield return new WaitForSeconds( stagnation );
			}

			m_tmPro.CrossFadeAlpha( finalAlpha, seconds, false );

			yield break;
		}
	}


}
