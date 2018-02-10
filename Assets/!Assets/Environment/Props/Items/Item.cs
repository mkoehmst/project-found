using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

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

		public void PickUp( )
		{
			(m_handler as ItemHandler).AddToInventory( this );
		}

		public override bool ValidateAction( ActionType actionType )
		{
			switch ( actionType )
			{
				case ActionType.PickUp:
				case ActionType.Use:
					m_currentActionType = actionType;
					return true;
				default:
					m_currentActionType = ActionType.None;
					return false;
			}
		}

		public override void Reaction( )
		{
			switch ( m_currentActionType )
			{
				case ActionType.PickUp:
					this.gameObject.SetActive( false );
					break;
				case ActionType.Use:
					this.gameObject.SetActive( true );
					break;
				default:
					base.Reaction( );
					break;
			}
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
