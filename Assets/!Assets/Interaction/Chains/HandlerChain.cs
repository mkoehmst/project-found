namespace ProjectFound.Environment.Handlers
{
	using System.Collections;
	using System.Collections.Generic;
	using UnityEngine;

	public abstract class HandlerChain : ScriptableObject
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

		protected List<HandlerDesc> m_handlers = new List<HandlerDesc>( );

		protected void OnEnable()
		{
			m_handlers.Clear( );
		}

		public IEnumerator<float> RunWindowChain( Interactee ie, Interactor ir )
		{
			int count = m_handlers.Count;
			for ( int i = 0; i < count; ++i )
			{
				HandlerDesc handlerDesc = m_handlers[i];
				InteracteeHandler handler = handlerDesc.m_handler;

				if ( handler.DelegateWindow != null )
				{
					//Debug.Log( "HandlerChain.RunWindowChain() " + handler 
						//+ handlerDesc.m_executionMode );
					MEC.Timing.RunCoroutine( handler.DelegateWindow( ie, ir ) );
				}
			}

			//Debug.Log( "HandlerChain RunWindowChain() exited gracefully" );

			yield break;
		}

		public IEnumerator<float> RunWindowReleaseChain( Interactee ie, Interactor ir )
		{
			int count = m_handlers.Count;
			for ( int i = 0; i < count; ++i )
			{
				HandlerDesc handlerDesc = m_handlers[i];
				InteracteeHandler handler = handlerDesc.m_handler;

				if ( handler.DelegateWindow != null )
				{
					//Debug.Log( "HandlerChain.RunWindowReleaseChain() " + handler 
						//+ handlerDesc.m_executionMode );
					MEC.Timing.RunCoroutine( handler.DelegateWindowRelease( ie, ir ) );
				}
			}

			//Debug.Log( "HandlerChain RunWindowReleaseChain() exited gracefully" );

			yield break;
		}

		public IEnumerator<float> RunWindowAbortChain( Interactee ie, Interactor ir )
		{
			int count = m_handlers.Count;
			for ( int i = 0; i < count; ++i )
			{
				HandlerDesc handlerDesc = m_handlers[i];
				InteracteeHandler handler = handlerDesc.m_handler;

				if ( handler.DelegateWindowAbort != null )
				{
					//Debug.Log( "HandlerChain.RunWindowAbortChain() " + handler 
					//	+ handlerDesc.m_executionMode );
					MEC.Timing.RunCoroutine( handler.DelegateWindowAbort( ie, ir ) );
				}
			}

			yield break;
		}

		public IEnumerator<float> RunHandlerChain( Interactee ie, Interactor ir )
		{
			BeginChain( ir );

			MEC.CoroutineHandle chainHandle = new MEC.CoroutineHandle( );

			// GetMyHandle is special in that does not cause a one frame delay even though it's in 
			// a yield return statement.
			yield return MEC.Timing.GetMyHandle( x => chainHandle = x );

			int count = m_handlers.Count;
			for ( int i = 0; i < count; ++i )
			{
				HandlerDesc handlerDesc = m_handlers[i];
				InteracteeHandler handler = handlerDesc.m_handler;

				//Debug.Log( "HandlerChain.RunHandlerChain() " + handler + handlerDesc.m_executionMode );

				MEC.CoroutineHandle handlerHandle
					= MEC.Timing.RunCoroutine( handler.Handler( ie, ir ) );

				MEC.Timing.LinkCoroutines( chainHandle, handlerHandle );

				if ( handlerDesc.m_executionMode == HandlerExecutionMode.Blocking )
				{
					while ( handlerHandle.IsRunning )
					{
						yield return MEC.Timing.WaitForOneFrame;
					}
				}
			}

			HashSet<MEC.CoroutineHandle> slaves = MEC.Timing.GetLinkedSlaves( ref chainHandle );
			if ( slaves != null )
			{
				// Since this Chain Coroutine runs before the Handler Coroutines,
				// it would only detect a Handler Coroutine finishing on the frame afterwards.
				// So instead run in the LateUpdate Segment, allowing it to detect
				// on the same frame that the Handler Coroutines finish.
				yield return MEC.Timing.WaitUntilDoneAndContinue(
					WaitForSlaves( slaves ), MEC.Segment.LateUpdate );
			}

			EndChain( ir );

			//Debug.Log( "HandlerChain.RunHandlerChain() exited gracefully" );

			yield break;
		}

		public IEnumerator<float> RunHandlerReleaseChain( Interactee ie, Interactor ir )
		{
			int count = m_handlers.Count;
			for ( int i = 0; i < m_handlers.Count; ++i )
			{
				HandlerDesc handlerDesc = m_handlers[i];
				InteracteeHandler handler = handlerDesc.m_handler;

				if ( handler.DelegateHandlerRelease != null )
				{
					//Debug.Log( "HandlerChain.RunHandlerReleaseChain() " + handler 
						//+ handlerDesc.m_executionMode );
					MEC.Timing.RunCoroutine( handler.DelegateHandlerRelease( ie, ir ) );
				}
			}

			//Debug.Log( "HandlerChain RunHandlerReleaseChain() exited gracefully" );

			yield break;
		}

		public IEnumerator<float> RunHandlerAbortChain( Interactee ie, Interactor ir )
		{
			int count = m_handlers.Count;
			for ( int i = 0; i < m_handlers.Count; ++i )
			{
				HandlerDesc handlerDesc = m_handlers[i];
				InteracteeHandler handler = handlerDesc.m_handler;

				if ( handler.DelegateHandlerAbort != null )
				{
					//Debug.Log( "HandlerChain.RunAbortHandleChain() " + handler 
						//+ handlerDesc.m_executionMode );
					MEC.Timing.RunCoroutine( handler.DelegateHandlerAbort( ie, ir ) );
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
				m_handlers.Add( new HandlerDesc( handler, executionMode ) );
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

		private IEnumerator<float> WaitForSlaves( HashSet<MEC.CoroutineHandle> slaves )
		{
			//MEC.CoroutineHandle rawHandle = new MEC.CoroutineHandle( );
			/*const string message1 = "Coroutine 1 is no longer running";
			const string message2 = "Coroutine 2 is no longer running";
			const string message3 = "Coroutine 3 is no longer running";
			const string message4 = "Coroutine 4 is no longer running";*/

			bool isStillExecuting = false;

			do
			{

				foreach ( var handler in slaves )
				{
					isStillExecuting = handler.IsRunning;

					if ( isStillExecuting )
					{
						yield return MEC.Timing.WaitForOneFrame;
						break;
					}
				}

			} while ( isStillExecuting );

			slaves.Clear( );

			yield break;
		}
	}

}