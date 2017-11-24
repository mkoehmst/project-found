using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Assertions;

using ProjectFound.Environment.Items;

namespace ProjectFound.CameraUI {


	public class PropCollectionUI : MonoBehaviour
	{
		[SerializeField] GameObject m_propButtonPrefab;

		public Dictionary<Prop,Button> Buttons { get; private set; }
		public WidgetItemGridUI PropGrid { get; private set; }

		void Awake( )
		{
			PropGrid = GetComponentInChildren<WidgetItemGridUI>( );
			Assert.IsNotNull( PropGrid );

			Buttons = new Dictionary<Prop,Button>( );
		}

		void Start( )
		{
			Assert.IsNotNull( m_propButtonPrefab );
		}

		public Button AddProp( Prop prop )
		{
			GameObject slot = PropGrid.FirstEmptySlot;

			if ( slot == null )
				return null;

			GameObject obj = Instantiate( m_propButtonPrefab, slot.transform );
			Button button = obj.GetComponent<Button>( );
			Image img = obj.GetComponent<Image>( );
			img.sprite = prop.Icon;

			Buttons.Add( prop, button );

			return button;
		}

		public void RemoveProp( Prop prop )
		{
			Destroy( Buttons[prop].gameObject );

			Buttons.Remove( prop );
		}

		public void ClearCollection( )
		{
			foreach ( Button button in Buttons.Values )
			{
				Destroy( button.gameObject );
			}

			Buttons.Clear( );
		}
	}


}
