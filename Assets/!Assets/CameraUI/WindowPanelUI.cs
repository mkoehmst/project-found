using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

namespace ProjectFound.CameraUI {


	public class WindowPanelUI : MonoBehaviour
	{
		[SerializeField] Vector2 m_sizeRelativeToParent = new Vector2( 1f, 1f );

		private GameObject m_parentObject;
		private RectTransform m_parentRect;
		private RectTransform m_rect;

		void Start( )
		{
			Assert.AreNotEqual( m_sizeRelativeToParent, Vector2.zero, "Cannot have zero size" );

			m_parentObject = this.transform.parent.gameObject;
			m_parentRect = m_parentObject.GetComponent<RectTransform>( );
			m_rect = this.GetComponent<RectTransform>( );

			float parentX = m_parentRect.sizeDelta.x;
			float parentY = m_parentRect.sizeDelta.y;
			float sizeX = Mathf.Round( parentX * m_sizeRelativeToParent.x );
			float sizeY = Mathf.Round( parentY * m_sizeRelativeToParent.y );

			m_rect.localScale = new Vector3( 1f, 1f, 1f );
			m_rect.sizeDelta = new Vector2( sizeX, sizeY );
		}
	}


}
