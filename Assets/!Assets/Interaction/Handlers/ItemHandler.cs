namespace ProjectFound.Environment.Handlers
{

	using System.Collections;
	using ProjectFound.Environment.Props;

	public abstract class ItemHandler : PropHandler
	{
		public virtual IEnumerator HandleUsage( Item ie, Interactor ir )
		{ yield break; }
	}


}