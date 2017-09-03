using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

namespace ProjectFound.CameraUI {


	public class PanelWidgetUI : MonoBehaviour
	{
		protected WindowPanelUI m_parentPanel;
		protected RectTransform m_parentPanelRect;

		protected void Start( )
		{
			m_parentPanel = GetComponentInParent<WindowPanelUI>( );
			Assert.IsNotNull( m_parentPanel );

			m_parentPanelRect = m_parentPanel.GetComponent<RectTransform>( );
			Assert.IsNotNull( m_parentPanelRect );
		}
	}


}
