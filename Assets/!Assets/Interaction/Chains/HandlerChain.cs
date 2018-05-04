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

		protected void OnEnable( )
		{
			m_handlers.Clear( );
		}

		public IEnumerator<float> ExecuteChain( Interactee ie, Interactor ir )
		{
			BeginChain( ir );

			for ( int i = 0; i < m_handlers.Count; ++i )
			{
				HandlerDesc handlerDesc = m_handlers[i];
				InteracteeHandler handler = handlerDesc.m_handler;

				if ( handlerDesc.m_executionMode == HandlerExecutionMode.Async )
				{
					MEC.Timing.RunCoroutine( handler.Handle( ie, ir ) );
					//ir.StartCoroutine( handler.Handle( ie, ir ) );
				}
				else
				{
					yield return MEC.Timing.WaitUntilDone( handler.Handle( ie, ir ) );
				}
			}

			while ( EndChain( ir ) == false )
			{
				yield return MEC.Timing.WaitForOneFrame;
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

		private bool EndChain( Interactor ir )
		{
			for ( int i = 0; i < m_handlers.Count; ++i )
			{
				HandlerDesc handlerDesc = m_handlers[i];

				if ( handlerDesc.m_executionMode == HandlerExecutionMode.Async )
				{
					InteracteeHandler handler = handlerDesc.m_handler;

					if ( ir.HandlerExecutionDictionary.ContainsKey( handler )
						&& ir.HandlerExecutionDictionary[handler] == true )
					{
						return false;
					}
				}
			}

			ir.IsBusy = false;
			return true;
		}
	}

}