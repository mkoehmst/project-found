namespace ProjectFound.Interaction
{


	using System;
	using System.Collections.Generic;

	using UnityEngine;
	using UnityEngine.Assertions;

	[CreateAssetMenu(menuName=("Found/Handler Chain"))]
	public class HandlerChain : ScriptableObject
	{
		protected enum HandlerExecutionMode
		{
			Undefined,
			Async,
			Blocking
		}

		public enum ChainType
		{
			Undefined,
			Window,
			WindowRelease,
			WindowAbort,
			Handler,
			HandlerRelease,
			HandlerAbort
		}

		[Serializable]
		protected struct HandlerDesc
		{
			public InteracteeHandler m_handler;
			public HandlerExecutionMode m_executionMode;

			public HandlerDesc( InteracteeHandler handler, HandlerExecutionMode executionMode )
			{
				m_handler = handler;
				m_executionMode = executionMode;
			}
		}

		[SerializeField] List<HandlerDesc> _handlers = new List<HandlerDesc>( );

		protected void OnEnable()
		{
			foreach ( var handlerDesc in _handlers )
			{
				Assert.IsNotNull( handlerDesc.m_handler );
				Assert.IsFalse( handlerDesc.m_executionMode == HandlerExecutionMode.Undefined );
			}
		}

		public IEnumerator<float> RunWindowChain( Interactee ie, Interactor ir )
		{
			MEC.CoroutineHandle chainHandle = new MEC.CoroutineHandle( );
			yield return MEC.Timing.GetMyHandle( x => chainHandle = x );

			int count = _handlers.Count;
			for ( int i = 0; i < count; ++i )
			{
				HandlerDesc handlerDesc = _handlers[i];
				InteracteeHandler handler = handlerDesc.m_handler;

				if ( handler.DelegateWindow != null )
				{
					//Debug.Log( "HandlerChain.RunWindowChain() " + handler
						//+ handlerDesc.m_executionMode );
					MEC.CoroutineHandle windowHandle;
					yield return MEC.Timing.RunThisCoroutine( 
						handler.DelegateWindow( ie, ir ),
						out windowHandle, ref chainHandle, MEC.NestingType.ChildBlockAndLinkTo );
				}
			}

			//Debug.Log( "HandlerChain RunWindowChain() exited gracefully" );

			yield break;
		}

		public IEnumerator<float> RunWindowReleaseChain( Interactee ie, Interactor ir )
		{
			MEC.CoroutineHandle chainHandle = new MEC.CoroutineHandle( );
			yield return MEC.Timing.GetMyHandle( x => chainHandle = x );

			int count = _handlers.Count;
			for ( int i = 0; i < count; ++i )
			{
				HandlerDesc handlerDesc = _handlers[i];
				InteracteeHandler handler = handlerDesc.m_handler;

				if ( handler.DelegateWindow != null )
				{
					//Debug.Log( "HandlerChain.RunWindowReleaseChain() " + handler
						//+ handlerDesc.m_executionMode );
					MEC.CoroutineHandle windowReleaseHandle;
					yield return MEC.Timing.RunThisCoroutine( 
						handler.DelegateWindowRelease( ie, ir ),
						out windowReleaseHandle, ref chainHandle, MEC.NestingType.ChildBlockAndLinkTo );
				}
			}

			//Debug.Log( "HandlerChain RunWindowReleaseChain() exited gracefully" );

			yield break;
		}

		public IEnumerator<float> RunWindowAbortChain( Interactee ie, Interactor ir )
		{
			MEC.CoroutineHandle chainHandle = new MEC.CoroutineHandle( );
			yield return MEC.Timing.GetMyHandle( x => chainHandle = x );

			int count = _handlers.Count;
			for ( int i = 0; i < count; ++i )
			{
				HandlerDesc handlerDesc = _handlers[i];
				InteracteeHandler handler = handlerDesc.m_handler;

				if ( handler.DelegateWindowAbort != null )
				{
					//Debug.Log( "HandlerChain.RunWindowAbortChain() " + handler
					//	+ handlerDesc.m_executionMode );

					MEC.CoroutineHandle windowAbortHandle;
					yield return MEC.Timing.RunThisCoroutine( 
						handler.DelegateWindowAbort( ie, ir ),
						out windowAbortHandle, ref chainHandle, MEC.NestingType.ChildBlockAndLinkTo );
				}
			}

			yield break;
		}

		public IEnumerator<float> RunHandlerChain( Interactee ie, Interactor ir )
		{
			Assert.IsNotNull( ir );

			BeginChain( ir );

			// GetMyHandle is special in that does not cause a one frame delay even though it's in
			// a yield return statement.
			MEC.CoroutineHandle chainHandle = new MEC.CoroutineHandle( );
			yield return MEC.Timing.GetMyHandle( x => chainHandle = x );

			int count = _handlers.Count;
			for ( int i = 0; i < count; ++i )
			{
				HandlerDesc handlerDesc = _handlers[i];
				InteracteeHandler handler = handlerDesc.m_handler;

				Assert.IsNotNull( handler );
				Assert.IsTrue( handlerDesc.m_executionMode != HandlerExecutionMode.Undefined );

				//Debug.Log( "HandlerChain.RunHandlerChain() " + handler + handlerDesc.m_executionMode );

				MEC.NestingType nesting = 
					(handlerDesc.m_executionMode == HandlerExecutionMode.Blocking) 
						? MEC.NestingType.ChildBlockAndLinkTo 
						: MEC.NestingType.ChildLinkTo;

				MEC.CoroutineHandle handlerHandle;
				if ( handler.IsSingletonHandler )
				{
					yield return MEC.Timing.RunThisCoroutineSingleton( handler.Handler( ie, ir ), 
						out handlerHandle, chainHandle.CoroutineNode, nesting, 
						handler.GetInstanceID( ), handler.SingletonBehavior );
				}
				else
				{
					yield return MEC.Timing.RunThisCoroutine( handler.Handler( ie, ir ), 
						out handlerHandle, ref chainHandle, nesting );
				}
			}

			// Wait for async children to finish as well
			MEC.CoroutineList.Node child = chainHandle.CoroutineNode.FirstLinkee;
			while ( child != null )
			{
				if ( child.IsValid )
				{
					yield return MEC.Timing.WaitForOneFrame;
					child = chainHandle.CoroutineNode.FirstLinkee;
				}
				else
					child = child.NextLinkeeSibling;
			}

			EndChain( ir );

			//Debug.Log( "HandlerChain.RunHandlerChain() exited gracefully" );

			yield break;
		}

		public IEnumerator<float> RunHandlerReleaseChain( Interactee ie, Interactor ir )
		{
			MEC.CoroutineHandle chainHandle = new MEC.CoroutineHandle( );
			yield return MEC.Timing.GetMyHandle( x => chainHandle = x );

			int count = _handlers.Count;
			for ( int i = 0; i < _handlers.Count; ++i )
			{
				HandlerDesc handlerDesc = _handlers[i];
				InteracteeHandler handler = handlerDesc.m_handler;

				if ( handler.DelegateHandlerRelease != null )
				{
					//Debug.Log( "HandlerChain.RunHandlerReleaseChain() " + handler
						//+ handlerDesc.m_executionMode );
					MEC.CoroutineHandle handlerReleaseHandle;
					yield return MEC.Timing.RunThisCoroutine( 
						handler.DelegateHandlerRelease( ie, ir ),
						out handlerReleaseHandle, ref chainHandle, MEC.NestingType.ChildBlockAndLinkTo );
				}
			}

			//Debug.Log( "HandlerChain RunHandlerReleaseChain() exited gracefully" );

			yield break;
		}

		public IEnumerator<float> RunHandlerAbortChain( Interactee ie, Interactor ir )
		{
			MEC.CoroutineHandle chainHandle = new MEC.CoroutineHandle( );
			yield return MEC.Timing.GetMyHandle( x => chainHandle = x );

			int count = _handlers.Count;
			for ( int i = 0; i < _handlers.Count; ++i )
			{
				HandlerDesc handlerDesc = _handlers[i];
				InteracteeHandler handler = handlerDesc.m_handler;

				if ( handler.DelegateHandlerAbort != null )
				{
					//Debug.Log( "HandlerChain.RunAbortHandleChain() " + handler
						//+ handlerDesc.m_executionMode );
					MEC.CoroutineHandle handlerAbortHandle;
					yield return MEC.Timing.RunThisCoroutine( 
						handler.DelegateHandlerAbort( ie, ir ),
						out handlerAbortHandle, ref chainHandle, MEC.NestingType.ChildBlockAndLinkTo );
				}
			}

			yield break;
		}

		protected void AddHandler( InteracteeHandler handler, HandlerExecutionMode executionMode )
		{
			if ( handler == null )
			{
				Debug.LogError( "HandlerChain.AddHandler: handler is null!" );
			}
			else
			{
				_handlers.Add( new HandlerDesc( handler, executionMode ) );
			}
		}

		private void BeginChain( Interactor ir )
		{
			ir.IsBusy = true;
		}

		private void EndChain( Interactor ir )
		{
			ir.IsBusy = false;
		}
	}

}