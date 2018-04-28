using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace ProjectFound.CameraUI
{


	public class ConductBarUI : MonoBehaviour
	{
		[SerializeField] GameObject m_conductBarButtonPrefab;

		private WidgetItemGridUI m_grid;

		// Use this for initialization
		void Awake( )
		{
			m_grid = GetComponentInChildren<WidgetItemGridUI>( );
		}

		public Button AddConduct( Sprite sprite )
		{
			GameObject slot = m_grid.FirstEmptySlot;

			GameObject obj = GameObject.Instantiate( m_conductBarButtonPrefab, slot.transform );
			Button button = obj.GetComponent<Button>( );
			Image img = obj.GetComponent<Image>( );

			img.sprite = sprite;

			return button;
		}
	}


}
