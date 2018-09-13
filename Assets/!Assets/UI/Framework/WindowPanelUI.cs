namespace ProjectFound.CameraUI 
{


	using UnityEngine;
	using Autelia.Serialization;

	public class WindowPanelUI : MonoBehaviour
	{
		[SerializeField] Vector2 _panelMargins;
		[SerializeField] bool _fixedWidth;
		[SerializeField] bool _fixedHeight;

		private GameObject _parentObject;
		private RectTransform _parentRect;
		private RectTransform _rect;

		void Start( )
		{
			if (Serializer.IsLoading) return;

			_parentObject = this.transform.parent.gameObject;
			_parentRect = _parentObject.GetComponent<RectTransform>( );
			_rect = this.GetComponent<RectTransform>( );

			float sizeX;
			if ( _fixedWidth )
			{
				sizeX = _rect.sizeDelta.x - (2 * _panelMargins.x);
			}
			else
			{ 
				sizeX = _parentRect.sizeDelta.x 
					- _rect.anchoredPosition.x 
					- (2 * _panelMargins.x);
			}
			
			float sizeY;
			if ( _fixedHeight )
			{
				sizeY = _rect.sizeDelta.y - (2 * _panelMargins.y);
			}
			else
			{ 
				sizeY = _parentRect.sizeDelta.y 
					- _rect.anchoredPosition.y 
					- (2 * _panelMargins.y);
			}

			_rect.localScale = new Vector3( 1f, 1f, 1f );
			_rect.sizeDelta = new Vector2( sizeX, sizeY );

			_rect.anchoredPosition = new Vector2( 
				_rect.anchoredPosition.x + _panelMargins.x, 
				_rect.anchoredPosition.y - _panelMargins.y );
		}
	}


}
