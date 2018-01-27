using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using ProjectFound.Core;
using ProjectFound.Master;

namespace ProjectFound.Environment.Props
{

	[CreateAssetMenu(menuName=("Project Found/Prop Handler"))]
	public class PropHandler : InteracteeHandler
	{
		private const string m_activateTriggerString = "Prop_Activate";
		private int m_activateTrigger;

		protected Prop m_component;
		protected GameObject m_gameObject;

		protected Animator m_animator;

		public void Initialize( Prop prop )
		{
			m_component = prop;
			m_gameObject = prop.gameObject;
			m_animator = m_gameObject.GetComponent<Animator>( );

			m_activateTrigger = Animator.StringToHash( m_activateTriggerString );
		}

		public override void Use( )
		{
			m_animator?.SetTrigger( m_activateTrigger );
			m_component.IsReceptive = false;
		}

		public void DragAndDrop( ref RaycastHit hit )
		{
			var raycaster = RaycastMaster.CurrentRaycaster;

			RemoveFocus( );
			raycaster.IsEnabled = false;
			raycaster = RaycastMaster.CurrentRaycaster =
				RaycastMaster.Raycasters[RaycastMaster.RaycastMode.PropPlacement];
			raycaster.IsEnabled = true;
			raycaster.AddBlacklistee( m_gameObject );
			PlayerMaster.StartPropPlacement( m_component, m_gameObject, ref hit );
		}

		protected void RemoveFocusDirectly( )
		{
			// Nullify Previous Raycast Hit Check so RemoveFocus isn't called twice
			RaycastMaster.CurrentRaycaster.PreviousPriorityHitCheck.Remove( m_gameObject );
			RemoveFocus( );
		}

		protected void RemoveFocus( )
		{
			if ( m_component.IsFocused == true )
			{
				m_component.IsFocused = false;

				UIMaster.RemovePrompt( m_component );
				ShaderMaster.ToggleSelectionOutline( m_gameObject );
			}
		}
	}


}
