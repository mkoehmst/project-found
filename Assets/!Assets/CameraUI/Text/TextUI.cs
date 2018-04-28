using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ProjectFound.CameraUI
{


	public class TextUI : MonoBehaviour
	{
		[SerializeField] protected int m_maxVisibleLines;
		[SerializeField] protected bool m_doRotateToCamera = false;
		[SerializeField] protected bool m_doSyncWorldAnchor = false;

		protected TMPro.TextMeshProUGUI m_tmPro;

		public int MaxVisibleLines
		{
			get { return m_maxVisibleLines; }
			set { m_maxVisibleLines = value; }
		}

		public bool DoRotateToCamera
		{
			get { return m_doRotateToCamera; }
			set { m_doRotateToCamera = value; }
		}

		public Transform WorldAnchor { get; set; }

		protected void Awake( )
		{
			m_tmPro = GetComponentInChildren<TMPro.TextMeshProUGUI>( );

			if ( m_tmPro == null )
			{
				Debug.Log( "No TextMesh Pro UGUI Object found!" );
				return ;
			}
		}

		protected void Start( )
		{
			HideText( );
		}

		protected void LateUpdate( )
		{
			if ( IsVisible( ) == true )
			{
				if ( m_doRotateToCamera == true )
				{
					RotateToCamera( );
				}

				if ( m_doSyncWorldAnchor == true )
				{
					SyncWorldAnchorToScreenSpace( );
				}
			}
		}

		public void DisplayText( string text )
		{
			m_tmPro.SetText( text );
			m_tmPro.maxVisibleLines = m_maxVisibleLines;
		}

		public void DisplayText( )
		{
			m_tmPro.maxVisibleLines = m_maxVisibleLines;
		}

		public void HideText( )
		{
			m_tmPro.maxVisibleLines = 0;
		}

		public bool IsVisible( )
		{
			return m_tmPro.maxVisibleLines != 0;
		}

		public void RotateToCamera( )
		{
			transform.LookAt( Camera.main.transform );
		}

		public void SyncWorldAnchorToScreenSpace( )
		{
			m_tmPro.rectTransform.position =
				Camera.main.WorldToScreenPoint( WorldAnchor.position + new Vector3( 0f, 1.5f ) );
		}
	}


}
