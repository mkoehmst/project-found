namespace ProjectFound.Environment.Handlers
{

	using System.Collections.Generic;

	public abstract class InteracteeHandler : BaseHandler
	{
		public abstract IEnumerator<float> Handle( Interactee ie, Interactor ir );
	}

}
