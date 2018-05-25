using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using ProjectFound.Environment;
using ProjectFound.Environment.Characters;
using ProjectFound.Environment.Props;

namespace ProjectFound.Master
{

	using ProjectFound.Environment.Handlers;

	public class PlayerMaster
	{
		private ConductBar m_conductBar;
		private Inventory m_inventory;

		private Placement Placement { get; set; }
		private Vector3 m_placementStartingPosition;
		private Quaternion m_placementStartingRotation;

		public Prop PropBeingPlaced { get; private set; }
		public Player Player { get; private set; }
		//public CharacterMovement CharacterMovement { get; private set; }
		public MovementFeedback MovementFeedback { get; private set; }
		public MK_RPGCharacterControllerFREE CharacterController { get; private set; }

		public bool OccludedFromCamera { get; set; }

		public SkillBook SkillBook { get; private set; }
		public Skill[] ConductBarSkills
		{
			get { return m_conductBar.m_state.m_skills; }
		}

		public PlayerMaster( )
		{
			Player = UnityEngine.Component.FindObjectOfType<Player>( );

			CharacterController = Player.GetComponent<MK_RPGCharacterControllerFREE>( );
			MovementFeedback = Player.GetComponentInChildren<MovementFeedback>( );
			OccludedFromCamera = false;
			SkillBook = Player.GetComponent<SkillBook>( );
			m_conductBar = Player.GetComponent<ConductBar>( );
			m_inventory = Player.GetComponent<Inventory>( );
		}

		public void Loop( )
		{

		}

		public void AddInventoryItem( Item item )
		{
			m_inventory.AddItem( item );
		}

		public void SetPropBeingPlaced( Prop prop )
		{
			PropBeingPlaced = prop;

			if ( prop != null )
			{ 
				Transform xform = prop.transform;
				m_placementStartingPosition = xform.position;
				m_placementStartingRotation = xform.rotation;
			}
		}

		public void PreparePropPlacement( )
		{
			Placement = PropBeingPlaced.gameObject.AddComponent<Placement>( );
			Placement.SetStartingTransform( ref m_placementStartingPosition,
				ref m_placementStartingRotation );
			//Placement.RecordCursorOffset( ref hit );
		}

		public void PropPlacement( ref Vector3 hitPoint, ref Vector3 hitNormal )
		{
			Placement.Place( ref hitPoint, ref hitNormal );
		}

		public void EndPropPlacement( )
		{
			Placement.ValidatePlacement( );
			PropBeingPlaced = null;
		}

		public void DropItem( Item item )
		{
			item.transform.position = Player.transform.position + Player.transform.forward * 0.75f;
		}

		public void RunOneShotChain( Interactee ie )
		{
			Player.RunOneShotChain( ie );
		}

		public void RunOneShotReleaseChain( Interactee ie )
		{
			HandlerChain chain = ie.OneShotReleaseChain;

			if ( chain != null )
			{
				Player.RunOneShotReleaseChain( ie, chain );
			}
		}

		public void RunWindowChain( Interactee ie )
		{
			HandlerChain chain = ie.WindowChain;

			if ( chain != null )
			{
				Player.RunWindowChain( ie, chain );
			}
		}

		public void RunWindowReleaseChain( Interactee ie )
		{
			HandlerChain chain = ie.WindowReleaseChain;

			if ( chain != null )
			{
				Player.RunWindowReleaseChain( ie, chain );
			}
		}

		public void RunHoldingChain( Interactee ie )
		{
			HandlerChain chain = ie.HoldingChain;

			if ( chain != null )
			{
				Player.RunHoldingChain( ie, chain );
			}
		}

		public void RunHoldingReleaseChain( Interactee ie )
		{
			HandlerChain chain = ie.HoldingReleaseChain;

			if ( chain != null )
			{
				Player.RunHoldingReleaseChain( ie, chain );
			}
		}

		public void RunFocusChain( Interactee ie )
		{
			HandlerChain chain = ie.FocusChain;

			if ( chain != null )
			{
				Player.RunFocusChain( ie, chain );
			}
		}

		public void RunFocusReleaseChain( Interactee ie )
		{
			HandlerChain chain = ie.FocusReleaseChain;

			if ( chain != null )
			{
				Player.RunFocusReleaseChain( ie, chain );
			}
		}

		public void RunUsageChain( Interactee ie )
		{
			HandlerChain chain = ie.UsageChain;

			if ( chain != null )
			{
				Player.RunUsageChain( ie, chain );
			}
		}

		public void KillOneShotChain( )
		{
			Player.KillOneShotChain( );
		}

		public void KillOneShotReleaseChain( )
		{
			Player.KillOneShotReleaseChain( );
		}

		public void KillWindowChain( )
		{
			Player.KillWindowChain( );
		}

		public void KillWindowReleaseChain( )
		{
			Player.KillWindowReleaseChain( );
		}

		public void KillHoldingChain( )
		{
			Player.KillHoldingChain( );
		}

		public void KillHoldingReleaseChain( )
		{
			Player.KillHoldingReleaseChain( );
		}

		public void KillFocusChain( )
		{
			Player.KillFocusChain( );
		}

		public void KillFocusReleaseChain( )
		{
			Player.KillFocusReleaseChain( );
		}

		public void KillUsageChain( )
		{
			Player.KillUsageChain( );
		}

		public void AbortInteractionChains( )
		{
			Interactee ie = Player.TargetInteractee;

			if ( ie == null )
			{
				return;
			}

			HandlerChain oneShotChain = ie.OneShotChain;
			if ( oneShotChain != null )
			{
				Player.AbortOneShotChain( ie, oneShotChain );
			}

			HandlerChain holdingWindowChain = ie.WindowChain;
			if ( holdingWindowChain != null )
			{
				Player.AbortWindowChain( ie, holdingWindowChain );
			}

			HandlerChain holdingChain = ie.HoldingChain;
			if ( holdingChain != null )
			{
				Player.AbortHoldingChain( ie, holdingChain );
			}

			HandlerChain focusChain = ie.FocusChain;
			if ( focusChain != null )
			{
				Player.AbortFocusChain( ie, focusChain );
			}

			HandlerChain usageChain = ie.UsageChain;
			if ( usageChain != null )
			{
				Player.AbortUsageChain( ie, usageChain );
			}
		}

		public void MoveTo( ref Vector3 destination )
		{
			CharacterController.SetMovementTarget( ref destination );
		}

		public void StopMoving( )
		{
			if ( IsMoving( ) )
			{
				CharacterController.ResetMovementTarget( );
			}
		}

		public bool IsMoving( )
		{
			return Misc.Floater.GreaterThan( CharacterController.MovementSpeed, 0f );
		}

		public bool CanMoveTo( Vector3 destination )
		{
			return true;
			//return CharacterMovement.CanMoveTo( destination );
		}

		/*public bool CanMoveToTarget( )
		{
			return CharacterMovement.CanMoveTo( Player.CombatTarget.transform.position );
		}*/

		public float NavMeshDistanceTo( )
		{
			return 5f;
			//return CharacterMovement.CalculatePathDistance( );
		}

		public float DistanceTo( Vector3 point )
		{
			return (Player.transform.position - point).magnitude;
		}

		public void CombatMovementFeedback( Vector3 loc, bool isGood )
		{
			MovementFeedback.IsFeedbackGood = isGood;
			MovementFeedback.DrawCenter( loc );
		}
	}

}
