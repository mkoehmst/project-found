using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace ProjectFound.CameraUI
{


	public class ActionBarUI : MonoBehaviour
	{
		[SerializeField] GameObject m_actionBarButtonPrefab;

		private WidgetItemGridUI m_grid;

		void Awake( )
		{
			m_grid = GetComponentInChildren<WidgetItemGridUI>( );
		}

		// Use this for initialization
		void Start () {

		}

		// Update is called once per frame
		void Update () {

		}

		public Button AddAction( Sprite sprite )
		{
			GameObject slot = m_grid.FirstEmptySlot;

			GameObject obj = GameObject.Instantiate( m_actionBarButtonPrefab, slot.transform );
			Button button = obj.GetComponent<Button>( );
			Image img = obj.GetComponent<Image>( );

			img.sprite = sprite;

			return button;
		}
	}


}
