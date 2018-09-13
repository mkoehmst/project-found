
namespace ProjectFound.Environment.Items
{


	using UnityEngine;
	using Autelia.Serialization;

	using ProjectFound.Interaction;
	using ProjectFound.Environment.Props;

	[RequireComponent(typeof(Pickupable), typeof(Usable))]
	public class Item : Prop
	{
		new protected void Awake( )
		{
			base.Awake( );

			LayerID = LayerID.Item;

			if ( Serializer.IsLoading ) return;
		}

		new protected void Start( )
		{
			base.Start( );

			if ( Serializer.IsLoading ) return;
			
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
