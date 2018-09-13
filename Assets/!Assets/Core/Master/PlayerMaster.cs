namespace ProjectFound.Core.Master
{


	using UnityEngine;
	using UnityEngine.Assertions;

	using ProjectFound.Interaction;
	using ProjectFound.Environment.Characters;
	using ProjectFound.Environment.Props;

	public class PlayerMaster
	{
		//private ConductBar m_conductBar;
		//private Inventory m_inventory;

		public Placeable Placeable { get; private set; }

		//public MovementFeedback MovementFeedback { get; private set; }
		public Protagonist Protagonist { get; private set; }
		public ProtagonistController ProtagonistController { get; private set; }

		public bool OccludedFromCamera { get; set; }

		//public SkillBook SkillBook { get; private set; }
		//public Skill[] ConductBarSkills
		//{
		//	get { return m_conductBar.m_state.m_skills; }
		//}

		public PlayerMaster( )
		{
			Protagonist = UnityEngine.Component.FindObjectOfType<Protagonist>( );
			ProtagonistController = Protagonist.GetComponent<ProtagonistController>( );

			//MovementFeedback = Player.GetComponentInChildren<MovementFeedback>( );
			OccludedFromCamera = false;
			//SkillBook = Player.GetComponent<SkillBook>( );
			//m_conductBar = Player.GetComponent<ConductBar>( );
			//m_inventory = Player.GetComponent<Inventory>( );
		}

		public void Loop( )
		{

		}

		/*public void AddInventoryItem( Item item )
		{
			m_inventory.AddItem( item );
		}*/

		public void SetPlaceable( Prop prop )
		{
			if ( prop != null )
			{
				Placeable = prop.GetComponent<Placeable>( );
				Assert.IsNotNull( Placeable );
			}
			else
			{ 
				Placeable = null;
			}
		}

		public void PreparePropPlacement( )
		{
			Placeable.PreparePlacement( );
		}

		public void PropPlacement( ref Vector3 hitPoint, ref Vector3 hitNormal )
		{
			Placeable.Place( ref hitPoint, ref hitNormal );
		}

		public void EndPropPlacement( )
		{
			Placeable.ValidatePlacement( );
			Placeable = null;
			//PlacementProp = null;
		}

		//public void DropItem( Item item )
		//{
		//	item.transform.position = Player.transform.position + Player.transform.forward * 0.75f;
		//}

	

		/*public void AbortInteractionChains( )
		{
			Interactee ie = Protagonist.TargetInteractee;

			if ( ie == null )
			{
				return;
			}

			HandlerChain oneShotChain = ie.OneShotChain;
			if ( oneShotChain != null )
			{
				Protagonist.AbortOneShotChain( ie, oneShotChain );
			}

			HandlerChain holdingWindowChain = ie.WindowChain;
			if ( holdingWindowChain != null )
			{
				Protagonist.AbortWindowChain( ie, holdingWindowChain );
			}

			HandlerChain holdingChain = ie.HoldingChain;
			if ( holdingChain != null )
			{
				Protagonist.AbortHoldingChain( ie, holdingChain );
			}

			HandlerChain focusChain = ie.FocusChain;
			if ( focusChain != null )
			{
				Protagonist.AbortFocusChain( ie, focusChain );
			}

			HandlerChain usageChain = ie.UsageChain;
			if ( usageChain != null )
			{
				Protagonist.AbortUsageChain( ie, usageChain );
			}
		}*/

		public bool IsMoving( )
		{
			return Misc.Floater.GreaterThan( Protagonist.MovementSpeed, 0f );
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


		public void CombatMovementFeedback( Vector3 loc, bool isGood )
		{
			//MovementFeedback.IsFeedbackGood = isGood;
			//MovementFeedback.DrawCenter( loc );
		}
	}


}
