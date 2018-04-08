using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

using ProjectFound.Environment.Handlers;

namespace ProjectFound.Environment.Props {


	public class Item : Prop
	{

		//[SerializeField] ItemSpec m_itemSpec;

		new void Start( )
		{
			base.Start( );

			//m_handler.Initialize( this );

			//GenerateIcon( );
		}

		/*
		public void GenerateIcon( )
		{
			int counter = 0;

			while ( m_icon == null && counter < 75 )
			{
				m_icon = AssetPreview.GetAssetPreview( m_iconPrefab );
				counter++;
				System.Threading.Thread.Sleep( 15 );
			}

			//m_icon = AssetPreview.GetMiniThumbnail( m_iconPrefab );

			//m_icon = AssetPreview.GetAssetPreview( gameObject );
		}*/
	}


}
