using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using ProjectFound.Environment.Characters;

namespace ProjectFound.CameraUI
{


	public class ActionPointUI : MonoBehaviour
	{
		[SerializeField] GameObject m_actionPointPrefab;

		private int m_maxActionPoints;
		private int m_curActionPoints;
		private List<GameObject> m_actionPointObjects;

		public int MaxActionPoints
		{
			get { return m_maxActionPoints; }
			set
			{
				int prevMaxActionPoints = m_maxActionPoints;
				m_maxActionPoints = value;

				if ( prevMaxActionPoints < m_maxActionPoints )
				{
					int difference = m_maxActionPoints - prevMaxActionPoints;
					for ( int i = 0; i < difference; ++i )
					{
						// Hierarcy in regular order
						// List in reverse order (makes adding/removing AP easier)

						GameObject go = GameObject.Instantiate( m_actionPointPrefab, transform );

						RectTransform rectXform = go.GetComponent<RectTransform>( );

						rectXform.localPosition =
							new Vector3( m_actionPointObjects.Count * 33f, 0f );

						go.SetActive( false );

						m_actionPointObjects.Insert( 0, go );
					}
				}
				else if ( prevMaxActionPoints > m_maxActionPoints )
				{
					int difference = prevMaxActionPoints - m_maxActionPoints;
					for ( int i = 0; i < difference; ++i )
					{

						GameObject.Destroy( m_actionPointObjects[0] );
						m_actionPointObjects.RemoveAt( 0 );
					}
				}
			}
		}

		public int CurActionPoints
		{
			get { return m_curActionPoints; }
		}

		void Awake( )
		{
			m_actionPointObjects = new List<GameObject>( );
		}

		void Start( )
		{
			CombatEncounter.Instance.DelegateEncounterBegin += OnCombatBegin;
			MaxActionPoints = 15;
		}

		public void OnCombatBegin( List<Combatant> combatants )
		{
			Combatant player = combatants[0];
			int ap = player.ActionPoints;

			if ( m_curActionPoints != ap )
			{
				if ( m_curActionPoints > ap )
				{
					RemoveActionPoints( m_curActionPoints - ap );
				}
				else
				{
					AddActionPoints( ap - m_curActionPoints );
				}
			}
		}

		public void RemoveActionPoints( int count )
		{
			m_curActionPoints -= count;

			if ( m_curActionPoints < 0 )
			{
				m_curActionPoints = 0;
			}

			for ( int i = 0; i < count; ++i )
			{
				m_actionPointObjects[i].SetActive( false );
			}
		}

		public void AddActionPoints( int count )
		{
			m_curActionPoints += count;

			if ( m_curActionPoints > m_maxActionPoints )
			{
				m_curActionPoints = m_maxActionPoints;
			}

			int margin = m_maxActionPoints - m_curActionPoints;

			for ( int i = 0; i < count; ++i )
			{
				m_actionPointObjects[margin + i].SetActive( true );
			}
		}

		public void MaxOutActionPoints( )
		{
			int difference = m_maxActionPoints - m_curActionPoints;

			m_curActionPoints = m_maxActionPoints;
		}
	}


}
