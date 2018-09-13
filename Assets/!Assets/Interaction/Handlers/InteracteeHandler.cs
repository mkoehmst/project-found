namespace ProjectFound.Interaction
{


	using System.Collections.Generic;

	using UnityEngine;

	public abstract class InteracteeHandler : BaseHandler
	{
		public delegate IEnumerator<float> HandlerDelegate( Interactee ie, Interactor ir );

		public HandlerDelegate DelegateWindow { get; protected set; }
		public HandlerDelegate DelegateWindowRelease { get; protected set; }
		public HandlerDelegate DelegateWindowAbort { get; protected set; }
		public HandlerDelegate DelegateHandlerRelease { get; protected set; }
		public HandlerDelegate DelegateHandlerAbort { get; protected set; }

		[SerializeField] bool _isSingletonHandler = false;
		public bool IsSingletonHandler { get { return _isSingletonHandler; } }

		[SerializeField] MEC.SingletonBehavior _singletonBehavior = MEC.SingletonBehavior.Overwrite;
		public MEC.SingletonBehavior SingletonBehavior { get { return _singletonBehavior; } }

		public virtual IEnumerator<float> Window( Interactee ie, Interactor ir )
		{
			Debug.LogError( "InteracteeHandler.PreHandle() should not get called!" );

			yield break;
		}

		public virtual IEnumerator<float> WindowRelease( Interactee ie, Interactor ir )
		{
			Debug.LogError( "Interactee.PreHandlerCleanup should not get called!" );

			yield break;
		}

		public virtual IEnumerator<float> WindowAbort( Interactee ie, Interactor ir )
		{
			Debug.LogError( "Interactee.WindowAbort should not get called!" );

			yield break;
		}

		// Mark Handle abstract as it's the only required execution mode for a Handler
		public abstract IEnumerator<float> Handler( Interactee ie, Interactor ir );

		public virtual IEnumerator<float> HandlerRelease( Interactee ie, Interactor ir )
		{
			Debug.LogError( "InteracteeHandler.HandleRelease() should not get called!" );

			yield break;
		}

		public virtual IEnumerator<float> HandleAbort( Interactee ie, Interactor ir )
		{
			Debug.LogError( "InteracteeHandler.HandleAbort() should not get called!" );

			yield break;
		}
	}


}
