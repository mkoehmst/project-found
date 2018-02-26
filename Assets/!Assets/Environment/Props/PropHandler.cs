using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using ProjectFound.Core;
using ProjectFound.Master;
using ProjectFound.Environment.Props;

namespace ProjectFound.Environment.Handlers
{

	[CreateAssetMenu(menuName=("Project Found/Prop Handler"))]
	public class PropHandler : InteracteeHandler
	{
		private const string m_activateTriggerString = "Prop_Activate";
		private const string m_deactivateTriggerString = "Prop_Deactivate";
		private int m_activateTrigger;
		private int m_deactivateTrigger;

		//protected Prop m_component;
		//protected GameObject m_gameObject;

		//protected Animator m_animator;

		void OnEnable( )
		{
			if ( m_activateTrigger == 0 )
			{
				m_activateTrigger = Animator.StringToHash( m_activateTriggerString );
			}

			if ( m_deactivateTrigger == 0 )
			{
				m_deactivateTrigger = Animator.StringToHash( m_deactivateTriggerString );
			}
		}

		/*public void Initialize( Prop prop )
		{
			//m_component = prop;
			//m_gameObject = prop.gameObject;
			//m_animator = m_gameObject.GetComponent<Animator>( );

			if ( m_activateTrigger == 0 )
			{
				m_activateTrigger = Animator.StringToHash( m_activateTriggerString );
			}

			if ( m_deactivateTrigger == 0 )
			{
				m_deactivateTrigger = Animator.StringToHash( m_deactivateTriggerString );
			}
		}*/

		public override IEnumerator Use( Interactee interactee )
		{
			if ( interactee.IsReceptive == false )
			{
				yield break;
			}

			Animator animator = interactee.GetComponent<Animator>( );

			if ( interactee.IsActivated == false )
			{
				animator?.SetTrigger( m_activateTrigger );
				//m_animator?.SetBool( "IsActivated", true );
				interactee.IsActivated = true;
			}
			else
			{
				animator?.SetTrigger( m_deactivateTrigger );
				//m_animator?.SetBool( "IsActivated", false );
				interactee.IsActivated = false;
			}
		}

		public void DragAndDrop( Prop prop, ref RaycastHit hit )
		{
			if ( prop.IsReceptive == false || prop.IsDraggable == false )
			{
				return ;
			}

			GameObject gameObj = prop.gameObject;
			var raycaster = RaycastMaster.CurrentInteracteeRaycaster;

			RemoveFocus( prop );
			raycaster.IsEnabled = false;
			raycaster = RaycastMaster.CurrentInteracteeRaycaster =
				RaycastMaster.PropPlacementRaycaster;
			raycaster.IsEnabled = true;
			raycaster.AddBlacklistee( prop );
			PlayerMaster.StartPropPlacement( prop, gameObj, ref hit );
		}

		protected void RemoveFocusDirectly( Prop prop )
		{
			// Nullify Previous Raycast Hit Check so RemoveFocus isn't called twice
			RaycastMaster.CurrentInteracteeRaycaster.PreviousPriorityHitCheck.Remove( prop );
			RemoveFocus( prop );
		}

		protected void RemoveFocus( Prop prop )
		{
			if ( prop.IsFocused == true )
			{
				GameObject gameObj = prop.gameObject;
				prop.IsFocused = false;

				UIMaster.RemovePrompt( prop );
				ShaderMaster.ToggleSelectionOutline( gameObj );
			}
		}
	}


}
