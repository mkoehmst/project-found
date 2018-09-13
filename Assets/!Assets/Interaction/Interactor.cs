namespace ProjectFound.Interaction
{


	using UnityEngine;

	using ProjectFound.Environment;

	public abstract class Interactor : Interactee
	{
		private MEC.CoroutineHandle _oneShotHandle;
		private MEC.CoroutineHandle _oneShotReleaseHandle;
		private MEC.CoroutineHandle _windowHandle;
		private MEC.CoroutineHandle _windowReleaseHandle;
		private MEC.CoroutineHandle _holdingHandle;
		private MEC.CoroutineHandle _holdingReleaseHandle;
		private MEC.CoroutineHandle _focusHandle;
		private MEC.CoroutineHandle _focusReleaseHandle;
		private MEC.CoroutineHandle _usageHandle;
		private MEC.CoroutineHandle _abortHandle;

		protected Interactee _targetInteractee;

		protected bool _isBusy = false;
		public bool IsBusy
		{
			get { return _isBusy; }
			set { _isBusy = value; }
		}

		public Interactee TargetInteractee { get { return _targetInteractee; } }
		public LayerID TargetLayerID { get { return _targetInteractee.LayerID; } }
		public bool HasTarget { get { return _targetInteractee != null; } }


		public void RunOneShotChain( Interactee ie )
		{
			HandlerChain chain = ie?.OneShotChain;

			if ( chain != null )
			{
				_targetInteractee = ie;

				MEC.Timing.RunThisCoroutine( chain.RunHandlerChain( ie, this ), 
					out _oneShotHandle );
			}
		}

		public void RunOneShotReleaseChain( Interactee ie )
		{
			HandlerChain chain = ie?.OneShotReleaseChain;

			if ( chain != null )
			{
				_targetInteractee = ie;

				MEC.Timing.RunThisCoroutine( chain.RunHandlerReleaseChain( ie, this ),
					out _oneShotReleaseHandle );
			}
		}

		public void RunWindowChain( Interactee ie )
		{
			HandlerChain chain = ie?.WindowChain;

			if ( chain != null )
			{
				_targetInteractee = ie;

				MEC.Timing.RunThisCoroutine( chain.RunWindowChain( ie, this ),
					out _windowHandle );
			}
		}

		public void RunWindowReleaseChain( Interactee ie )
		{
			HandlerChain chain = ie?.WindowReleaseChain;

			if ( chain != null )
			{
				_targetInteractee = ie;

				MEC.Timing.RunThisCoroutine( chain.RunWindowReleaseChain( ie, this ),
					out _windowReleaseHandle );
			}
		}

		public void RunHoldingChain( Interactee ie )
		{
			HandlerChain chain = ie?.HoldingChain;

			if ( chain != null )
			{
				_targetInteractee = ie;

				MEC.Timing.RunThisCoroutine( chain.RunHandlerChain( ie, this ),
					out _holdingHandle );
			}
		}

		public void RunHoldingReleaseChain( Interactee ie )
		{
			HandlerChain chain = ie?.HoldingReleaseChain;

			if ( chain != null )
			{
				_targetInteractee = ie;

				MEC.Timing.RunThisCoroutine( chain.RunHandlerReleaseChain( ie, this ),
					out _holdingReleaseHandle );
			}
		}

		public void RunFocusChain( Interactee ie )
		{
			HandlerChain chain = ie?.FocusChain;

			if ( chain != null )
			{
				_targetInteractee = ie;

				MEC.Timing.RunThisCoroutine( chain.RunHandlerChain( ie, this ),
					out _focusHandle );
			}
		}

		public void RunFocusReleaseChain( Interactee ie )
		{
			HandlerChain chain = ie?.FocusReleaseChain;

			if ( chain != null )
			{
				_targetInteractee = ie;

				MEC.Timing.RunThisCoroutine( chain.RunHandlerReleaseChain( ie, this ),
					out _focusReleaseHandle );
			}
		}

		public void RunUsageChain( Interactee ie )
		{
			HandlerChain chain = ie?.UsageChain;

			if ( chain != null )
			{
				_targetInteractee = ie;

				MEC.Timing.RunThisCoroutine( chain.RunHandlerChain( ie, this ),
					out _usageHandle );
			}
		}

		public void KillOneShotChain()
		{
			if ( _oneShotHandle.IsValid )
			{
				//Debug.Log( "Interactor.KillOneShotChain()" );
				KillChain( ref _oneShotHandle );
			}
		}

		public void KillOneShotReleaseChain( )
		{
			if ( _oneShotReleaseHandle.IsValid )
			{
				//Debug.Log( "Interactor.KillOneShotReleaseChain()" );
				KillChain( ref _oneShotReleaseHandle );
			}
		}

		public void KillWindowChain()
		{
			if ( _windowHandle.IsValid )
			{
				//Debug.Log( "Interactor.KillWindowChain()" );
				KillChain( ref _windowHandle );
			}
		}

		public void KillWindowReleaseChain( )
		{
			if ( _windowReleaseHandle.IsValid )
			{
				//Debug.Log( "Interactor.KillWindowReleaseChain()" );
				KillChain( ref _windowReleaseHandle );
			}
		}

		public void KillHoldingChain()
		{
			if ( _holdingHandle.IsValid )
			{
				//Debug.Log( "Interactor.KillHoldigChain()" );
				KillChain( ref _holdingHandle );
			}
		}

		public void KillHoldingReleaseChain( )
		{
			if ( _holdingReleaseHandle.IsValid )
			{
				//Debug.Log( "Interactor.KillHoldingReleaseChain()" );
				KillChain( ref _holdingReleaseHandle );
			}
		}

		public void KillFocusChain()
		{
			if ( _focusHandle.IsValid )
			{
				//Debug.Log( "Interactor.KillFocusChain()" );
				KillChain( ref _focusHandle );
			}
		}

		public void KillFocusReleaseChain( )
		{
			if ( _focusReleaseHandle.IsValid )
			{
				//Debug.Log( "Interactor.KillFocusReleaseChain()" );
				KillChain( ref _focusReleaseHandle );
			}
		}

		public void KillUsageChain( )
		{
			if ( _usageHandle.IsValid )
			{
				KillChain( ref _usageHandle );
			}
		}

		public void AbortOneShotChain( Interactee ie, HandlerChain chain )
		{
			if ( _oneShotHandle.IsValid )
			{
				//Debug.Log( "Interactor.AbortOneShotChain()" );
				KillChain( ref _oneShotHandle );
				MEC.Timing.RunThisCoroutine( chain.RunHandlerAbortChain( ie, this ),
					out _abortHandle );
			}
		}

		public void AbortWindowChain( Interactee ie, HandlerChain chain )
		{
			if ( _windowHandle.IsValid )
			{
				//Debug.Log( "Interactor.AbortWindowChain()" );
				KillChain( ref _windowHandle );
				MEC.Timing.RunThisCoroutine( chain.RunWindowAbortChain( ie, this ),
					out _abortHandle );
			}
		}

		public void AbortHoldingChain( Interactee ie, HandlerChain chain )
		{
			if ( _holdingHandle.IsValid )
			{
				//Debug.Log( "Interactor.AbortHoldingChain()" );
				KillChain( ref _holdingHandle );
				MEC.Timing.RunThisCoroutine( chain.RunHandlerAbortChain( ie, this ),
					out _abortHandle );
			}
		}

		public void AbortFocusChain( Interactee ie, HandlerChain chain )
		{
			if ( _focusHandle.IsValid )
			{
				//Debug.Log( "Interactor.AbortFocusChain" );
				KillChain( ref _focusHandle );
				MEC.Timing.RunThisCoroutine( chain.RunHandlerAbortChain( ie, this ),
					out _abortHandle );
			}
		}

		public void AbortUsageChain( Interactee ie, HandlerChain chain )
		{
			if ( _usageHandle.IsValid )
			{
				KillChain( ref _usageHandle );
				MEC.Timing.RunThisCoroutine( chain.RunHandlerAbortChain( ie, this ),
					out _abortHandle );
			}
		}
		/*
		private void RunChain( Interactee ie, HandlerChain chain, ref MEC.CoroutineHandle handle,
			HandlerChain.ChainType chainType )
		{
			//if ( ie.IsReceptive == true )
			//{
				_targetInteractee = ie;

				if ( chain == null )
				{
					Debug.LogError( "Chain is null!" );
					return;
				}

				switch ( chainType )
				{
					case HandlerChain.ChainType.Window:
						//Debug.Log( "Interactor.RunChain() Window " + chain );
						MEC.Timing.RunThisCoroutine( chain.RunWindowChain( ie, this ),
							out handle );
						break;
					case HandlerChain.ChainType.WindowRelease:
						//Debug.Log( "Interactor.RunChain() WindowRelease " + chain );
						MEC.Timing.RunThisCoroutine( chain.RunWindowReleaseChain( ie, this ),
							out handle );
						break;
					case HandlerChain.ChainType.WindowAbort:
						//Debug.Log( "Interactor.RunChain() WindowAbort " + chain );
						MEC.Timing.RunThisCoroutine( chain.RunWindowAbortChain( ie, this ),
							out handle );
						break;
					case HandlerChain.ChainType.Handler:
						//Debug.Log( "Interactor.RunChain() Handler " + chain );
						MEC.Timing.RunThisCoroutine( chain.RunHandlerChain( ie, this ), 
							out handle );
						break;
					case HandlerChain.ChainType.HandlerRelease:
						//Debug.Log( "Interactor.RunChain() HandlerRelease " + chain );
						MEC.Timing.RunThisCoroutine( chain.RunHandlerReleaseChain( ie, this ),
							out handle );
						break;
					case HandlerChain.ChainType.HandlerAbort:
						//Debug.Log( "Interactor.RunChain() AbortHandler " + chain );
						MEC.Timing.RunThisCoroutine( chain.RunHandlerAbortChain( ie, this ),
							out handle );
						break;
				}
			//}
		}*/

		private void KillChain( ref MEC.CoroutineHandle handle )
		{
			if ( handle.IsRunning )
			{
				MEC.Timing.KillCoroutines( handle );
			}

			handle = MEC.CoroutineHandle.RawHandle;
			_targetInteractee = null;
		}
	}


}