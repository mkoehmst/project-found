namespace ProjectFound.CameraUI
{ 


	using UnityEngine;
	using UnityEngine.Assertions;
	using UnityEngine.UI;
	using Autelia.Serialization;

	public class WidgetCharacterDollUI : PanelWidgetUI 
	{
		[SerializeField] GameObject _dollPrefab;

		private RawImage _renderImage;
		private RectTransform _renderTransform;
		private DollCamera _dollRenderCamera;

		void Awake( )
		{
			if ( Serializer.IsLoading ) return;

			_dollRenderCamera = FindObjectOfType<DollCamera>( );
			Assert.IsNotNull( _dollRenderCamera );

			Assert.IsNotNull( _dollPrefab );
		}

		new void Start( ) 
		{
			base.Start( );

			if ( Serializer.IsLoading ) return;

			_renderImage = Instantiate( _dollPrefab, transform ).GetComponent<RawImage>( );
			_renderTransform = _renderImage.GetComponent<RectTransform>( );
			_renderTransform.sizeDelta = GetComponent<RectTransform>( ).sizeDelta;
		}
	}


}
