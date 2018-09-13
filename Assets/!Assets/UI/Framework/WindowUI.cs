namespace ProjectFound.CameraUI
{ 

	using System.Collections.Generic;

	using UnityEngine;
	using UnityEngine.UI;

	[RequireComponent(typeof(CanvasGroup))]
	[RequireComponent(typeof(Image))]
	public abstract class WindowUI : MonoBehaviour 
	{
		[SerializeField] Sprite _backgroundSprite;

		public bool IsHidden { get; protected set; } = true;

		public CanvasGroup CanvasGroup { get; private set; }
		public Image BackgroundImage { get; private set; }

		public List<WindowPanelUI> Panels { get; private set; } = new List<WindowPanelUI>( );

		protected void Awake( ) 
		{
			CanvasGroup = GetComponent<CanvasGroup>( );
			BackgroundImage = GetComponent<Image>( );

			WindowPanelUI[] panels = GetComponentsInChildren<WindowPanelUI>( );
			Panels.AddRange( panels );
		}

		protected void Start( )
		{
			BackgroundImage.sprite = _backgroundSprite;
			BackgroundImage.raycastTarget = false;
		}
	
		public void Show( )
		{
			IsHidden = false;

			CanvasGroup.alpha = 1f;
			CanvasGroup.interactable = true;
			CanvasGroup.blocksRaycasts = true;
		}

		public void Hide( )
		{
			IsHidden = true;

			CanvasGroup.alpha = 0f;
			CanvasGroup.interactable = false;
			CanvasGroup.blocksRaycasts = false;
		}

		public void Toggle( )
		{
			if ( IsHidden )
			{
				Show( );
			}
			else
			{
				Hide( );
			}
		}

		public void AddPanel( WindowPanelUI panel )
		{
			Panels.Add( panel );
		}

		public void RemovePanel( WindowPanelUI panel )
		{
			Panels.Remove( panel );
		}
	}


}
