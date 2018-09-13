namespace ProjectFound.CameraUI
{ 


	using System.Collections.Generic;
	
	using UnityEngine;
	using UnityEngine.Assertions;
	using UnityEngine.UI;

	using ProjectFound.Environment;
	using ProjectFound.Environment.Props;

	public class DetectionUI : WindowUI 
	{
		[SerializeField] GameObject _detectionEntryPrefab;

		public WidgetItemGridUI ItemGrid { get; private set; }
		public Dictionary<Prop, Button> Buttons { get; private set; }

		protected new void Awake( )
		{
			base.Awake( );

			Buttons = new Dictionary<Prop,Button>( );

			Assert.IsNotNull( _detectionEntryPrefab );

			ItemGrid = GetComponentInChildren<WidgetItemGridUI>( );
			Assert.IsNotNull( ItemGrid );
		}

		protected new void Start( )
		{
			base.Start( );

			Hide( );
		}

		public void AddDetections( Collider[] detections, int detectionCount )
		{
			for ( int i = 0; i < detectionCount; ++i )
			{
				Collider detection = detections[i];
				
				Prop prop = detection.GetComponentInParent<Prop>( );
				Assert.IsNotNull( prop );

				if ( prop != null && !Buttons.ContainsKey( prop ) )
				{ 
					AddDetection( prop );
				}
			}
		}

		public new void Show( )
		{
			if ( Buttons.Count > 0 )
			{
				base.Show( );
			}
		}

		public new void Hide( )
		{
			base.Hide( );

			var values = Buttons.Values;
			foreach ( Button button in values )
			{ 
				Misc.SmartDestroy.Destroy( button.gameObject );
			}

			Buttons.Clear( );
		}

		private Button AddDetection( Prop prop )
		{
			GameObject slot = ItemGrid.FirstEmptySlot;

			if ( slot == null )
				return null;

			GameObject obj = Instantiate( _detectionEntryPrefab, slot.transform );

			Button button = obj.GetComponent<Button>( );
			Assert.IsNotNull( button );

			Image img = obj.GetComponent<Image>( );
			Assert.IsNotNull( img );

			img.sprite = prop.Icon;

			Buttons.Add( prop, button );

			return button;
		}
	}


}
