namespace ProjectFound.CameraUI 
{


	using UnityEngine;
	using UnityEngine.Assertions;
	using Autelia.Serialization;

	public abstract class PanelWidgetUI : MonoBehaviour
	{
		protected WindowPanelUI m_parentPanel;
		protected RectTransform m_parentPanelRect;

		protected void Start( )
		{
			if (Serializer.IsLoading) return;

			m_parentPanel = GetComponentInParent<WindowPanelUI>( );
			Assert.IsNotNull( m_parentPanel );

			m_parentPanelRect = m_parentPanel.GetComponent<RectTransform>( );
			Assert.IsNotNull( m_parentPanelRect );
		}
	}


}
