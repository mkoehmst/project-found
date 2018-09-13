namespace ProjectFound.Interaction
{


	using UnityEngine;

	//using ProjectFound.Environment.Handlers;
	using ProjectFound.Environment;
	//using ProjectFound.Interaction;
	//using Autelia.Serialization;

	public abstract class Interactee : MonoBehaviour
	{
		// TODO Decide if non-interactable inanimate objects might also have health

		// Bring health all the way down to the Interactee level because inanimate objects
		// can have health too, the amount of damage before they are destroyed

		public string m_ingameName = "Unknown";


		[SerializeField] protected float m_maxHealthPoints = 100f;
		[SerializeField] protected float m_curHealthPoints = 1f;
		[SerializeField] protected string m_activateText = "Pickup";
		[SerializeField] protected string m_deactivateText = "Drop";
		[SerializeField] protected bool m_isActivated = false;
		//[SerializeField] protected CommentSpec m_commentSpec;

		[Header("Handler Chains")]
		[SerializeField] protected HandlerChain _oneShotChain;
		public HandlerChain OneShotChain 
		{ 
			get { return _oneShotChain; } 
			set { _oneShotChain = value; }
		}

		[SerializeField] protected HandlerChain _oneShotReleaseChain;
		public HandlerChain OneShotReleaseChain 
		{  
			get { return _oneShotReleaseChain; }
			set { _oneShotReleaseChain = value; }
		}

		[SerializeField] protected HandlerChain _windowChain;
		public HandlerChain WindowChain 
		{ 
			get { return _windowChain; } 
			set { _windowChain = value; }
		}

		[SerializeField] protected HandlerChain _windowReleaseChain;
		public HandlerChain WindowReleaseChain 
		{ 
			get { return _windowReleaseChain; } 
			set { _windowReleaseChain = value; }
		}

		[SerializeField] protected HandlerChain _holdingChain;
		public HandlerChain HoldingChain 
		{ 
			get { return _holdingChain; } 
			set { _holdingChain = value; }
		}

		[SerializeField] protected HandlerChain _holdingReleaseChain;
		public HandlerChain HoldingReleaseChain 
		{ 
			get { return _holdingReleaseChain; } 
			set { _holdingReleaseChain = value; }
		}

		[SerializeField] protected HandlerChain _focusChain;
		public HandlerChain FocusChain 
		{ 
			get { return _focusChain; } 
			set { _focusChain = value; }
		}

		[SerializeField] protected HandlerChain _focusReleaseChain;
		public HandlerChain FocusReleaseChain 
		{ 
			get { return _focusReleaseChain; }
			set { _focusReleaseChain = value; }
		}

		[SerializeField] protected HandlerChain _usageChain;
		public HandlerChain UsageChain 
		{ 
			get { return _usageChain; } 
			set { _usageChain = value; }
		}

		//public State.UnityID UnityID { get; private set; }
		//public State.Saveable Saveable { get; private set; }

		public bool IsFocused { get; private set; }
		public LayerID LayerID { get; protected set; }

		public string ActivateText
		{
			get { return m_activateText; }
		}

		public string DeactivateText
		{
			get { return m_deactivateText; }
		}

		public string PromptText
		{
			get { return m_isActivated ? m_deactivateText : m_activateText; }
		}

		public string IngameName
		{
			get { return m_ingameName; }
		}

		public bool IsActivated
		{
			get { return m_isActivated; }
			set { m_isActivated = value; }
		}

		//public CommentSpec CommentSpec
		//{
		//	get { return m_commentSpec; }
		//}

		public float HealthAsPercentage
		{
			get { return m_curHealthPoints / m_maxHealthPoints; }
		}

		public float Health
		{
			get { return m_curHealthPoints; }
		}

		public Interactee ApproachTarget { get; set; }

		[System.NonSerialized] private bool _areLayersSet = false;

		protected void Awake( )
		{
			LayerID = LayerID.Undefined;
		}

		protected void Start( )
		{
			if ( LayerID != LayerID.Undefined && !_areLayersSet )
			{
				SetLayers( transform );
				_areLayersSet = true;
			}

			if (Autelia.Serialization.Serializer.IsLoading) return;

			Debug.Assert( m_curHealthPoints > 0f, "Must start with positive health" );

			m_curHealthPoints = m_maxHealthPoints;
			IsFocused = false;
		}

		private void SetLayers( Transform parent )
		{
			Collider[] colliders = parent.GetComponentsInChildren<Collider>( );
			for ( int i = 0; i < colliders.Length; ++i )
			{
				colliders[i].gameObject.layer = (int)LayerID;
			}
		}

		public float DistanceTo( Vector3 point )
		{
			return (transform.position - point).magnitude;
		}

		public float DistanceTo( ref Vector3 point )
		{
			return (transform.position - point).magnitude;
		}
	}


}
