namespace ProjectFound.Environment.Handlers
{

	using System.Collections;

	public abstract class InteracteeHandler : BaseHandler
	{
		public abstract IEnumerator Handle( Interactee ie, Interactor ir );
	}

}
